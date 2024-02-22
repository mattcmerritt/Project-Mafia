using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private List<PlayerControls> playerControls;

    [SerializeField, Range(0, 180)] private float swapTimer;
    
    // Singleton reference
    public static PlayerManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;

        if (isServer)
        {
            StartCoroutine(WaitForTwoPlayers());
        }
    }

    public void AddPlayerControls(PlayerControls player)
    {
        playerControls.Add(player);

        // TODO: replace with a position selection screen to allow players to chose
        if (playerControls.Count == 1)
        {
            StartCoroutine(player.WaitForInputMap(true));
        }
        else if (playerControls.Count == 2)
        {
            StartCoroutine(player.WaitForInputMap(false));
        }
    }

    [Command]
    public void SwitchBothPlayers()
    {
        foreach (PlayerControls player in playerControls)
        {
            player.SwitchCurrentPlayer();
        }
    }

    private IEnumerator WaitForTwoPlayers()
    {
        Debug.Log("Waiting for players");
        yield return new WaitUntil(() => playerControls.Count == 2);
        Debug.Log("Got two players!");
        StartCoroutine(WaitForSwapTimer());
    }

    private IEnumerator WaitForSwapTimer()
    {
        float currentTimer = 0f;
        while (currentTimer < swapTimer)
        {
            currentTimer += Time.deltaTime;
            TimerUI.Instance.UpdateTimerText($"{Mathf.CeilToInt(currentTimer)} seconds");
            yield return null;
        }

        SwitchBothPlayers();
        StartCoroutine(WaitForSwapTimer());
    }
}
