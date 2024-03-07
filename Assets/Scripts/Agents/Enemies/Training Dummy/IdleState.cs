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
            // NOTE:
            //  if this behavior check should always apply to the agent, then it should be in the agent
            //  this is located here for now to demonstrate functionality

            // starting a hit indicator coroutine
            ((TrainingDummy)agent).ActiveCoroutine = agent.StartCoroutine(((TrainingDummy)agent).ShowHit());   
        }

        public override void TriggerExitBehavior(Agent agent, Collider other)
        {
            // Should not do anything
        }
    }
}