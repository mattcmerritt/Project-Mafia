using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float PlayerSpeed;
    private Vector3 MovementDirection;
    public bool MeleeAnimationLock; // used to prevent swing cancels, public for animators
    private Animator PlayerAnimator;
    
    public void SetMovementDirection(Vector3 direction)
    {
        MovementDirection = direction;
    }

    public bool CheckIfDirectionSet()
    {
        return !(MovementDirection == Vector3.zero);
    }

    public void TryMeleeAttack()
    {
        if(!MeleeAnimationLock)
        {
            MeleeAnimationLock = true;
            PlayerAnimator.Play("Swing1");
        }
        else
        {
            Debug.LogWarning("Too fast!");
        }
    }

    public void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        MeleeAnimationLock = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO: collision checking
        transform.LookAt(transform.position + MovementDirection);
        transform.position += MovementDirection * Time.fixedDeltaTime * PlayerSpeed;
    }
}
