using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public abstract class Agent : NetworkBehaviour
{
    public NavMeshAgent NavAgent { get; private set; }
    [SyncVar] public AgentState activeState;

    #region State Management
    [Command]
    public virtual void ChangeState(AgentState newState)
    {
        Debug.Log($"Changing state from {activeState.stateName} to {newState.stateName}");

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

    // when attempting to set initial state, it will fail on start claiming the following:
    //  Command System.Void Agent::ChangeState(AgentState) called on <Agent> while NetworkClient is not ready.
    // to avoid this, a coroutine must be used to hold off on calling the command until the client is ready
    // more details: https://forum.unity.com/threads/spawnwithclientauthority-networkconnection-is-not-ready.523990/
    public IEnumerator ChangeStateWhenReady(AgentState newState)
    {
        // TODO: cannot use connectionToClient on non-player network behaviours - determine alternative
        yield return new WaitUntil(() => connectionToClient != null && connectionToClient.isReady);
        ChangeState(newState);
    }

    #endregion State Management

    #region Behaviour Delegation
    // Equivalent to Start, does not do anything special at this time
    public override void OnStartServer()
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
    public virtual void OnTriggerEnter(Collider other)
    {
        if (activeState != null)
        {
            activeState.TriggerActiveBehavior(this, other);
        }
    }

    // Delegates trigger response action to the current state
    // Necessary for if state while a collision is occurring
    public virtual void OnTriggerStay(Collider other)
    {
        if (activeState != null)
        {
            activeState.TriggerActiveBehavior(this, other);
        }
    }

    // Delegates trigger exit response action to the current state
    public virtual void OnTriggerExit(Collider other)
    {
        if (activeState != null)
        {
            activeState.TriggerExitBehavior(this, other);
        }
    }
    #endregion Behaviour Delegation
}