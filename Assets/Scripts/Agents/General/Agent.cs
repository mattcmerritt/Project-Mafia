using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using System;

public abstract class Agent : NetworkBehaviour
{
    public NavMeshAgent NavAgent { get; private set; }
    public Animator Animator { get; private set; }

    // State management
    public List<AgentState> availableStates; 
    public AgentState activeState;

    // Agent stats
    public float health { get; set; }

    #region State Management
    public void ChangeState<StateType>()
    {
        int stateIndex = availableStates.FindIndex((AgentState state) => state is StateType);

        // only allow state changes to new states
        //  this can help with some cases where multiple collisions occur
        //  this should not be used as a solution to poorly managing collision detection
        if (availableStates[stateIndex] != activeState)
        {
            Debug.Log($"Changing to {typeof(StateType)}, found at index {stateIndex}");
            ServerChangeState(stateIndex);
        }
        else
        {
            Debug.LogWarning($"Failed to change to {typeof(StateType)} (already in state).");
        }
    }

    [Command(requiresAuthority = false)]
    public void ServerChangeState(int index)
    {
        ClientChangeState(index);
    }

    [ClientRpc]
    public void ClientChangeState(int index)
    {
        LocalChangeState(availableStates[index]);
    }

    [Client]
    private void LocalChangeState(AgentState newState)
    {
        if (activeState != null)
        {
            activeState.DeactivateState(this);
        }

        if (newState != null)
        {
            newState.ActivateState(this);
        }

        activeState = newState;
    }
    #endregion State Management

    #region Behaviour Delegation
    // Equivalent to Start, does not do anything special at this time
    public override void OnStartServer()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        // configure the agent's state set
        ActivateAgent();

        // starting the initial state of the agent
        if (availableStates.Count != 0)
        {
            activeState = availableStates[0];
            LocalChangeState(activeState);
        }
    }

    // Equivalent to Start, does not do anything special at this time
    public override void OnStartClient()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        // configure the agent's state set
        ActivateAgent();

        // starting the initial state of the agent
        if (availableStates.Count != 0)
        {
            activeState = availableStates[0];
            LocalChangeState(activeState);
        }
    }

    protected virtual void ActivateAgent()
    {
        // not implemented at abstract level
    }

    // Delegates update action to the current state
    protected virtual void Update()
    {
        if (activeState != null)
        {
            activeState.UpdateBehavior(this);
        }
    }

    // Delegates the damage taking to the state
    //  important for cases where an agent is invincible
    public virtual void TakeDamage(float damage)
    {
        if (activeState != null)
        {
            activeState.TakeDamage(this, damage);
        }
    }
    #endregion Behaviour Delegation

    // Delegates the gizmos to the state
    public virtual void OnDrawGizmos()
    {
        if (activeState != null)
        {
            activeState.DrawGizmos(this);
        }
    }
}