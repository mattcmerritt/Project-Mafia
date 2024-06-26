using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        // Debug.Log($"Sword triggered {c.gameObject}");

        Agent hitAgent = c.gameObject.GetComponent<Agent>();
        if (hitAgent)
        {
            // TODO: determine damage from player and stats
            Debug.Log($"Agent {hitAgent} was hit with sword.");
            hitAgent.TakeDamage(1);
        }
    }
}
