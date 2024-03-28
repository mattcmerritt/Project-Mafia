using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Mirror;

public enum PlayerState
{
    NotConfigured,
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
    [SerializeField] private GameObject PlayerCharacter;
    [SerializeField, SyncVar] private PlayerState CurrentPlayerState;

    // identification information
    [SyncVar] public string networkName;
    // player functionality
    [SerializeField] private PlayerKit selectedPlayerKit;

    [SerializeField] private GameObject characterSelectUI;

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

        // reveal player select UI if the player is the local player
        if (isLocalPlayer) 
        {
            characterSelectUI.SetActive(true);
        }
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

    #region Character Select
    public void SetCharacterKit(PlayerKit kit)
    {
        selectedPlayerKit = kit;

        if (CurrentPlayerState == PlayerState.OnField)
        {
            SetAsOnFieldPlayer();
        }
        else if (CurrentPlayerState == PlayerState.OffField)
        {
            SetAsOffFieldPlayer();
        }
    }
    #endregion Character Select

    #region Manager
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
    #endregion Manager

    #region Action Maps
    [Command(requiresAuthority = false)]
    public void MarkAsSpecificState(int stateIndex)
    {
        CurrentPlayerState = (PlayerState) stateIndex;
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

    public PlayerState GetCurrentPlayerState()
    {
        return CurrentPlayerState;
    }

    public void SwitchCurrentPlayer()
    {
        if (CurrentPlayerState == PlayerState.OnField)
        {
            SetAsOffFieldPlayer();
            selectedPlayerKit.OffFieldSetup();
        }
        else if (CurrentPlayerState == PlayerState.OffField)
        {
            SetAsOnFieldPlayer();
            selectedPlayerKit.OnFieldSetup();
        }
    }
    #endregion Action Maps

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
        PlayerCharacter.GetComponent<PlayerMovement>().Move(input);
    }
    #endregion Movement

    #region Melee Attack
    [Client]
    public void LocalHandleMeleeAttack()
    {
        Vector3 target = PlayerCharacter.GetComponent<PlayerMovement>().UseCursorPositionAsTarget();
        CommandHandleMeleeAttack(target);
    }

    [Command]
    public void CommandHandleMeleeAttack(Vector3 target)
    {
        ClientHandleMeleeAttack(target);
    }

    [ClientRpc]
    public void ClientHandleMeleeAttack(Vector3 target)
    {
        selectedPlayerKit.MeleeAttack(target);
    }
    #endregion Melee Attack

    #region Ranged Attack
    [Client]
    public void LocalHandleRangedAttack()
    {
        Vector3 target = PlayerCharacter.GetComponent<PlayerMovement>().UseCursorPositionAsTarget();
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
        selectedPlayerKit.RangedAttack(target);
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
        selectedPlayerKit.Block();
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
