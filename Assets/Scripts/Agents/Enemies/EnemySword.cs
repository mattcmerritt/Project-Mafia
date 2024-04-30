using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    private Color initialColor;

    private void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        SkinnedMeshRenderer rend = player.GetComponentInChildren<SkinnedMeshRenderer>();
        Color initialColor = rend.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.GetComponent<PlayerMovement>())
        {
            StartCoroutine(ShowHit(other, 0.25f));
            Debug.Log("Player hit!");
        }
    }

    private IEnumerator ShowHit(Collider other, float delay)
    {
        SkinnedMeshRenderer rend = other.GetComponentInChildren<SkinnedMeshRenderer>();
        rend.material.color = Color.red;
        yield return new WaitForSeconds(delay);
        rend.material.color = initialColor;
    }
}
