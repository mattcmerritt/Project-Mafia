using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private List<PlayerControls> playerControls;

    // Player swapping information
    private HashSet<string> requestedSwap;
    
    // Singleton reference
    public static PlayerManager Instance { get; private set; }

    private void Start()
    {
        requestedSwap = new HashSet<string>();

        Instance = this;
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
}
