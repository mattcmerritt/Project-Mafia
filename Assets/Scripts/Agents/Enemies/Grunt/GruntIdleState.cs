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

        // animator changes
        if (agent.previousState is GruntChaseState)
        {
            agent.Animator.SetTrigger("SheathSword");
        }
        else if (agent.previousState is GruntAttackState)
        {
            agent.Animator.SetTrigger("LowerSword");
            agent.Animator.SetTrigger("SheathSword");
        }
    }

    public override void DeactivateState(Agent agent)
    {
        // clean up side effects of using state
        stateChangeActivated = false;

        // clean up animator state
        if (agent.previousState is GruntChaseState)
        {
            agent.Animator.ResetTrigger("SheathSword");
        }
        else if (agent.previousState is GruntAttackState)
        {
            agent.Animator.ResetTrigger("LowerSword");
            agent.Animator.ResetTrigger("SheathSword");
        }
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
        if (!stateChangeActivated)
        {
            Collider[] detectedObjects = Physics.OverlapSphere(agent.transform.position, detectionRadius);
            foreach (Collider detectedObject in detectedObjects)
            {
                if (detectedObject.GetComponent<PlayerMovement>())
                {
                    stateChangeActivated = true;
                    agent.ChangeState<GruntChaseState>();
                }
            }
        }
    }

    // DEBUG: draw detection radii
    public override void DrawGizmos(Agent agent)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
