using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grunt
{
    public class IdleState : AgentState
    {
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
            // if triggered by a player entering it's radius, chase the player
            agent.ActiveState = new ChaseState(other.gameObject);
        }

        public override void TriggerExitBehavior(Agent agent, Collider other)
        {
            // Should not do anything
        }
    }
}