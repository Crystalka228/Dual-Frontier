using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Seven-phase validator executed before <see cref="DualFrontier.Core.Scheduling.DependencyGraph"/>
/// registration. Phase A checks that every loaded mod is compatible with the
/// current <see cref="ContractsVersion"/>. Phase B inspects every mod system
/// alongside the provided core systems and reports any write-write collision
/// on a component type — producing a precise per-mod diagnostic the scheduler
/// would otherwise surface only as an opaque <c>write conflict detected</c>.
/// Phase E rejects regular mods whose assemblies export a type implementing
/// <see cref="IEvent"/> or <see cref="IModContract"/> — those marker
/// interfaces define cross-mod identities and must live in shared mods
/// (MOD_OS_ARCHITECTURE §5, §6.5 D-4). Phase C verifies every
/// <c>capabilities.required</c> token is provided by the kernel or by an
/// explicitly listed dependency. Phase D cross-checks each mod system's
/// <see cref="ModCapabilitiesAttribute"/> against the owning manifest.
/// Phase F validates each <see cref="LoadedSharedMod"/> against the
/// shared-mod compliance rules of MOD_OS_ARCHITECTURE §5.2: manifest must
/// have empty <c>entryAssembly</c>, <c>entryType</c>, and <c>replaces</c>
/// fields, and the loaded assembly must contain no <see cref="IMod"/>
/// implementation. Phase G validates inter-mod dependency version
/// constraints — every non-null <see cref="ModDependency.Version"/> must be
/// satisfied by the declared version of the providing mod in the load batch.
/// Missing providers are skipped silently;
/// <see cref="ModIntegrationPipeline.CheckDependencyPresence"/> catches that
/// case at M5.1 pipeline level. Phases A, B, E and G run unconditionally;
/// phases C and D run only when a <see cref="KernelCapabilityRegistry"/> is
/// supplied; phase F runs only when a shared-mod list is supplied to
/// <see cref="Validate"/>.
///
/// Non-throwing by design: the pipeline decides whether to abort or surface
/// warnings in the UI.
/// </summary>
internal sealed class ContractValidator
{
    // Marker for "core" in the writer-ownership table.
    // Cannot collide with a legal modId (mod ids use reverse-domain form).
    private const string CoreOwner = "<core>";

    /// <summary>
    /// Runs every applicable validation phase and returns a report. A
    /// mod-system that lacks <see cref="SystemAccessAttribute"/> is skipped
    /// silently here — registration itself throws a clearer diagnostic at
    /// that point. Phases C and D run only when
    /// <paramref name="kernelCapabilities"/> is non-null; phase F runs only
    /// when <paramref name="sharedMods"/> is non-null; legacy callers that
    /// omit either continue to exercise Phases A, B and E.
    /// </summary>
    /// <param name="mods">Mods returned by <see cref="ModLoader"/>.</param>
    /// <param name="coreSystems">Core systems included in the rebuild.</param>
    /// <param name="kernelCapabilities">
    /// Kernel-provided capability tokens. <see langword="null"/> opts out of
    /// Phases C and D entirely (used by tests that predate capability
    /// validation).
    /// </param>
    /// <param name="sharedMods">
    /// Shared mods loaded into the shared ALC. <see langword="null"/> opts
    /// out of Phase F entirely (used by tests that predate shared-mod
    /// compliance validation).
    /// </param>
    public ValidationReport Validate(
        IReadOnlyList<LoadedMod> mods,
        IReadOnlyList<SystemBase> coreSystems,
        KernelCapabilityRegistry? kernelCapabilities = null,
        IReadOnlyList<LoadedSharedMod>? sharedMods = null)
    {
        if (mods is null) throw new ArgumentNullException(nameof(mods));
        if (coreSystems is null) throw new ArgumentNullException(nameof(coreSystems));

        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        ValidateContractsVersions(mods, errors);
        ValidateWriteWriteConflicts(mods, coreSystems, errors);
        ValidateRegularModContractTypes(mods, errors);
        ValidateInterModDependencyVersions(mods, errors);

        if (kernelCapabilities is not null)
        {
            ValidateCapabilitySatisfiability(mods, kernelCapabilities, errors);
            ValidateModCapabilitiesAttributes(mods, errors);
        }

        if (sharedMods is not null)
        {
            ValidateSharedModCompliance(sharedMods, errors);
        }

        bool isValid = errors.Count == 0;
        return new ValidationReport(isValid, errors, warnings);
    }

    /// <summary>
    /// Phase A — kernel API version compatibility. Dual-path for backward
    /// compat: when <see cref="ModManifest.ApiVersion"/> is <see langword="null"/>
    /// (v1 manifest), the legacy path parses
    /// <see cref="ModManifest.RequiresContractsVersion"/> as a
    /// <see cref="ContractsVersion"/> and uses
    /// <see cref="ContractsVersion.IsCompatible"/>; failures surface as
    /// <see cref="ValidationErrorKind.IncompatibleContractsVersion"/>. When
    /// <see cref="ModManifest.ApiVersion"/> is non-null (v2 manifest), the
    /// typed <see cref="VersionConstraint"/> pipeline checks
    /// <see cref="VersionConstraint.IsSatisfiedBy"/> against
    /// <see cref="ContractsVersion.Current"/> and surfaces failures as
    /// <see cref="ValidationErrorKind.IncompatibleVersion"/> per
    /// MOD_OS_ARCHITECTURE §11.2 M5 spec.
    /// </summary>
    private static void ValidateContractsVersions(
        IReadOnlyList<LoadedMod> mods,
        List<ValidationError> errors)
    {
        ContractsVersion current = ContractsVersion.Current;

        foreach (LoadedMod mod in mods)
        {
            if (mod.Manifest.ApiVersion is null)
            {
                // v1 manifest — preserve legacy behavior: parse
                // RequiresContractsVersion as ContractsVersion, check via
                // ContractsVersion.IsCompatible. Failure mode is legacy
                // IncompatibleContractsVersion error kind.
                ContractsVersion required;
                try
                {
                    required = ContractsVersion.Parse(mod.Manifest.RequiresContractsVersion);
                }
                catch (FormatException ex)
                {
                    errors.Add(new ValidationError(
                        mod.ModId,
                        ValidationErrorKind.IncompatibleContractsVersion,
                        $"Mod '{mod.ModId}' has invalid requiresContracts: {ex.Message}"));
                    continue;
                }

                if (!ContractsVersion.IsCompatible(required, current))
                {
                    errors.Add(new ValidationError(
                        mod.ModId,
                        ValidationErrorKind.IncompatibleContractsVersion,
                        $"Mod '{mod.ModId}' requires DualFrontier.Contracts {required} " +
                        $"but the current build provides {current}."));
                }
            }
            else
            {
                // v2 manifest — typed VersionConstraint pipeline.
                // Failure mode is IncompatibleVersion (per §11.2 M5 spec).
                VersionConstraint constraint = mod.Manifest.ApiVersion.Value;
                if (!constraint.IsSatisfiedBy(current))
                {
                    errors.Add(new ValidationError(
                        mod.ModId,
                        ValidationErrorKind.IncompatibleVersion,
                        $"Mod '{mod.ModId}' requires DualFrontier.Contracts " +
                        $"{constraint} but the current build provides {current}. " +
                        "Per MOD_OS_ARCHITECTURE §8.1, kernel API constraint must " +
                        "be satisfied at load time."));
                }
            }
        }
    }

    private readonly struct WriteEntry
    {
        public readonly string OwnerId;
        public readonly Type SystemType;
        public readonly HashSet<Type> Writes;

        public WriteEntry(string ownerId, Type systemType, HashSet<Type> writes)
        {
            OwnerId = ownerId;
            SystemType = systemType;
            Writes = writes;
        }

        public bool IsCore => OwnerId == CoreOwner;
    }

    private static void ValidateWriteWriteConflicts(
        IReadOnlyList<LoadedMod> mods,
        IReadOnlyList<SystemBase> coreSystems,
        List<ValidationError> errors)
    {
        var entries = new List<WriteEntry>();

        foreach (SystemBase core in coreSystems)
        {
            Type t = core.GetType();
            HashSet<Type>? writes = ReadWrites(t);
            if (writes is not null && writes.Count > 0)
                entries.Add(new WriteEntry(CoreOwner, t, writes));
        }

        foreach (LoadedMod mod in mods)
        {
            foreach (Type systemType in EnumerateDeclaredSystemTypes(mod))
            {
                HashSet<Type>? writes = ReadWrites(systemType);
                if (writes is not null && writes.Count > 0)
                    entries.Add(new WriteEntry(mod.ModId, systemType, writes));
            }
        }

        // Pairwise check — same algorithm as in DependencyGraph, but attributed.
        for (int i = 0; i < entries.Count; i++)
        {
            WriteEntry a = entries[i];
            for (int j = i + 1; j < entries.Count; j++)
            {
                WriteEntry b = entries[j];
                // The same mod system may appear twice (e.g. via two assemblies) —
                // a self-entry is not a conflict.
                if (a.OwnerId == b.OwnerId && a.SystemType == b.SystemType)
                    continue;

                Type? shared = FindIntersection(a.Writes, b.Writes);
                if (shared is null)
                    continue;

                ReportConflict(errors, a, b, shared);
            }
        }
    }

    private static void ReportConflict(
        List<ValidationError> errors,
        WriteEntry a,
        WriteEntry b,
        Type component)
    {
        string aLabel = a.IsCore
            ? $"core system '{a.SystemType.FullName}'"
            : $"mod '{a.OwnerId}' system '{a.SystemType.FullName}'";
        string bLabel = b.IsCore
            ? $"core system '{b.SystemType.FullName}'"
            : $"mod '{b.OwnerId}' system '{b.SystemType.FullName}'";

        string message =
            $"Write-write conflict on '{component.FullName}': " +
            $"{aLabel} and {bLabel} both declare writes.";

        // Each ValidationError is attributed to a specific mod (never to core)
        // so the UI can flag that mod's card as "conflicting".
        if (!a.IsCore)
        {
            errors.Add(new ValidationError(
                a.OwnerId,
                ValidationErrorKind.WriteWriteConflict,
                message,
                ConflictingModId: b.IsCore ? null : b.OwnerId,
                ConflictingComponent: component));
        }
        if (!b.IsCore && b.OwnerId != a.OwnerId)
        {
            errors.Add(new ValidationError(
                b.OwnerId,
                ValidationErrorKind.WriteWriteConflict,
                message,
                ConflictingModId: a.IsCore ? null : a.OwnerId,
                ConflictingComponent: component));
        }
    }

    private static HashSet<Type>? ReadWrites(Type systemType)
    {
        SystemAccessAttribute? access =
            systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false);
        if (access is null)
            return null;
        var set = new HashSet<Type>();
        foreach (Type t in access.Writes)
            set.Add(t);
        return set;
    }

    private static IEnumerable<Type> EnumerateDeclaredSystemTypes(LoadedMod mod)
    {
        // The list is assembled by the loader before Initialize is called — the validator only reads it.
        foreach (Type t in mod.DeclaredSystemTypes)
        {
            if (t is null) continue;
            if (!typeof(SystemBase).IsAssignableFrom(t)) continue;
            if (t.IsAbstract) continue;
            yield return t;
        }
    }

    private static Type? FindIntersection(HashSet<Type> a, HashSet<Type> b)
    {
        HashSet<Type> smaller = a.Count <= b.Count ? a : b;
        HashSet<Type> larger = ReferenceEquals(smaller, a) ? b : a;
        foreach (Type t in smaller)
        {
            if (larger.Contains(t))
                return t;
        }
        return null;
    }

    /// <summary>
    /// Phase E — every regular mod's assemblies are scanned and any exported
    /// type implementing <see cref="IEvent"/> or <see cref="IModContract"/>
    /// is reported. Per MOD_OS_ARCHITECTURE §5/§6.5 D-4 those marker
    /// interfaces define cross-mod identities: a type defined inside a regular
    /// mod's collectible <see cref="ModLoadContext"/> is invisible to other
    /// mods because each lives in its own ALC. Such types must be vended by a
    /// shared mod so all regular mods resolve to the same <see cref="Type"/>
    /// instance through the shared ALC. <see cref="LoadedSharedMod"/>s never
    /// reach this validator; the manifest <c>Kind</c> guard below is defensive
    /// against synthetic <see cref="LoadedMod"/>s with <see cref="ModKind.Shared"/>.
    /// </summary>
    private static void ValidateRegularModContractTypes(
        IReadOnlyList<LoadedMod> mods,
        List<ValidationError> errors)
    {
        foreach (LoadedMod mod in mods)
        {
            if (mod.Manifest.Kind != ModKind.Regular)
                continue;

            foreach (Assembly asm in mod.Context.Assemblies)
            {
                // Defensive: only scan assemblies physically owned by this
                // mod's ALC. A delegated shared assembly returned via
                // ModLoadContext.Load belongs to the shared ALC and was
                // already vetted at the source.
                if (AssemblyLoadContext.GetLoadContext(asm) != mod.Context)
                    continue;

                Type[] exported;
                try
                {
                    exported = asm.GetExportedTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    exported = ex.Types as Type[] ?? Array.Empty<Type>();
                }

                foreach (Type t in exported)
                {
                    if (t is null) continue;
                    // The marker interfaces themselves live in
                    // DualFrontier.Contracts (default ALC) and never appear
                    // here — guarded only as a paranoid no-op.
                    if (t == typeof(IEvent) || t == typeof(IModContract))
                        continue;

                    if (typeof(IEvent).IsAssignableFrom(t))
                        errors.Add(BuildContractTypeError(mod, asm, t, nameof(IEvent)));

                    if (typeof(IModContract).IsAssignableFrom(t))
                        errors.Add(BuildContractTypeError(mod, asm, t, nameof(IModContract)));
                }
            }
        }
    }

    private static ValidationError BuildContractTypeError(
        LoadedMod mod, Assembly asm, Type type, string interfaceName)
    {
        string asmName = asm.GetName().Name ?? asm.FullName ?? "<unknown>";
        return new ValidationError(
            mod.ModId,
            ValidationErrorKind.ContractTypeInRegularMod,
            $"Mod '{mod.ModId}' assembly '{asmName}' exports type " +
            $"'{type.FullName}' which implements '{interfaceName}'. Contract " +
            "and event types must live in shared mods so every regular mod " +
            "resolves to the same Type instance through the shared ALC — " +
            "move it to a separate mod with kind=\"shared\" " +
            "(MOD_OS_ARCHITECTURE §5, §6.5 D-4).");
    }

    /// <summary>
    /// Phase C — every <c>capabilities.required</c> token must be provided
    /// by the kernel or by a mod that is explicitly listed in this mod's
    /// <c>dependencies</c>. Implicit satisfaction (a loaded mod that happens
    /// to provide the token but is not listed) is rejected per
    /// MOD_OS_ARCHITECTURE §3.4.
    /// </summary>
    private static void ValidateCapabilitySatisfiability(
        IReadOnlyList<LoadedMod> mods,
        KernelCapabilityRegistry kernelCapabilities,
        List<ValidationError> errors)
    {
        foreach (LoadedMod mod in mods)
        {
            foreach (string token in mod.Manifest.Capabilities.Required)
            {
                if (kernelCapabilities.Provides(token))
                    continue;

                bool satisfied = false;
                foreach (ModDependency dep in mod.Manifest.Dependencies)
                {
                    LoadedMod? provider = FindMod(mods, dep.ModId);
                    if (provider is null)
                        continue;
                    if (provider.Manifest.Capabilities.ProvidesCapability(token))
                    {
                        satisfied = true;
                        break;
                    }
                }

                if (!satisfied)
                {
                    errors.Add(new ValidationError(
                        mod.ModId,
                        ValidationErrorKind.MissingCapability,
                        $"Mod '{mod.ModId}' requires capability '{token}' " +
                        "which is not provided by the kernel or any listed dependency."));
                }
            }
        }
    }

    /// <summary>
    /// Phase D — every token declared by a mod system's
    /// <see cref="ModCapabilitiesAttribute"/> must also appear in the owning
    /// mod's <c>capabilities.required</c> list. Closes the loophole where a
    /// system silently demands kernel access without declaring it in the
    /// manifest (MOD_OS_ARCHITECTURE §3.7–3.8).
    /// </summary>
    private static void ValidateModCapabilitiesAttributes(
        IReadOnlyList<LoadedMod> mods,
        List<ValidationError> errors)
    {
        foreach (LoadedMod mod in mods)
        {
            foreach (Type systemType in EnumerateDeclaredSystemTypes(mod))
            {
                ModCapabilitiesAttribute? attr =
                    systemType.GetCustomAttribute<ModCapabilitiesAttribute>(inherit: false);
                if (attr is null)
                    continue;

                foreach (string token in attr.Tokens)
                {
                    if (mod.Manifest.Capabilities.RequiresCapability(token))
                        continue;

                    errors.Add(new ValidationError(
                        mod.ModId,
                        ValidationErrorKind.MissingCapability,
                        $"System '{systemType.FullName}' in mod '{mod.ModId}' " +
                        $"declares [ModCapabilities(\"{token}\")] but the " +
                        $"manifest does not list '{token}' in capabilities.required."));
                }
            }
        }
    }

    private static LoadedMod? FindMod(IReadOnlyList<LoadedMod> mods, string modId)
    {
        foreach (LoadedMod m in mods)
        {
            if (m.ModId == modId)
                return m;
        }
        return null;
    }

    /// <summary>
    /// Phase G — inter-mod dependency version check. For every
    /// <see cref="ModDependency"/> with a non-null <c>Version</c> constraint,
    /// finds the provider mod in the load batch and verifies the constraint
    /// is satisfied by the provider's <see cref="ModManifest.Version"/>.
    /// Missing provider mods are skipped silently — that case is
    /// <see cref="ModIntegrationPipeline.CheckDependencyPresence"/>'s
    /// responsibility (M5.1). A malformed provider version string surfaces
    /// as <see cref="ValidationErrorKind.IncompatibleVersion"/> attributed
    /// to the provider (the mod author's mistake).
    ///
    /// Cascade-failure semantics: errors accumulate; no mod is silently
    /// dropped if its provider fails its own validation. Per
    /// MOD_OS_ARCHITECTURE §8.7 and existing pipeline accumulation pattern.
    /// </summary>
    private static void ValidateInterModDependencyVersions(
        IReadOnlyList<LoadedMod> mods,
        List<ValidationError> errors)
    {
        foreach (LoadedMod mod in mods)
        {
            foreach (ModDependency dep in mod.Manifest.Dependencies)
            {
                if (!dep.Version.HasValue)
                    continue;  // No version constraint — presence-only dep.

                LoadedMod? provider = FindMod(mods, dep.ModId);
                if (provider is null)
                    continue;  // Missing provider — M5.1's CheckDependencyPresence handled it.

                ContractsVersion providerVersion;
                try
                {
                    providerVersion = ContractsVersion.Parse(provider.Manifest.Version);
                }
                catch (FormatException ex)
                {
                    errors.Add(new ValidationError(
                        provider.ModId,
                        ValidationErrorKind.IncompatibleVersion,
                        $"Mod '{provider.ModId}' has invalid version " +
                        $"'{provider.Manifest.Version}': {ex.Message}. " +
                        "Per MOD_OS_ARCHITECTURE §2.2, the version field must " +
                        "be a valid SemVer MAJOR.MINOR.PATCH."));
                    continue;
                }

                if (!dep.Version.Value.IsSatisfiedBy(providerVersion))
                {
                    errors.Add(new ValidationError(
                        mod.ModId,
                        ValidationErrorKind.IncompatibleVersion,
                        $"Mod '{mod.ModId}' requires '{dep.ModId}' version " +
                        $"{dep.Version.Value} but the loaded version is " +
                        $"{providerVersion}. Per MOD_OS_ARCHITECTURE §8.7, " +
                        "inter-mod dependency version constraints must be " +
                        "satisfied. Update one or both mods, or remove the " +
                        "dependency."));
                }
            }
        }
    }

    /// <summary>
    /// Phase F — every <see cref="LoadedSharedMod"/> is verified against
    /// MOD_OS_ARCHITECTURE §5.2: shared mods are pure type vendors, so the
    /// manifest's <c>entryAssembly</c>, <c>entryType</c>, and <c>replaces</c>
    /// fields must be empty, and the loaded assembly must contain no type
    /// implementing <see cref="IMod"/>. Each violation produces a typed
    /// <see cref="ValidationErrorKind.SharedModWithEntryPoint"/> error
    /// naming the offending field or type. Phase F is the single source of
    /// truth for shared-mod compliance — <see cref="ModLoader.LoadSharedMod"/>
    /// no longer guards against IMod-bearing shared assemblies (M4.3).
    /// </summary>
    private static void ValidateSharedModCompliance(
        IReadOnlyList<LoadedSharedMod> sharedMods,
        List<ValidationError> errors)
    {
        foreach (LoadedSharedMod mod in sharedMods)
        {
            if (!string.IsNullOrEmpty(mod.Manifest.EntryAssembly))
            {
                errors.Add(new ValidationError(
                    mod.ModId,
                    ValidationErrorKind.SharedModWithEntryPoint,
                    $"Shared mod '{mod.ModId}' has non-empty entryAssembly " +
                    $"('{mod.Manifest.EntryAssembly}'). Shared mods are pure " +
                    "type vendors per MOD_OS_ARCHITECTURE §5.2 — remove the " +
                    "entryAssembly field from the manifest."));
            }

            if (!string.IsNullOrEmpty(mod.Manifest.EntryType))
            {
                errors.Add(new ValidationError(
                    mod.ModId,
                    ValidationErrorKind.SharedModWithEntryPoint,
                    $"Shared mod '{mod.ModId}' has non-empty entryType " +
                    $"('{mod.Manifest.EntryType}'). Shared mods are pure type " +
                    "vendors per MOD_OS_ARCHITECTURE §5.2 — remove the " +
                    "entryType field from the manifest."));
            }

            if (mod.Manifest.Replaces.Count > 0)
            {
                errors.Add(new ValidationError(
                    mod.ModId,
                    ValidationErrorKind.SharedModWithEntryPoint,
                    $"Shared mod '{mod.ModId}' has non-empty replaces. Shared " +
                    "mods cannot replace systems — they have no executable " +
                    "code per MOD_OS_ARCHITECTURE §5.2. Remove the replaces " +
                    "field from the manifest."));
            }

            foreach (Assembly asm in mod.Context.Assemblies)
            {
                // Defensive: only scan assemblies physically owned by this
                // shared ALC. Anything resolved via SharedModLoadContext.Load
                // for the default ALC (e.g. DualFrontier.Contracts) belongs
                // to the kernel and is not a shared-mod assembly.
                if (AssemblyLoadContext.GetLoadContext(asm) != mod.Context)
                    continue;

                Type[] exported;
                try
                {
                    exported = asm.GetExportedTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    exported = ex.Types as Type[] ?? Array.Empty<Type>();
                }

                foreach (Type t in exported)
                {
                    if (t is null) continue;
                    if (t.IsAbstract || t.IsInterface) continue;
                    if (typeof(IMod).IsAssignableFrom(t))
                    {
                        string asmName = asm.GetName().Name ?? asm.FullName ?? "<unknown>";
                        errors.Add(new ValidationError(
                            mod.ModId,
                            ValidationErrorKind.SharedModWithEntryPoint,
                            $"Shared mod '{mod.ModId}' assembly '{asmName}' " +
                            $"contains type '{t.FullName}' implementing IMod. " +
                            "Shared mods are pure type vendors and cannot have " +
                            "entry points per MOD_OS_ARCHITECTURE §5.2. Move " +
                            "the IMod implementation to a separate regular " +
                            "mod, or change this manifest's kind to \"regular\"."));
                    }
                }
            }
        }
    }
}
