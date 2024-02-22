using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private void OnCollisionEnter(Collision c)
    {
        Debug.Log($"Sword hit {c.gameObject}");
    }

    private void OnTriggerEnter(Collider c)
    {
        Debug.Log($"Sword hit {c.gameObject}");
    }
}
