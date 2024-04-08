using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AgentState is intended to act as an abstract class. However, in order to be serialized in a way that
//  allows Mirror to store the class in a SyncVar, we cannot use an abstract class.

public abstract class AgentState
{
    // Setup for states, only called once
    public abstract void ActivateState(Agent agent);
    public abstract void DeactivateState(Agent agent);

    // Behaviors, will be implemented in subclasses and called repeatedly
    public abstract void UpdateBehavior(Agent agent);
    public abstract void TakeDamage(Agent agent, float damage);
}