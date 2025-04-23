using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to Target", story: "[Agent] [Moves] to [Target]", category: "Action", id: "f96456f198bbec4772bf903fbf65f7ec")]
public partial class MoveToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<MovementComponent> Moves;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private const float StoppingDistance = 2f; // Distance threshold to stop moving

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Moves.Value == null || Target.Value == null)
        {
            return Status.Failure; // Fail if any required reference is missing
        }

        // Calculate the direction to the target
        Vector3 direction = Target.Value.transform.position - Agent.Value.transform.position;

        // Check if the agent is close enough to the target
        if (direction.sqrMagnitude <= StoppingDistance * StoppingDistance)
        {
            Moves.Value.Move(Vector3.zero); // Stop movement
            return Status.Success; // Stop moving and return success
        }

        // Move towards the target
        Moves.Value.Move(direction);

        return Status.Running; // Continue moving
    }
}

