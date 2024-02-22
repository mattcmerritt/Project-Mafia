using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<PlayerControls> playerControls;
    
    // Singleton reference
    public static PlayerManager Instance { get; private set; }

    private void Start()
    {
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

    public void SwitchBothPlayers()
    {
        foreach (PlayerControls player in playerControls)
        {
            player.SwitchCurrentPlayer();
        }
    }
}
