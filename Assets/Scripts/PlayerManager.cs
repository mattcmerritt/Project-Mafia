using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private List<PlayerControls> playerControls;

    // Player swapping information
    public readonly SyncHashSet<string> requestedSwap = new SyncHashSet<string>();
    
    // Singleton reference
    public static PlayerManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void AddPlayerControls(PlayerControls player)
    {
        playerControls.Add(player);
    }

    // Server verification of player swapping
    // TODO: determine what the authority problem is here, and if it is possible to avoid expanding the access
    [Command(requiresAuthority = false)]
    public void IssueSwitchRequest(GameObject player)
    {
        Debug.Log($"<color=blue>Player Manager:</color> Player {player.name} has requested a swap.");
        requestedSwap.Add(player.name);

        // if both players are in the set, perform the swap
        if (requestedSwap.Count == 2)
        {
            SwitchBothPlayers();
            requestedSwap.Clear();
        }
    }

    [ClientRpc]
    public void SwitchBothPlayers()
    {
        foreach (PlayerControls player in playerControls)
        {
            player.SwitchCurrentPlayer();
        }
    }

    public bool WaitingForSwap()
    {
        return requestedSwap.Count > 0;
    }

    // ideally this method is not called without checking WaitingForSwap() first
    public string GetWaitingPlayer()
    {
        foreach (string playerName in requestedSwap)
        {
            return playerName;
        }
        return null; // theoretically never runs
    }
}
