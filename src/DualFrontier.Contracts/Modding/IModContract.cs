using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Маркер-интерфейс контракта между модами. Один мод публикует реализацию
/// через <c>IModApi.PublishContract</c>, другие — получают через
/// <c>IModApi.TryGetContract</c>. Это единственный легальный способ
/// коммуникации между модами: прямая ссылка на сборку другого мода
/// невозможна (разные AssemblyLoadContext).
/// </summary>
public interface IModContract
{
}
