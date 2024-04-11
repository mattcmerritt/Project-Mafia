using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntHurtState : AgentState
{
    [SerializeField, Range(0, 10)] private float invincibiltyDuration;
    private Coroutine runningCoroutine;

    public override void ActivateState(Agent agent)
    {
        runningCoroutine = StartCoroutine(WaitForInvincibility(agent));

        if (agent.health <= 0)
        {
            EnemyManager.Instance.RemoveEnemy(agent);
        }
    }

    public override void DeactivateState(Agent agent)
    {
        // realistically this should never have to be stopped here
        //  done as a safety measure in case too many state change calls are made
        //  if utilized, it is likely that the initial mesh color will be lost
        StopCoroutine(runningCoroutine); 
    }

    public override void TakeDamage(Agent agent, float damage)
    {
        // cannot be hit while in hitstun
    }

    public override void UpdateBehavior(Agent agent)
    {
        // TODO: implement some hurt animation or run away
    }

    private IEnumerator WaitForInvincibility(Agent agent)
    {
        MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
        Color initialColor = meshRenderer.material.color;
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(invincibiltyDuration);
        meshRenderer.material.color = initialColor;
        agent.ChangeState<GruntIdleState>();
    }
}
