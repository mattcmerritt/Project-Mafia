using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentState
{
    // Setup for states, only called once
    public abstract void ActivateState(Agent agent);
    public abstract void DeactivateState(Agent agent);

    // Behaviors, will be implemented in subclasses and called repeatedly
    public abstract void UpdateBehavior(Agent agent);
    public abstract void TriggerActiveBehavior(Agent agent, Collider other);
    public abstract void TriggerExitBehavior(Agent agent, Collider other);
}
