using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainingDummy
{
    public class IdleState : AgentState
    {
        public override void ActivateState(Agent agent)
        {
            // Nothing special required
        }

        public override void DeactivateState(Agent agent)
        {
            // Stop and clear the coroutine
            agent.StopAllCoroutines();
            ((TrainingDummy)agent).ActiveCoroutine = null;
        }

        public override void UpdateBehavior(Agent agent)
        {
            // Should not do anything
        }

        public override void TriggerActiveBehavior(Agent agent, Collider other)
        {
            // starting a hit indicator coroutine
            ((TrainingDummy)agent).ActiveCoroutine = agent.StartCoroutine(((TrainingDummy)agent).ShowHit());
        }

        public override void TriggerExitBehavior(Agent agent, Collider other)
        {
            // Should not do anything
        }
    }
}