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

    void Start()
    {
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
        OnMove += CommandHandleMovement;
        OnMeleeAttack += CommandHandleMeleeAttack;
        OnRangedAttack += CommandHandleRangedAttack;
        OnBlock += CommandHandleBlock;
        OnSwitch += CommandHandleSwitch;

        // player controls need to be parented for the clients
        if (transform.parent == null)
        {
            AttachToManager();
        }
        else
        {
            PlayerCharacter = transform.parent.gameObject;
        }

        // TODO: remove
        // SetAsOnFieldPlayer();
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
            if(action.name == "Move")
            {
                // NOTE: callback does not work for continuous movement, use check in FixedUpdate instead

                // TODO: remove if unnecessary
                // action.started += (InputAction.CallbackContext context) => OnMove?.Invoke(context.action.ReadValue<Vector2>());
            }
            else if(action.name == "MeleeAttack")
            {
                action.started += (InputAction.CallbackContext context) => OnMeleeAttack?.Invoke();
            }
        }
    }

    private void DisableOnFieldMap()
    {
        foreach (InputAction action in onFieldActionMap.actions)
        {
            if(action.name == "Move")
            {
                // NOTE: callback does not work for continuous movement, use check in FixedUpdate instead

                // TODO: remove if unnecessary
                // action.started -= (InputAction.CallbackContext context) => OnMove?.Invoke(context.action.ReadValue<Vector2>());
            }
            else if(action.name == "MeleeAttack")
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
        // only allow player to control their inputs
        //if (!isLocalPlayer)
        //{
        //    return;
        //}

        Debug.Log($"Local: {isLocalPlayer}");
        Debug.Log($"Maps: Onfield: {onFieldActionMap.enabled} Offfield: {offFieldActionMap.enabled}");

        // handle live movement
        if(onFieldActionMap.enabled)
        {
            Vector2 movementInput = onFieldActionMap.FindAction("Move").ReadValue<Vector2>();

            // TODO: not sure if mirror has a way to check this, or if I even need to check it at all
            // based on https://youtu.be/XhluFjFAo4E?t=477

            if(!NetworkClient.ready)
            {
                Debug.LogWarning("not ready!");
            }
            else if(isLocalPlayer)
            {
                CommandHandleMovement(movementInput);
            }

            // if(IsServer && isLocalPlayer)
            // {

            // }
            // else if (IsClient && isLocalPlayer)
            // {

            // }

            // foreach (InputAction action in onFieldActionMap.actions)
            // {
            //     if(action.name == "Move")
            //     {
            //         if(action.IsPressed())
            //         {
            //             OnMove?.Invoke(action.ReadValue<Vector2>());
            //         }
            //         // TODO: look into maybe doing this differently - feels wrong
            //         else if(PlayerCharacter.GetComponent<PlayerMovement>().CheckIfDirectionSet())
            //         {
            //             // makes the player stop
            //             // Debug.Log("stopping");
            //             CommandHandleMovement(new Vector2(0, 0));
            //         }
            //     }
            // }
        }
    }

    [Command]
    public void CommandHandleMovement(Vector2 input)
    {
        // Debug.Log($"Movement value: {input}");
        // PlayerCharacter.GetComponent<PlayerMovement>().SetMovementDirection(new Vector3(input.x, 0, input.y));
        PlayerCharacter.GetComponent<PlayerMovement>().Move(input);
    }

    [Command]
    public void CommandHandleMeleeAttack()
    {
        ClientHandleMeleeAttack();
    }

    [ClientRpc]
    public void ClientHandleMeleeAttack()
    {
        // Debug.Log($"Melee");
        PlayerCharacter.GetComponent<PlayerMovement>().TryMeleeAttack();
    }

    [Command]
    public void CommandHandleRangedAttack()
    {
        Vector3 target = PlayerCharacter.GetComponent<PlayerMovement>().FindRangedAttackTarget();
        ClientHandleRangedAttack(target);
    }

    [ClientRpc]
    public void ClientHandleRangedAttack(Vector3 target)
    {
        // Debug.Log($"Ranged");
        PlayerCharacter.GetComponent<PlayerMovement>().TryRangedAttack(target);
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

    [Command]
    public void CommandHandleSwitch()
    {
        ClientHandleSwitch();
    }

    [ClientRpc]
    public void ClientHandleSwitch()
    {
        Debug.Log($"Switch");
        PlayerManager.Instance.SwitchBothPlayers();
    }

    public void SwitchCurrentPlayer()
    {
        if(CurrentPlayerState == PlayerState.OnField)
        {
            SetAsOffFieldPlayer();   
        }
        else if(CurrentPlayerState == PlayerState.OffField)
        {
            SetAsOnFieldPlayer();
        }
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
}
