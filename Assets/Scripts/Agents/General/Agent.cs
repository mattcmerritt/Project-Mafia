using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Agent : MonoBehaviour
{
    public NavMeshAgent NavAgent { get; private set; }

    private AgentState activeState;
    public AgentState ActiveState 
    { 
        get
        {
            return activeState;
        }
        set 
        {
            if (activeState != null)
            {
                activeState.DeactivateState(this);
            }

            if (value != null)
            {
                value.ActivateState(this);
            }

            activeState = value;
        }
    }

    // Start does not do anything special at this time
    protected virtual void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
    }

    // Delegates update action to the current state
    protected virtual void Update()
    {
        if (activeState != null)
        {
            activeState.UpdateBehavior(this);
        }
    }

    // Delegates trigger response action to the current state
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (activeState != null)
        {
            activeState.TriggerActiveBehavior(this, other);
        }
    }

    // Delegates trigger response action to the current state
    // Necessary for if state while a collision is occurring
    protected virtual void OnTriggerStay(Collider other)
    {
        if (activeState != null)
        {
            activeState.TriggerActiveBehavior(this, other);
        }
    }

    // Delegates trigger exit response action to the current state
    protected virtual void OnTriggerExit(Collider other)
    {
        if (activeState != null)
        {
            activeState.TriggerExitBehavior(this, other);
        }
    }
}