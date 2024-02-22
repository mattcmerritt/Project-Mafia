using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Text timerText;
    public static TimerUI Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void UpdateTimerText(string newText)
    {
        timerText.text = $"Swap Timer: {newText}";
    }
}
