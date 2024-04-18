using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            StartCoroutine(ShowHit(other, 0.25f));
        }
    }

    private IEnumerator ShowHit(Collider other, float delay)
    {
        MeshRenderer rend = other.GetComponent<MeshRenderer>();
        Color initialColor = rend.material.color;
        rend.material.color = Color.red;
        yield return new WaitForSeconds(delay);
        rend.material.color = initialColor;
    }
}
