using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float PlayerSpeed;
    private Vector3 MovementDirection;
    public bool MeleeAnimationLock; // used to prevent swing cancels, public for animators
    private Animator PlayerAnimator;
    private CharacterController CharController;

    [ClientRpc]
    public void Move(Vector2 input)
    {
        CharController.Move(new Vector3(input.x, 0, input.y) * PlayerSpeed * Time.deltaTime);
    }

    // public void SetMovementDirection(Vector3 direction)
    // {
    //     MovementDirection = direction;
    // }

    // public bool CheckIfDirectionSet()
    // {
    //     return !(MovementDirection == Vector3.zero);
    // }

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

    public void TryRangedAttack()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.point);
        }
    }

    public void Start()
    {
        CharController = GetComponent<CharacterController>();
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
