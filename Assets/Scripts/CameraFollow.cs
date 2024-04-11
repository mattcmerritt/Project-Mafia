using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject following;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        transform.position = following.transform.position + offset;
    }
}
