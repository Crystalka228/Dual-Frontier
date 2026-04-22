using System;
using System.Collections.Generic;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Загружает и выгружает моды. Каждый мод в собственном
/// <see cref="System.Runtime.Loader.AssemblyLoadContext"/> — per TechArch 11.8.
/// Контекст создаётся с <c>isCollectible: true</c>, что позволяет горячую
/// выгрузку и физически изолирует сборку мода от ядра.
/// </summary>
public sealed class ModLoader
{
    /// <summary>
    /// TODO: Фаза 2 — загружает мод из указанного каталога.
    /// Читает <c>mod.manifest.json</c>, создаёт <see cref="ModLoadContext"/>,
    /// загружает entry-assembly, находит реализацию <c>IMod</c> по рефлексии
    /// и вызывает <c>Initialize(RestrictedModApi)</c>.
    /// </summary>
    /// <param name="path">Каталог мода, содержащий манифест и сборку.</param>
    public void LoadMod(string path)
    {
        throw new NotImplementedException("TODO: Фаза 2 — загрузка мода через AssemblyLoadContext");
    }

    /// <summary>
    /// TODO: Фаза 2 — выгружает мод по идентификатору.
    /// Вызывает <c>IMod.Unload</c>, затем <c>ModLoadContext.Unload</c>,
    /// дожидается сбора мусора и освобождения сборки.
    /// </summary>
    /// <param name="id">Идентификатор мода из <c>ModManifest.Id</c>.</param>
    public void UnloadMod(string id)
    {
        throw new NotImplementedException("TODO: Фаза 2 — выгрузка мода");
    }

    /// <summary>
    /// TODO: Фаза 2 — возвращает список идентификаторов загруженных модов.
    /// </summary>
    public IReadOnlyList<string> GetLoaded()
    {
        throw new NotImplementedException("TODO: Фаза 2 — реестр загруженных модов");
    }

    /// <summary>
    /// TODO: Фаза 2 — обрабатывает нарушение изоляции мод-системой.
    /// Последовательность: лог → отписка систем от шин → удаление из
    /// планировщика → IMod.Unload с таймаутом → выгрузка
    /// AssemblyLoadContext → публикация ModDisabledEvent в UI.
    /// </summary>
    public void HandleModFault(string modId, ModIsolationException exception)
    {
        throw new NotImplementedException("TODO: Фаза 2 — ModFaultHandler");
    }
}
