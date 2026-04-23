namespace DualFrontier.AI.BehaviourTree.Leaves;

using DualFrontier.Components.Pawn;
using DualFrontier.AI.BehaviourTree;
using System;

/// <summary>
/// Condition leaf: returns Success if pawn is exhausted, Failure otherwise.
/// </summary>
public sealed class IsExhaustedLeaf : Leaf
{
    public override BTStatus Tick(BTContext ctx)
    {
        var rest = ctx.Blackboard.Get<float>("Rest");
        return rest >= NeedsComponent.CriticalThreshold
            ? BTStatus.Success
            : BTStatus.Failure;
    }
}
