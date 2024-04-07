using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoUI : MonoBehaviour
{
    public static DemoUI Instance { get; private set; }
    [SerializeField] private PlayerControls currentPlayer;

    // text elements
    [SerializeField] private TMP_Text positionLabel;
    [SerializeField] private TMP_Text swapRequestIndicator;

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
    }

    private void Update()
    {
        if (currentPlayer == null) return;
        if (currentPlayer.GetCurrentPlayerState() == PlayerState.OnField)
        {
            positionLabel.text = "Position: On Field";
        }
        else if (currentPlayer.GetCurrentPlayerState() == PlayerState.OffField)
        {
            positionLabel.text = "Position: Off Field";
        }
        else
        {
            positionLabel.text = "Position: Not Configured";
        }

        if (PlayerManager.Instance.WaitingForSwap())
        {
            string waitingPlayerName = PlayerManager.Instance.GetWaitingPlayer();
            if (waitingPlayerName == currentPlayer.name)
            {
                swapRequestIndicator.color = Color.black;
                swapRequestIndicator.text = "Waiting for swap...";
            }
            else
            {
                swapRequestIndicator.color = Color.red;
                swapRequestIndicator.text = "Teammate wants to swap!";
            }
        }
        else
        {
            swapRequestIndicator.text = "";
        }
    }
}
