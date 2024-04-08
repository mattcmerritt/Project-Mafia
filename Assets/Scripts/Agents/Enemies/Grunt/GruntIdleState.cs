using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntIdleState : AgentState
{
    public override void ActivateState(Agent agent)
    {
        // nothing special
    }

    public override void DeactivateState(Agent agent)
    {
        // nothing special
    }

    public override void TakeDamage(Agent agent, float damage)
    {
        // nothing special
    }

    public override void UpdateBehavior(Agent agent)
    {
        Collider[] hitObjects = Physics.OverlapSphere(agent.transform.position, 5);
        if (hitObjects.Length > 0)
        {
            Debug.Log($"Agent: Collision with {agent.activeState}");
        } 
    }
}
