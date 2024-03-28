using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AgentState is intended to act as an abstract class. However, in order to be serialized in a way that
//  allows Mirror to store the class in a SyncVar, we cannot use an abstract class.

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
    public virtual void TriggerActiveBehavior(Agent agent, Collider other) { Debug.Log($"<color=blue>State:</color> Trigger Active Behavior called at abstract level for {agent.name}"); }
    public virtual void TriggerExitBehavior(Agent agent, Collider other) { }
}