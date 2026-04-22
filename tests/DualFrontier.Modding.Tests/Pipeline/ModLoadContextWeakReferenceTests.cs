using System;
using System.Runtime.CompilerServices;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// Verifies that <see cref="ModLoader.UnloadMod"/> physically releases the
/// mod's <see cref="ModLoadContext"/> — the acceptance criterion for
/// Phase 2 per <c>docs/MOD_PIPELINE.md</c> и <c>docs/ROADMAP.md</c>.
/// </summary>
public sealed class ModLoadContextWeakReferenceTests
{
    [Fact]
    public void Mod_Unload_releases_AssemblyLoadContext()
    {
        WeakReference weakRef = LoadAndUnload();

        // Ссылки на контекст оборваны и Unload() уже вызван. Цикл с WaitForPendingFinalizers
        // нужен потому, что коллекция collectible ALC требует нескольких проходов GC
        // чтобы снять внутренние финализаторы.
        for (int i = 0; i < 10 && weakRef.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        weakRef.IsAlive.Should().BeFalse(
            "ModLoadContext must be collected after Unload — иначе горячая перезагрузка " +
            "сохранит предыдущий AssemblyLoadContext и утечёт сборку мода.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference LoadAndUnload()
    {
        // Весь strong-граф создаётся в этом методе: по возврату все локальные
        // переменные покидают корневой набор, оставляя только WeakReference.
        var loader = new ModLoader();
        var manifest = new ModManifest
        {
            Id = "com.example.weak",
            Name = "Weak",
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = "1.0.0",
        };
        var context = new ModLoadContext("com.example.weak");
        var mod = new LoadedMod(
            ModId: "com.example.weak",
            Manifest: manifest,
            Instance: new NoOpMod(),
            Context: context,
            DeclaredSystemTypes: Array.Empty<Type>());
        loader.RegisterLoaded(mod);

        var weak = new WeakReference(context);
        loader.UnloadMod("com.example.weak");
        return weak;
    }

    private sealed class NoOpMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
