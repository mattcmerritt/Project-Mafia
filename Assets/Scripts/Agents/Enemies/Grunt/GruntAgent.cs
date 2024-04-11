using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAgent : Agent
{
    protected override void ActivateAgent()
    {
        base.ActivateAgent();

        health = 5;
    }
}
