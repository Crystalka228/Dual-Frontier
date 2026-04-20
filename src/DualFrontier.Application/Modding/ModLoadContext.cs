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
    /// TODO: Фаза 2 — разрешение зависимостей мода.
    /// Возврат <c>null</c> делегирует разрешение родителю; это позволяет
    /// моду видеть только сборки, которые ядро считает «контрактами»
    /// (например, <c>DualFrontier.Contracts</c>).
    /// </summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        throw new NotImplementedException("TODO: Фаза 2 — разрешение сборок мода");
    }
}
