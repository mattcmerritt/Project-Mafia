using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grunt
{
    public class ChaseState : AgentState
    {
        private GameObject player;
        private Coroutine chaseCoroutine, waitToReachTargetCoroutine;
        private Vector3 target;
        private bool isChasingTarget;

        public ChaseState(GameObject player)
        {
            this.player = player;
        }

        public override void ActivateState(Agent agent)
        {
            target = player.transform.position;
            chaseCoroutine = agent.StartCoroutine(ChaseToTarget(agent));
        }

        // stop the coroutines related to this state
        public override void DeactivateState(Agent agent)
        {
            if (chaseCoroutine != null)
            {
                agent.StopCoroutine(chaseCoroutine);
            }

            if (waitToReachTargetCoroutine != null)
            {
                agent.StopCoroutine(waitToReachTargetCoroutine);
            }
        }

        // whenever the player is seen, update the target location to where the player was seen
        public override void TriggerActiveBehavior(Agent agent, Collider other)
        {
            if (other.gameObject.GetComponent<PlayerManager>() != null)
            {
                target = other.transform.position;
            }
        }

        // if the player is not seen, start a coroutine to switch the state as soon as the target is reached
        public override void TriggerExitBehavior(Agent agent, Collider other)
        {
            if (other.gameObject.GetComponent<PlayerManager>() != null)
            {
                Coroutine oldCoroutine = waitToReachTargetCoroutine;
                waitToReachTargetCoroutine = agent.StartCoroutine(WaitToReachTarget(agent));
                if (oldCoroutine != null)
                {
                    agent.StopCoroutine(oldCoroutine);
                }
            }
        }

        // update the target for the nav agent
        public override void UpdateBehavior(Agent agent)
        {
            agent.NavAgent.SetDestination(target);
        }

        // coroutine to wait until the agent reaches the target
        private IEnumerator ChaseToTarget(Agent agent)
        {
            isChasingTarget = true;
            yield return new WaitUntil(() => Vector3.Magnitude(agent.transform.position - target) < 0.1f);
            isChasingTarget = false;
        }

        // co
        private IEnumerator WaitToReachTarget(Agent agent)
        {
            yield return new WaitUntil(() => !isChasingTarget);
            agent.ActiveState = new IdleState();
        }
    }
}