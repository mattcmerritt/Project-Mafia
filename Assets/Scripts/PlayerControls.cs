using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Mirror;

public enum PlayerState
{
    OnField,
    OffField
}

public class PlayerControls : NetworkBehaviour
{
    // inputs
    [SerializeField] private InputActionAsset inputs;
    private InputActionMap onFieldActionMap, offFieldActionMap, sharedActionMap;

    // events
    public Action<Vector2> OnMove;
    public Action OnMeleeAttack, OnRangedAttack, OnBlock, OnSwitch;

    // components
    private GameObject PlayerCharacter;
    [SerializeField] private PlayerState CurrentPlayerState;

    // identification information
    [SyncVar] public string networkName;

    void Start()
    {
        // configure name
        gameObject.name = networkName;

        ReadOnlyArray<InputActionMap> playerActionMaps = inputs.actionMaps;

        // locate action maps

        foreach (InputActionMap map in playerActionMaps)
        {
            if(map.name == "OnField")
            {
                onFieldActionMap = map;
            }
            else if(map.name == "OffField")
            {
                offFieldActionMap = map;
            }
            else if(map.name == "Shared")
            {
                sharedActionMap = map;
            }
        }

        // set up callbacks
        OnMove += LocalHandleMovement;
        OnMeleeAttack += LocalHandleMeleeAttack;
        OnRangedAttack += LocalHandleRangedAttack;
        OnBlock += LocalHandleBlock;
        OnSwitch += LocalHandleSwitch;

        // player controls need to be parented for the clients
        if (transform.parent == null)
        {
            AttachToManager();
        }
        else
        {
            PlayerCharacter = transform.parent.gameObject;
        }
    }

    public IEnumerator WaitForInputMap(bool onField)
    {
        yield return new WaitUntil(() => onFieldActionMap != null);
        if (onField)
        {
            SetAsOnFieldPlayer();
        }
        else
        {
            SetAsOffFieldPlayer();
        }
    }

    [Client]
    public void SetAsOnFieldPlayer()
    {
        if (!isLocalPlayer) return;
        EnableOnFieldMap();
        DisableOffFieldMap();
        EnableSharedMap();
        CurrentPlayerState = PlayerState.OnField;
    }

    [Client]
    public void SetAsOffFieldPlayer()
    {
        if (!isLocalPlayer) return;
        DisableOnFieldMap();
        EnableOffFieldMap();
        EnableSharedMap();
        CurrentPlayerState = PlayerState.OffField;
    }

    private void EnableOnFieldMap()
    {
        onFieldActionMap.Enable();
        foreach (InputAction action in onFieldActionMap.actions)
        {
            if(action.name == "MeleeAttack")
            {
                action.started += (InputAction.CallbackContext context) => OnMeleeAttack?.Invoke();
            }
        }
    }

    private void DisableOnFieldMap()
    {
        foreach (InputAction action in onFieldActionMap.actions)
        {
            if(action.name == "MeleeAttack")
            {
                action.started -= (InputAction.CallbackContext context) => OnMeleeAttack?.Invoke();
            }
        }
        onFieldActionMap.Disable();
    }

    private void EnableOffFieldMap()
    {
        offFieldActionMap.Enable();
        foreach (InputAction action in offFieldActionMap.actions)
        {
            if(action.name == "RangedAttack")
            {
                action.started += (InputAction.CallbackContext context) => OnRangedAttack?.Invoke();
            }
            else if(action.name == "Block")
            {
                action.started += (InputAction.CallbackContext context) => OnBlock?.Invoke();
            }
        }
    }

    private void DisableOffFieldMap()
    {
        foreach (InputAction action in offFieldActionMap.actions)
        {
            if(action.name == "RangedAttack")
            {
                action.started -= (InputAction.CallbackContext context) => OnRangedAttack?.Invoke();
            }
            else if(action.name == "Block")
            {
                action.started -= (InputAction.CallbackContext context) => OnBlock?.Invoke();
            }
        }
        offFieldActionMap.Disable();
    }

    private void EnableSharedMap()
    {
        sharedActionMap.Enable();
        foreach (InputAction action in sharedActionMap.actions)
        {
            if(action.name == "SwitchMode")
            {
                action.started += (InputAction.CallbackContext context) => OnSwitch?.Invoke();
            }
        }
    }

    // most likely useless but included anyways
    private void DisableSharedMap()
    {
        foreach (InputAction action in sharedActionMap.actions)
        {
            if(action.name == "SwitchMode")
            {
                action.started += (InputAction.CallbackContext context) => OnSwitch?.Invoke();
            }
        }
        sharedActionMap.Disable();
    }

    //[Client]
    private void Update()
    {
        // Debug.Log($"Local: {isLocalPlayer}");
        // Debug.Log($"Maps: Onfield: {onFieldActionMap.enabled} Offfield: {offFieldActionMap.enabled}");

        // handle live movement
        if(onFieldActionMap.enabled)
        {
            Vector2 movementInput = onFieldActionMap.FindAction("Move").ReadValue<Vector2>();

            if(!NetworkClient.ready)
            {
                Debug.LogWarning("not ready!");
            }
            else if(isLocalPlayer)
            {
                CommandHandleMovement(movementInput);
            }
        }
    }

    public void SwitchCurrentPlayer()
    {
        if (CurrentPlayerState == PlayerState.OnField)
        {
            SetAsOffFieldPlayer();
        }
        else if (CurrentPlayerState == PlayerState.OffField)
        {
            SetAsOnFieldPlayer();
        }
    }

    public PlayerState GetCurrentPlayerState()
    {
        return CurrentPlayerState;
    }

    // Needed if clients need to load a PlayerManager
    public void AttachToManager()
    {
        StartCoroutine(AddPlayerControlsToManager());
    }

    private IEnumerator AddPlayerControlsToManager()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);
        PlayerManager.Instance.AddPlayerControls(this);
        transform.parent = PlayerManager.Instance.transform;
        PlayerCharacter = transform.parent.gameObject;
    }

    #region Networked Actions
    #region Movement
    [Client]
    public void LocalHandleMovement(Vector2 input)
    {
        CommandHandleMovement(input);
    }

    [Command]
    public void CommandHandleMovement(Vector2 input)
    {
        // Debug.Log($"Movement value: {input}");
        // PlayerCharacter.GetComponent<PlayerMovement>().SetMovementDirection(new Vector3(input.x, 0, input.y));
        PlayerCharacter.GetComponent<PlayerMovement>().Move(input);
    }
    #endregion Movement

    #region Melee Attack
    [Client]
    public void LocalHandleMeleeAttack()
    {
        CommandHandleMeleeAttack();
    }

    [Command]
    public void CommandHandleMeleeAttack()
    {
        ClientHandleMeleeAttack();
    }

    [ClientRpc]
    public void ClientHandleMeleeAttack()
    {
        PlayerCharacter.GetComponent<PlayerMovement>().TryMeleeAttack();
    }
    #endregion Melee Attack

    #region Ranged Attack
    [Client]
    public void LocalHandleRangedAttack()
    {
        Vector3 target = PlayerCharacter.GetComponent<PlayerMovement>().FindRangedAttackTarget();
        CommandHandleRangedAttack(target);
    }

    [Command]
    public void CommandHandleRangedAttack(Vector3 target)
    {
        ClientHandleRangedAttack(target);
    }

    [ClientRpc]
    public void ClientHandleRangedAttack(Vector3 target)
    {
        PlayerCharacter.GetComponent<PlayerMovement>().TryRangedAttack(target);
    }
    #endregion Ranged Attack

    #region Block
    [Client]
    public void LocalHandleBlock()
    {
        CommandHandleBlock();
    }

    [Command]
    public void CommandHandleBlock()
    {
        ClientHandleBlock();
    }

    [ClientRpc]
    public void ClientHandleBlock()
    {
        Debug.Log($"Block");
    }
    #endregion Block

    #region Player Switching
    [Client]
    public void LocalHandleSwitch()
    {
        PlayerManager.Instance.IssueSwitchRequest(gameObject);
    }
    #endregion Player Switching
    #endregion Networked Actions
}
