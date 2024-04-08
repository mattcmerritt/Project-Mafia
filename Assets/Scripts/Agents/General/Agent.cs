using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public abstract class Agent : NetworkBehaviour
{
    public NavMeshAgent NavAgent { get; private set; }

    // State management
    public List<AgentState> availableStates; 
    public AgentState activeState;

    #region State Management
    [Command(requiresAuthority = false)]
    public void ServerChangeState(int index)
    {
        ClientChangeState(index);
    }

    [ClientRpc]
    public void ClientChangeState(int index)
    {
        ChangeState(availableStates[index]);
    }

    private void ChangeState(AgentState newState)
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

        // configure the agent's state set
        ActivateAgent();

        // starting the initial state of the agent
        if (availableStates.Count != 0)
        {
            activeState = availableStates[0];
            ChangeState(activeState);
        }
    }

    // Equivalent to Start, does not do anything special at this time
    public override void OnStartClient()
    {
        NavAgent = GetComponent<NavMeshAgent>();

        // configure the agent's state set
        ActivateAgent();

        // starting the initial state of the agent
        if (availableStates.Count != 0)
        {
            activeState = availableStates[0];
            ChangeState(activeState);
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
}