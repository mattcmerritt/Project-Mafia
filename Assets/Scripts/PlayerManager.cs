using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<PlayerControls> playerControls;
    
    // Singleton reference
    public static PlayerManager Instance { get; private set; }

    [SerializeField] public bool UseTimer;
    [SerializeField] private float SwapInterval, TimeUntilNextSwap;

    private void Start()
    {
        Instance = this;
    }

    // TODO: remove after A/B testing
    private void Update()
    {
        if (UseTimer)
        {
            if (TimeUntilNextSwap < 0f)
            {
                Debug.Log("Time interval hit, swapping players!");
                SwitchBothPlayers();
                TimeUntilNextSwap = SwapInterval;
            }
            else
            {
                TimeUntilNextSwap -= Time.deltaTime;

                int minutes = Mathf.Clamp(((int)TimeUntilNextSwap / 60), 0, ((int)TimeUntilNextSwap / 60));
                int seconds = Mathf.Clamp(((int)TimeUntilNextSwap % 60), 0, ((int)TimeUntilNextSwap % 60));

                DemoUI.Instance.UpdateTimerText(string.Format("Time Remaining: {0:0}:{1:00}", minutes, seconds));
            }
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

    public void SwitchBothPlayers()
    {
        foreach (PlayerControls player in playerControls)
        {
            player.SwitchCurrentPlayer();
        }
    }
}
