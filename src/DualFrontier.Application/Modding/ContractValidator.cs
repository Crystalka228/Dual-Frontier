using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Two-phase validator executed before <see cref="DualFrontier.Core.Scheduling.DependencyGraph"/>
/// registration. Phase A checks that every loaded mod is compatible with the
/// current <see cref="ContractsVersion"/>. Phase B inspects every mod system
/// alongside the provided core systems and reports any write-write collision
/// on a component type — producing a precise per-mod diagnostic the scheduler
/// would otherwise surface only as an opaque <c>write conflict detected</c>.
///
/// Non-throwing by design: the pipeline decides whether to abort or surface
/// warnings in the UI.
/// </summary>
internal sealed class ContractValidator
{
    // Маркер «ядро» в таблице владения системой-пишущим компонентом.
    // Не может пересечься с легальным modId (идентификаторы имеют формат reverse-domain).
    private const string CoreOwner = "<core>";

    /// <summary>
    /// Runs both validation phases and returns a report. A mod-system that
    /// lacks <see cref="SystemAccessAttribute"/> is skipped silently here —
    /// registration itself throws a clearer diagnostic at that point.
    /// </summary>
    /// <param name="mods">Mods returned by <see cref="ModLoader"/>.</param>
    /// <param name="coreSystems">Core systems included in the rebuild.</param>
    public ValidationReport Validate(
        IReadOnlyList<LoadedMod> mods,
        IReadOnlyList<SystemBase> coreSystems)
    {
        if (mods is null) throw new ArgumentNullException(nameof(mods));
        if (coreSystems is null) throw new ArgumentNullException(nameof(coreSystems));

        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        ValidateContractsVersions(mods, errors);
        ValidateWriteWriteConflicts(mods, coreSystems, errors);

        bool isValid = errors.Count == 0;
        return new ValidationReport(isValid, errors, warnings);
    }

    private static void ValidateContractsVersions(
        IReadOnlyList<LoadedMod> mods,
        List<ValidationError> errors)
    {
        ContractsVersion current = ContractsVersion.Current;

        foreach (LoadedMod mod in mods)
        {
            // Фаза A — версия контрактов. Битая строка версии считается
            // несовместимостью с точным сообщением для UI.
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

        // Попарная проверка — тот же алгоритм, что в DependencyGraph, но с атрибуцией.
        for (int i = 0; i < entries.Count; i++)
        {
            WriteEntry a = entries[i];
            for (int j = i + 1; j < entries.Count; j++)
            {
                WriteEntry b = entries[j];
                // Одна и та же система мода может встретиться дважды (через разные сборки) —
                // запись о самой себе не является конфликтом.
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

        // Каждая ValidationError атрибутирована конкретному моду (не ядру),
        // чтобы UI мог пометить карточку этого мода как «конфликтующая».
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
        // Список собран загрузчиком до вызова Initialize — валидатор лишь читает.
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
}
