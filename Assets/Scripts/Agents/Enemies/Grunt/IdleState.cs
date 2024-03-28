using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grunt
{
    public class IdleState : AgentState
    {
        public IdleState()
        {
            stateName = "Grunt Idle";
        }

        public override void ActivateState(Agent agent)
        {
            // Nothing special required
        }

        public override void DeactivateState(Agent agent)
        {
            // Nothing special required
        }

        public override void UpdateBehavior(Agent agent)
        {
            // Should not do anything
        }

        public override void TriggerActiveBehavior(Agent agent, Collider other)
        {
            Debug.Log($"<color=blue>State:</color> Trigger Active Behavior called at specialized level (from IdleState) for {agent.name}");

            // if triggered by a player entering it's radius, chase the player
            if (other.GetComponent<PlayerManager>() != null)
            {
                agent.ChangeState(new ChaseState(other.gameObject));
            }
        }

        public override void TriggerExitBehavior(Agent agent, Collider other)
        {
            // Should not do anything
        }
    }
}