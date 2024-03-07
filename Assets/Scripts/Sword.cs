using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // private void OnCollisionEnter(Collision c)
    // {
    //     Debug.Log($"Sword collided with {c.gameObject}");
    // }

    private void OnTriggerEnter(Collider c)
    {
        Debug.Log($"Sword triggered {c.gameObject}");
    }
}
