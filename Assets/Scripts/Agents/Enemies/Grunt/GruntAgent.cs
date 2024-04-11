using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAgent : Agent
{
    [SerializeField] private float startingHealth;

    protected override void ActivateAgent()
    {
        base.ActivateAgent();
        health = startingHealth;
    }
}
