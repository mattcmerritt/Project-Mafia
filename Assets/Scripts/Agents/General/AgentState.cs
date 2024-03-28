using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentState
{
    // Piece of data for serialization
    public string stateName;

    // Setup for states, only called once
    public virtual void ActivateState(Agent agent) { }
    public virtual void DeactivateState(Agent agent) { }

    // Behaviors, will be implemented in subclasses and called repeatedly
    public virtual void UpdateBehavior(Agent agent) { }
    public virtual void TriggerActiveBehavior(Agent agent, Collider other) { }
    public virtual void TriggerExitBehavior(Agent agent, Collider other) { }
}