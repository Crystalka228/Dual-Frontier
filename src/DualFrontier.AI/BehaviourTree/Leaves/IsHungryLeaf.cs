namespace DualFrontier.AI.BehaviourTree.Leaves;

using DualFrontier.Components.Pawn;
using DualFrontier.AI.BehaviourTree;
using System;

/// <summary>
/// Condition leaf: returns Success if pawn is hungry, Failure otherwise.
///
/// TODO(Phase 4): the blackboard key "Hunger" is not yet populated by any
/// producer — Phase 3 JobSystem uses inline priority logic and does not
/// tick a behaviour tree. Until a BT-runner system seeds the blackboard
/// from NeedsComponent before each tree tick, this leaf reads default(float)
/// and always returns Failure.
/// </summary>
public sealed class IsHungryLeaf : Leaf
{
    public override BTStatus Tick(BTContext ctx)
    {
        var hunger = ctx.Blackboard.Get<float>("Hunger");
        return hunger >= NeedsComponent.CriticalThreshold
            ? BTStatus.Success
            : BTStatus.Failure;
    }
}