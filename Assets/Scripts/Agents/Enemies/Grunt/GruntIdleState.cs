using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntIdleState : AgentState
{
    // state specific information
    [SerializeField, Range(0, 10)] private float detectionRadius;

    // necessary for preventing multiple collisions causing multiple state changes
    private bool stateChangeActivated;

    public override void ActivateState(Agent agent)
    {
        // indicate that nothing has caused a change yet
        stateChangeActivated = false;
    }

    public override void DeactivateState(Agent agent)
    {
        // clean up side effects of using state
        stateChangeActivated = false;
    }

    public override void TakeDamage(Agent agent, float damage)
    {
        // do not allow multiple collision detections to cause multiple damage instances on same frame
        //  this happens when the state change occurs slower than the collisions are detected, resulting in more hits
        //  to fix, we just need to wait for the change to occur
        if (stateChangeActivated) return;

        Debug.Log($"Agent {agent.name} took damage!");
        stateChangeActivated = true;
        agent.health -= damage;
        agent.ChangeState<GruntHurtState>();
        
    }

    public override void UpdateBehavior(Agent agent)
    {
        Collider[] detectedObjects = Physics.OverlapSphere(agent.transform.position, detectionRadius);
        foreach (Collider detectedObject in detectedObjects)
        {
            Debug.Log($"Agent {agent.name} detected with {detectedObject.name}");
        }

        // TODO: this should not be handled by the agents, but by the player
        //  the player should tell the agents that they were hurt (like with ranged attacks)
        //  this has the agent tell the player that they hit them, and registers too many collisions
        CapsuleCollider agentCollider = agent.GetComponent<CapsuleCollider>();
        Collider[] hitObjects = Physics.OverlapCapsule(agent.transform.position + Vector3.down * agentCollider.height / 2, agent.transform.position + Vector3.up * agentCollider.height / 2, agentCollider.radius);
        foreach (Collider hitObject in hitObjects)
        {
            // Debug.Log($"Agent {agent.name} collided with {hitObject.name}");
            if (hitObject.GetComponent<SwordHitbox>())
            {
                agent.TakeDamage(1); // TODO: read this damage from the player
            }
        }
    }
}
