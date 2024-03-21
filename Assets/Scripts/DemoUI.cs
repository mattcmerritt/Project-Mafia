using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoUI : MonoBehaviour
{
    public static DemoUI Instance { get; private set; }
    private PlayerControls currentPlayer;
    [SerializeField] private TMP_Text positionLabel;

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
        else
        {
            positionLabel.text = "Position: Off Field";
        }
    }
}
