using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float PlayerSpeed;
    private Vector3 MovementDirection;
    
    public void SetMovementDirection(Vector3 direction)
    {
        MovementDirection = direction;
    }

    public bool CheckIfDirectionSet()
    {
        return !(MovementDirection == Vector3.zero);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO: collision checking
        transform.LookAt(transform.position + MovementDirection);
        transform.position += MovementDirection * Time.fixedDeltaTime * PlayerSpeed;
    }
}
