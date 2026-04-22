using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// Coverage for <see cref="ContractValidator"/> strictly following the list
/// documented in MOD_PIPELINE — version check, mod-vs-mod write conflict,
/// mod-vs-core write conflict, valid-mod happy path, and diagnostic payload.
/// </summary>
public sealed class ContractValidatorTests
{
    [Fact]
    public void Validator_rejects_mod_requiring_newer_contracts_version()
    {
        var validator = new ContractValidator();
        // Требуется версия с мажорной бумкой — текущая 1.0.0 несовместима.
        LoadedMod mod = MakeMod("com.example.future", "2.0.0", Array.Empty<Type>());

        ValidationReport report = validator.Validate(new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        report.Errors.Should().ContainSingle();
        report.Errors[0].Kind.Should().Be(ValidationErrorKind.IncompatibleContractsVersion);
        report.Errors[0].ModId.Should().Be("com.example.future");
    }

    [Fact]
    public void Validator_detects_write_conflict_between_two_mods()
    {
        var validator = new ContractValidator();
        LoadedMod modA = MakeMod("com.example.a", "1.0.0", new[] { typeof(WriteASystem) });
        LoadedMod modB = MakeMod("com.example.b", "1.0.0", new[] { typeof(WriteASystemAlt) });

        ValidationReport report = validator.Validate(new[] { modA, modB }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        // Оба мода помечаются как конфликтующие — UI сможет пометить обе карточки.
        report.Errors.Should().Contain(e => e.ModId == "com.example.a" && e.ConflictingModId == "com.example.b");
        report.Errors.Should().Contain(e => e.ModId == "com.example.b" && e.ConflictingModId == "com.example.a");
        report.Errors.Should().OnlyContain(e =>
            e.Kind == ValidationErrorKind.WriteWriteConflict &&
            e.ConflictingComponent == typeof(ComponentA));
    }

    [Fact]
    public void Validator_detects_write_conflict_between_mod_and_core()
    {
        var validator = new ContractValidator();
        LoadedMod mod = MakeMod("com.example.bad", "1.0.0", new[] { typeof(WriteASystem) });
        var core = new CoreWriteASystem();

        ValidationReport report = validator.Validate(new[] { mod }, new SystemBase[] { core });

        report.IsValid.Should().BeFalse();
        ValidationError err = report.Errors.Should().ContainSingle().Subject;
        err.ModId.Should().Be("com.example.bad");
        err.Kind.Should().Be(ValidationErrorKind.WriteWriteConflict);
        err.ConflictingComponent.Should().Be(typeof(ComponentA));
        err.ConflictingModId.Should().BeNull(); // core не имеет modId
        err.Message.Should().Contain("core system");
    }

    [Fact]
    public void Validator_valid_mods_return_empty_errors()
    {
        var validator = new ContractValidator();
        LoadedMod modA = MakeMod("com.example.a", "1.0.0", new[] { typeof(WriteASystem) });
        LoadedMod modB = MakeMod("com.example.b", "1.0.0", new[] { typeof(WriteBSystem) });

        ValidationReport report = validator.Validate(new[] { modA, modB }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validator_reports_precise_component_in_conflict_message()
    {
        var validator = new ContractValidator();
        LoadedMod modA = MakeMod("com.example.a", "1.0.0", new[] { typeof(WriteASystem) });
        LoadedMod modB = MakeMod("com.example.b", "1.0.0", new[] { typeof(WriteASystemAlt) });

        ValidationReport report = validator.Validate(new[] { modA, modB }, Array.Empty<SystemBase>());

        // Точное сообщение должно содержать FQN компонента — иначе UI не сможет показать деталь.
        report.Errors.Should().OnlyContain(e =>
            e.Message.Contains(typeof(ComponentA).FullName!));
        report.Errors.Should().OnlyContain(e => e.Message.Contains("com.example.a"));
        report.Errors.Should().OnlyContain(e => e.Message.Contains("com.example.b"));
    }

    [Fact]
    public void Validator_ok_for_compatible_older_patch_version()
    {
        var validator = new ContractValidator();
        // Current = 1.0.0. Мод требует 1.0.0 — совпадает → валиден.
        LoadedMod mod = MakeMod("com.example.compat", "1.0.0", Array.Empty<Type>());

        ValidationReport report = validator.Validate(new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
    }

    private static LoadedMod MakeMod(string modId, string requiresVersion, Type[] systemTypes)
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = requiresVersion,
        };
        var context = new ModLoadContext(modId);
        IMod instance = new StubMod();
        return new LoadedMod(modId, manifest, instance, context, systemTypes);
    }

    // --- Stubs --------------------------------------------------------------

    public sealed class ComponentA : IComponent
    {
        public int Value { get; init; }
    }

    public sealed class ComponentB : IComponent
    {
        public int Value { get; init; }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ComponentA) }, bus: nameof(IGameServices.World))]
    [TickRate(TickRates.NORMAL)]
    public sealed class WriteASystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ComponentA) }, bus: nameof(IGameServices.World))]
    [TickRate(TickRates.NORMAL)]
    public sealed class WriteASystemAlt : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ComponentB) }, bus: nameof(IGameServices.World))]
    [TickRate(TickRates.NORMAL)]
    public sealed class WriteBSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ComponentA) }, bus: nameof(IGameServices.World))]
    [TickRate(TickRates.NORMAL)]
    public sealed class CoreWriteASystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    private sealed class StubMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
