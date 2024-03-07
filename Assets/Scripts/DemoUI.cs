using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoUI : MonoBehaviour
{
    public static DemoUI Instance { get; private set; }
    private PlayerControls currentPlayer;
    [SerializeField] private TMP_Text positionLabel, timerLabel;
    [SerializeField] private PlayerState currentState;

    private void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayerStart());
    }

    private IEnumerator WaitForPlayerStart()
    {
        yield return new WaitUntil(() => FindObjectOfType<PlayerControls>() != null);
        PlayerControls[] controls = FindObjectsOfType<PlayerControls>();
        foreach (PlayerControls control in controls)
        {
            if (control.isLocalPlayer)
            {
                currentPlayer = control;
            }
        }

        /*
        // TODO: find a better way to do this for secondary clients
        yield return new WaitForSeconds(0.1f);
        currentPlayer.OnSwitch += UpdatePositionLabel;
        // set up as opposite so the coroutine immediately starts
        if (currentPlayer.GetCurrentPlayerState() == PlayerState.OnField)
        {
            currentState = PlayerState.OffField;
        }
        else
        {
            currentState = PlayerState.OnField;
        }
        StartCoroutine(WaitForRoleUpdate());
        */
    }

    /*
    private void UpdatePositionLabel()
    {
        // TODO: look into other ways to wait on the first subscriber callback
        StartCoroutine(WaitForRoleUpdate());
    }

    private IEnumerator WaitForRoleUpdate()
    {
        yield return new WaitUntil(() => currentPlayer.GetCurrentPlayerState() != currentState);
        if (currentPlayer.GetCurrentPlayerState() == PlayerState.OnField)
        {
            positionLabel.text = "Position: On Field";
            currentState = PlayerState.OnField;
        }
        else
        {
            positionLabel.text = "Position: Off Field";
            currentState = PlayerState.OffField;
        }
    }
    */

    private void Update()
    {
        if (currentPlayer == null) return;
        if (currentPlayer.GetCurrentPlayerState() == PlayerState.OnField)
        {
            positionLabel.text = "Position: On Field";
            currentState = PlayerState.OnField;
        }
        else
        {
            positionLabel.text = "Position: Off Field";
            currentState = PlayerState.OffField;
        }
    }

    public void UpdateTimerText(string newText)
    {
        timerLabel.text = newText;
    }
}
