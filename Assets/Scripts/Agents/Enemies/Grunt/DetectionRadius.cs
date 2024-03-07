using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRadius : MonoBehaviour
{
    [SerializeField] private Agent Parent { get; set; }

    public void Start()
    {
        Parent = GetComponentInParent<Agent>();
    }

    public void OnTriggerEnter(Collider other)
    {
        Parent.OnTriggerEnter(other);
    }
}