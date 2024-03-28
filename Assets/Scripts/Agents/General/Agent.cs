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
    [Command(requiresAuthority = false)]
    public virtual void ChangeState(AgentState newState)
    {
        // Debug.Log($"Changing state from {activeState.stateName} to {newState.stateName}");

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
        //yield return new WaitUntil(() => connectionToClient != null && connectionToClient.isReady);

        // TODO: determine if there is a better way to handle the start logic
        yield return new WaitForEndOfFrame();
        ChangeState(newState);
    }

    #endregion State Management

    #region Behaviour Delegation
    // Equivalent to Start, does not do anything special at this time
    public override void OnStartServer()
    {
        NavAgent = GetComponent<NavMeshAgent>();
    }

    // Equivalent to Start, does not do anything special at this time
    public override void OnStartClient()
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
            // TODO: Something is going wrong with the polymorphism used here.

            // activeState is recognized to be not null, and it is even set up with a state name, which should only be 
            //  possible if activeState stores a reference to a specialized class (like Grunt.IdleState). Thus, I am
            //  concluding that activeState is storing the specialized class object, not a generic AgentState object.

            // however, when calling any of the methods, only the virtual implementation is called, even if an
            //  override exists for the method. For example, Grunt.IdleState implements TriggerActiveBehavior, and
            //  in this implementation, no calls to base.TriggerActiveBehavior are made.

            Debug.Log($"<color=blue>State:</color> Agent class triggered state ({activeState.stateName}) for {gameObject.name}!");
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