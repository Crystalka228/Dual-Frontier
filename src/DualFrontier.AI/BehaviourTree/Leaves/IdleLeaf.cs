namespace DualFrontier.AI.BehaviourTree.Leaves;

using DualFrontier.AI.BehaviourTree;
using System;

/// <summary>
/// Action leaf: always returns Success. Used as fallback in Selector.
/// </summary>
public sealed class IdleLeaf : Leaf
{
    public override BTStatus Tick(BTContext ctx) => BTStatus.Success;
}