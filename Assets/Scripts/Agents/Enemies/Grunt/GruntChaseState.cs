using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntChaseState : AgentState
{
    // state specific information
    [SerializeField, Range(0, 10)] private float detectionRadius;
    [SerializeField, Range(0, 10)] private float chaseRadius;
    [SerializeField, Range(0, 10)] private float attackRadius;

    // necessary for preventing multiple collisions causing multiple state changes
    private bool stateChangeActivated;

    public override void ActivateState(Agent agent)
    {
        // indicate that nothing has caused a change yet
        stateChangeActivated = false;

        Collider[] detectedObjects = Physics.OverlapSphere(agent.transform.position, detectionRadius);
        foreach (Collider detectedObject in detectedObjects)
        {
            if (detectedObject.GetComponent<PlayerMovement>())
            {
                agent.NavAgent.SetDestination(detectedObject.transform.position);
            }
        }

        // prepare sword
        agent.Animator.ResetTrigger("SheathSword");
        agent.Animator.SetTrigger("DrawSword");
    }

    public override void DeactivateState(Agent agent)
    {
        // clean up side effects of using state
        stateChangeActivated = false;
        agent.NavAgent.SetDestination(agent.transform.position);

        // put away weapon
        agent.Animator.ResetTrigger("DrawSword");
        agent.Animator.SetTrigger("SheathSword");
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
        // detecting if the enemy is still close enough to the player to keep chasing
        Collider[] detectedObjects = Physics.OverlapSphere(agent.transform.position, chaseRadius);
        bool playerFound = false;
        foreach (Collider detectedObject in detectedObjects)
        {
            if (detectedObject.GetComponent<PlayerMovement>())
            {
                playerFound = true;
                agent.NavAgent.SetDestination(detectedObject.transform.position);
            }
        }

        if (!playerFound && !stateChangeActivated)
        {
            stateChangeActivated = true;
            agent.ChangeState<GruntIdleState>();
        }

        // detecting if the player is close enough to be attacked
        Collider[] attackRangeObjects = Physics.OverlapSphere(agent.transform.position, attackRadius);
        foreach (Collider detectedObject in attackRangeObjects)
        {
            if (detectedObject.GetComponent<PlayerMovement>() && !stateChangeActivated)
            {
                stateChangeActivated = true;
                agent.ChangeState<GruntAttackState>();
            }
        }
    }

    // DEBUG: draw detection radii
    public override void DrawGizmos(Agent agent)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
