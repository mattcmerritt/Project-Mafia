using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public enum PlayerState
{
    OnField,
    OffField
}

public class PlayerControls : MonoBehaviour
{
    // [SerializeField] private InputActionMap OnFieldActions, OffFieldActions;
    [SerializeField] private InputActionAsset inputs;

    private InputActionMap onFieldActionMap, offFieldActionMap, sharedMap;

    // private Action<InputAction.CallbackContext> OnMeleeAttack, OnRangedAttack, OnBlock, OnSwitch;

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
                sharedMap = map;
            }
        }

        // locate all actions and create corresponding callbacks
        foreach (InputAction action in onFieldActionMap.actions)
        {
            if(action.name == "Move")
            {
                // TODO: hook onto an event
            }
            else if(action.name == "MeleeAttack")
            {
                // TODO: hook onto an event
            }
        }

        foreach (InputAction action in offFieldActionMap.actions)
        {
            if(action.name == "RangedAttack")
            {
                // TODO: hook onto an event
            }
            else if(action.name == "Block")
            {
                // TODO: hook onto an event
            }
        }

        foreach (InputAction action in sharedMap.actions)
        {
            if(action.name == "Switch")
            {
                // TODO: hook onto an event
            }
        }
        
    }

    public void HandleMovement(InputAction.CallbackContext context)
    {

    }

    public void HandleMeleeAttack(InputAction.CallbackContext context)
    {
        
    }

    public void HandleRangedAttack(InputAction.CallbackContext context)
    {
        
    }

    public void HandleBlock(InputAction.CallbackContext context)
    {
        
    }

    public void HandleSwitch(InputAction.CallbackContext context)
    {
        
    }

    // public void Testing(InputAction.CallbackContext context)
    // {
    //     // Debug.Log(context.action.name);
    //     // if(context.action.name == "Move")
    //     // {
    //     //     Debug.Log($"Movement value: {context.action.ReadValue<Vector2>()}");
    //     // }

    //     // if(playerInput.currentActionMap.name == "On")
    //     // {
    //     //     playerInput.SwitchCurrentActionMap("Off");
    //     // }
    //     // else if(playerInput.currentActionMap.name == "Off")
    //     // {
    //     //     playerInput.SwitchCurrentActionMap("On");
    //     // }
    // }

    // public void OnMove()
    // {
    //     Debug.Log("move");
    // }
    // public void OnMeleeAttack()
    // {
    //     Debug.Log("melee");
    // }
    // public void OnRangedAttack()
    // {
    //     Debug.Log("ranged");
    // }
    // public void OnBlock()
    // {
    //     Debug.Log("block");
    // }
    // public void OnSwitch()
    // {
    //     if(playerInput.currentActionMap.name == "On")
    //     {
    //         playerInput.SwitchCurrentActionMap("Off");
    //     }
    //     else if(playerInput.currentActionMap.name == "Off")
    //     {
    //         playerInput.SwitchCurrentActionMap("On");
    //     }
    //     else
    //     {
    //         Debug.LogError("Player is using an unknown action map! Change failed!");
    //     }
    // }
}
