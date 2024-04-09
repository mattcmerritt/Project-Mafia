using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float PlayerSpeed;

    private Vector3 MovementDirection;
    private Animator PlayerAnimator;
    private CharacterController CharController;

    // necessary for handling sword trails and swings
    [SerializeField] private GameObject SwordTrailPrefab;
    [SerializeField] private GameObject SwordObject;
    private GameObject CurrentTrail;
    public bool MeleeAnimationLock;

    [SyncVar(hook = nameof(SetTrailGradient))]
    [SerializeField] public Gradient SwordTrailGradientToUse;

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
        if(CurrentTrail == null)
        {
            CurrentTrail = Instantiate(SwordTrailPrefab, SwordObject.transform);
            CurrentTrail.GetComponent<TrailRenderer>().colorGradient = SwordTrailGradientToUse;
        }
        else
        {
            DestroySwordTrail();
            CurrentTrail = Instantiate(SwordTrailPrefab, SwordObject.transform);
            CurrentTrail.GetComponent<TrailRenderer>().colorGradient = SwordTrailGradientToUse;
        }
    }

    public void DestroySwordTrail()
    {
        Destroy(CurrentTrail);
    }

    public void SetTrailGradient(Gradient oldGradient, Gradient newGradient)
    {
        Debug.Log("setting gradient");
        SwordTrailGradientToUse = newGradient;
    }
    #endregion Sword

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
