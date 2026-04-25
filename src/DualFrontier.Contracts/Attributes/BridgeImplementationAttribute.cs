using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Помечает систему как временную bridge-реализацию, которая будет заменена
/// полноценной реализацией в указанной фазе. Bridge-системы — стабы, чьи
/// <c>OnInitialize</c> и <c>Update</c> являются no-op (или минимальный
/// stub-granter); важно, что они НЕ бросают <c>NotImplementedException</c>
/// при регистрации в планировщике.
///
/// См. ROADMAP §«Phase 5 — Магия и Phase 6 — Мир: мост между фазами».
/// Введено TechArch v0.3 §13.2. Свойство <see cref="Phase"/> доступно как
/// именованный аргумент (<c>[BridgeImplementation(Phase = 6)]</c>), что
/// требуется для совместимости с уже описанным в ROADMAP синтаксисом и для
/// анализатора, который позже будет предупреждать об оставленных bridge'ах.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class BridgeImplementationAttribute : Attribute
{
    /// <summary>
    /// Номер фазы, в которой bridge должен быть заменён полной реализацией
    /// (по нумерации фаз в <c>docs/ROADMAP.md</c>).
    /// </summary>
    public int Phase { get; set; }
}
