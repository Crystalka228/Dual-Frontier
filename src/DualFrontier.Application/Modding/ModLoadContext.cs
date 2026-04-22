using System;
using System.Reflection;
using System.Runtime.Loader;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Собственный <see cref="AssemblyLoadContext"/> для одного мода.
/// Создаётся с <c>isCollectible: true</c>, чтобы мод можно было выгрузить
/// без перезапуска игры. Физически изолирует сборку мода от ядра
/// (TechArch 11.8).
/// </summary>
internal sealed class ModLoadContext : AssemblyLoadContext
{
    /// <summary>
    /// TODO: Фаза 2 — создаёт контекст для мода с заданным именем.
    /// Имя используется для диагностики и горячей выгрузки.
    /// </summary>
    /// <param name="name">Уникальное имя контекста (обычно id мода).</param>
    public ModLoadContext(string name)
        : base(name, isCollectible: true)
    {
    }

    /// <summary>
    /// Resolves a mod dependency by delegating to the default context.
    /// Returning <c>null</c> lets the parent load shared assemblies such as
    /// <c>DualFrontier.Contracts</c>, while the private context still owns
    /// the mod's own assemblies loaded via <c>LoadFromAssemblyPath</c> —
    /// keeping them isolated and collectible. A follow-up phase can filter
    /// the delegated set to harden isolation against accidental loads of
    /// <c>DualFrontier.Core</c> and siblings.
    /// </summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return null;
    }
}
