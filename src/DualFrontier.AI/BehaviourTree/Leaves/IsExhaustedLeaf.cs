namespace DualFrontier.AI.BehaviourTree.Leaves;

using DualFrontier.Components.Pawn;
using DualFrontier.AI.BehaviourTree;
using System;

/// <summary>
/// Condition leaf: returns Success if pawn is exhausted, Failure otherwise.
///
/// TODO(Phase 4): the blackboard key "Sleep" is not yet populated by any
/// producer — see note on <see cref="IsHungryLeaf"/> for the broader gap.
/// </summary>
public sealed class IsExhaustedLeaf : Leaf
{
    public override BTStatus Tick(BTContext ctx)
    {
        var sleep = ctx.Blackboard.Get<float>("Sleep");
        return sleep <= NeedsComponent.CriticalThreshold
            ? BTStatus.Success
            : BTStatus.Failure;
    }
}
