using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // [SerializeField] private InputActionMap OnFieldActions, OffFieldActions;
    [SerializeField] private PlayerInput playerInput;

    public void OnMove()
    {
        Debug.Log("move");
    }
    public void OnMeleeAttack()
    {
        Debug.Log("melee");
    }
    public void OnRangedAttack()
    {
        Debug.Log("ranged");
    }
    public void OnBlock()
    {
        Debug.Log("block");
    }
    public void OnSwitch()
    {
        if(playerInput.currentActionMap.name == "On")
        {
            playerInput.SwitchCurrentActionMap("Off");
        }
        else if(playerInput.currentActionMap.name == "Off")
        {
            playerInput.SwitchCurrentActionMap("On");
        }
        else
        {
            Debug.LogError("Player is using an unknown action map! Change failed!");
        }
    }
}
