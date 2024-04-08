using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAgent : Agent
{
    protected override void ActivateAgent()
    {
        availableStates = new List<AgentState>() { new GruntIdleState() };
    }
}
