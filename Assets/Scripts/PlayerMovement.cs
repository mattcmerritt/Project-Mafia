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
        // TODO: reimplement rotation

        if(CharController != null)
        {
            CharController.Move(new Vector3(input.x, 0, input.y) * PlayerSpeed * Time.deltaTime);
        }
        // when loading into an existing lobby, will try to run move before Start
        // if this happens, run Start first to set up necessary references
        else
        {
            Start();
            CharController.Move(new Vector3(input.x, 0, input.y) * PlayerSpeed * Time.deltaTime);
        }
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

    public Vector3 FindRangedAttackTarget()
    {
        // NOTE: the 100f is the max raycast distance - without this parameter, it uses the layer mask as the max distance instead
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, LayerMask.GetMask("Ranged Raycastable"));
        Debug.Log($"hit {hit.collider}");
        return hit.point;
    }

    public void TryRangedAttack(Vector3 target)
    {
        // Debug Markers for ranged attack hit detection
        GameObject hitMarker = new GameObject("Debug: Ranged Hit Location");
        hitMarker.transform.position = target;
        hitMarker.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        hitMarker.AddComponent<MeshRenderer>();
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
