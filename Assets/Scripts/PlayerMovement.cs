using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float PlayerSpeed, WalkAccel, WalkDecel;
    private float AnimSpeed;

    private Vector3 MovementDirection;
    private CharacterController CharController;
    [SerializeField] private Animator ModelAnimator;

    // necessary for handling sword trails and swings
    [SerializeField] private GameObject SwordTrailPrefab;
    [SerializeField] private GameObject SwordObject;
    private GameObject CurrentTrail;
    public bool MeleeAnimationLock;

    [ClientRpc]
    public void Move(Vector2 input)
    {
        // when loading into an existing lobby, will try to run move before Start
        // if this happens, run Start first to set up necessary references
        if(CharController == null)
        {
            Start();
        }
        Vector3 direction = new Vector3(input.x, 0, input.y);
        CharController.Move(direction * PlayerSpeed * Time.deltaTime);

        if (direction.magnitude > 0)
        {
            ModelAnimator.SetBool("Running", true);
            ModelAnimator.SetFloat("Speed", AnimSpeed);
            AnimSpeed = AnimSpeed <= 1f ? AnimSpeed += Time.deltaTime * WalkAccel : 1;
        }
        else
        {
            ModelAnimator.SetBool("Running", false);
            ModelAnimator.SetFloat("Speed", AnimSpeed);
            AnimSpeed = AnimSpeed >= 0f ? AnimSpeed -= Time.deltaTime * WalkDecel : 0;
        }

        // turning
        if(!MeleeAnimationLock && direction != Vector3.zero)
        {
            transform.LookAt(transform.position + direction);
        }
        
    }

    // this simply finds the pointer position - the functions themselves need to decide how to use this
    // for example, if this is used for a hitscan attack, the other function should check for walls between player and the point returned by this
    public Vector3 UseCursorPositionAsTarget()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Targetable Surface"));
        // Debug.Log($"clickpos: {hit.point}");
        return hit.point;
    }

    #region Sword
    public void InstantiateSwordTrail()
    {
        Gradient VFXGradientToUse = null;
        PlayerControls[] AllPlayerControls = FindObjectsOfType<PlayerControls>();
        Debug.Log("player count: " + AllPlayerControls.Length);
        foreach (PlayerControls pc in AllPlayerControls)
        {
            if (pc.GetCurrentPlayerState() == PlayerState.OnField)
            {
                VFXGradientToUse = pc.GetCharacterKit().GetVFXGradient();
            }
        }

        if(CurrentTrail == null)
        {
            CurrentTrail = Instantiate(SwordTrailPrefab, SwordObject.transform);
        }
        else
        {
            DestroySwordTrail();
            CurrentTrail = Instantiate(SwordTrailPrefab, SwordObject.transform);
        }

        if(VFXGradientToUse != null)
        {
            CurrentTrail.GetComponent<TrailRenderer>().colorGradient = VFXGradientToUse;
        }
    }

    public void DestroySwordTrail()
    {
        Destroy(CurrentTrail);
    }
    #endregion Sword

    public void Start()
    {
        CharController = GetComponent<CharacterController>();
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
