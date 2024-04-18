using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntHurtState : AgentState
{
    [SerializeField, Range(0, 10)] private float invincibiltyDuration;
    private Coroutine runningCoroutine;
    private Color initialColor;

    public override void ActivateState(Agent agent)
    {
        MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;

        runningCoroutine = StartCoroutine(WaitForInvincibility(agent));

        if (agent.health <= 0)
        {
            EnemyManager.Instance.RemoveEnemy(agent);
        }
    }

    public override void DeactivateState(Agent agent)
    {
        // clean up state side effects
        MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
        meshRenderer.material.color = initialColor;
        initialColor = Color.black;

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

    public override void DrawGizmos(Agent agent)
    {
        // nothing to draw
    }

    private IEnumerator WaitForInvincibility(Agent agent)
    {
        MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(invincibiltyDuration);
        agent.ChangeState<GruntIdleState>();
    }
}
