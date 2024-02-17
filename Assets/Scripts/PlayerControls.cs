using System;
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

    private InputActionMap onFieldActionMap, offFieldActionMap, sharedActionMap;

    private Action<Vector2> OnMove;
    private Action OnMeleeAttack, OnRangedAttack, OnBlock, OnSwitch;

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
        OnMove += HandleMovement;
        OnMeleeAttack += HandleMeleeAttack;
        OnRangedAttack += HandleRangedAttack;
        OnBlock += HandleBlock;
        OnSwitch += HandleSwitch;

        // enable correct action maps
        EnableOnFieldMap();
        EnableSharedMap();
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

    private void FixedUpdate()
    {
        // handle live movement
        if(onFieldActionMap.enabled)
        {
            foreach (InputAction action in onFieldActionMap.actions)
            {
                if(action.name == "Move")
                {
                    if(action.IsPressed())
                    {
                        OnMove?.Invoke(action.ReadValue<Vector2>());
                    }
                }
            }
        }
    }

    public void HandleMovement(Vector2 input)
    {
        Debug.Log($"Movement value: {input}");
    }

    public void HandleMeleeAttack()
    {
        Debug.Log($"Melee");
    }

    public void HandleRangedAttack()
    {
        Debug.Log($"Ranged");
    }

    public void HandleBlock()
    {
        Debug.Log($"Block");
    }

    public void HandleSwitch()
    {
        Debug.Log($"Switch");
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
