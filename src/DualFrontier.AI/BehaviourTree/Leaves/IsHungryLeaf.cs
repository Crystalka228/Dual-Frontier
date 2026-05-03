namespace DualFrontier.AI.BehaviourTree.Leaves;

using DualFrontier.Components.Pawn;
using DualFrontier.AI.BehaviourTree;
using System;

/// <summary>
/// Condition leaf: returns Success if pawn is hungry, Failure otherwise.
///
/// TODO(Phase 4): the blackboard key "Satiety" is not yet populated by any
/// producer — Phase 3 JobSystem uses inline priority logic and does not
/// tick a behaviour tree. Until a BT-runner system seeds the blackboard
/// from NeedsComponent before each tree tick, this leaf reads default(float)
/// (= 0f, which is critically low wellness) and always returns Success.
/// </summary>
public sealed class IsHungryLeaf : Leaf
{
    public override BTStatus Tick(BTContext ctx)
    {
        var satiety = ctx.Blackboard.Get<float>("Satiety");
        return satiety <= NeedsComponent.CriticalThreshold
            ? BTStatus.Success
            : BTStatus.Failure;
    }
}
