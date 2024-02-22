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
    }

    public void SwitchBothPlayers()
    {
        foreach (PlayerControls player in playerControls)
        {
            player.SwitchCurrentPlayer();
        }
    }
}
