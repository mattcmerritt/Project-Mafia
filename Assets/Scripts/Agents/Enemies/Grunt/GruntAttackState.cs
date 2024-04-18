using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAttackState : AgentState
{
    // state specific information
    [SerializeField, Range(0, 10)] private float attackRadius;
    [SerializeField, Range(0, 10)] private float attackChaseRadius;
    [SerializeField, Range(0, 10)] private float windUpDelay;
    [SerializeField, Range(0, 10)] private float nextAttackDelay;
    [SerializeField, Range(0, 10)] private float invincibilityDuration;

    // temporary state information (NEEDS TO BE CLEANED UP AT DEACTIVATE)
    private GameObject player;
    private Coroutine windUpCoroutine;
    private bool isInvincible;
    private Coroutine invincibilityCoroutine;
    private Color initialColor;

    // necessary for preventing multiple collisions causing multiple state changes
    private bool stateChangeActivated;

    public override void ActivateState(Agent agent)
    {
        // indicate that nothing has caused a change yet
        stateChangeActivated = false;

        // find the player that is supposed to be attacked
        Collider[] detectedObjects = Physics.OverlapSphere(agent.transform.position, attackRadius);
        foreach (Collider detectedObject in detectedObjects)
        {
            if (detectedObject.GetComponent<PlayerMovement>())
            {
                Debug.LogWarning($"Player {player} was found!");
                player = detectedObject.gameObject;
                agent.NavAgent.SetDestination(player.transform.position);
            }
        }

        windUpCoroutine = StartCoroutine(AttackWindUp(agent));

        // color stuff for hit detection
        MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;

        // ready the sword
        agent.Animator.ResetTrigger("LowerSword");
        agent.Animator.SetTrigger("RaiseSword");
    }

    public override void DeactivateState(Agent agent)
    {
        // clean up side effects of using state
        stateChangeActivated = false;
        agent.NavAgent.SetDestination(agent.transform.position);

        if (windUpCoroutine != null) StopCoroutine(windUpCoroutine);

        // TODO: determine a way to keep the i-frames between states
        //  it may be possible to cheese enemies by walking into attack range, hitting them, then stepping out
        //  not a top priority since the delay will be short
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            isInvincible = false;
        }

        player = null;

        // remove colors
        MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
        meshRenderer.material.color = initialColor;
        initialColor = Color.black;

        // put down the sword stance
        agent.Animator.ResetTrigger("SwingSword");
        agent.Animator.ResetTrigger("RaiseSword");
        agent.Animator.SetTrigger("LowerSword");
    }

    public override void TakeDamage(Agent agent, float damage)
    {
        // damage cannot be taken while invincible
        if (isInvincible) return; 

        // taking damage in this state will not prevent the enemy from attacking
        //  they will attempt to trade blows with the player unless the player walks away
        Debug.Log($"Agent {agent.name} took damage, but is still attacking!");
        agent.health -= damage;

        // prevent repeated hits and start hit delay
        isInvincible = true;
        invincibilityCoroutine = StartCoroutine(WaitForInvincibility(agent));

        // remove agent if they are dead
        if (agent.health <= 0)
        {
            EnemyManager.Instance.RemoveEnemy(agent);
        }
    }

    public override void UpdateBehavior(Agent agent)
    {
        // if the enemy is not in range of the player, move back to the player until in range
        float distance = Vector3.Magnitude(agent.transform.position - player.transform.position);

        // if close, stand in place
        if (distance < attackRadius)
        {
            agent.NavAgent.SetDestination(agent.transform.position);
        }
        // if close but not too far, move closer
        else if (distance < attackChaseRadius)
        {
            agent.NavAgent.SetDestination(player.transform.position);
        }
        // if nowhere close (and not already transitioning state), transition to idle state
        else if (!stateChangeActivated)
        {
            stateChangeActivated = true;
            agent.ChangeState<GruntIdleState>();
        }
    }

    // DEBUG: draw detection radii
    public override void DrawGizmos(Agent agent)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackChaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    private IEnumerator AttackWindUp(Agent agent)
    {
        yield return new WaitForSeconds(windUpDelay);
        
        // perform an attack here
        agent.Animator.SetTrigger("SwingSword");

        // continue chaining attacks if possible
        yield return new WaitForSeconds(nextAttackDelay);
        windUpCoroutine = StartCoroutine(AttackWindUp(agent));
    }

    private IEnumerator WaitForInvincibility(Agent agent)
    {
        MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
        meshRenderer.material.color = initialColor;
    }
}