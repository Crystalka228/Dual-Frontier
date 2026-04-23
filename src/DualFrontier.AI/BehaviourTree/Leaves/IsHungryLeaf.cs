namespace DualFrontier.AI.BehaviourTree.Leaves;

using DualFrontier.Components.Pawn;
using DualFrontier.AI.BehaviourTree;
using System;

/// <summary>
/// Condition leaf: returns Success if pawn is hungry, Failure otherwise.
/// </summary>
public sealed class IsHungryLeaf : Leaf
{
    public override BTStatus Tick(BTContext ctx)
    {
        // Read hunger from blackboard key "Hunger" (float).
        // Set by NeedsSystem before BT tick.
        var hunger = ctx.Blackboard.Get<float>("Hunger");
        return hunger >= NeedsComponent.CriticalThreshold
            ? BTStatus.Success
            : BTStatus.Failure;
    }
}