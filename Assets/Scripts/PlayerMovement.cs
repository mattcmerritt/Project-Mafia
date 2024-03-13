using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float PlayerSpeed, PlayerRange;
    private Vector3 MovementDirection;
    public bool MeleeAnimationLock; // used to prevent swing cancels, public for animators
    private Animator PlayerAnimator;
    private CharacterController CharController;

    [SerializeField] private GameObject HitMarkerPrefab;
    [SerializeField] private GameObject SwordTrailPrefab;
    [SerializeField] private GameObject SwordObject;
    private GameObject CurrentTrail;

    [ClientRpc]
    public void Move(Vector2 input)
    {
        // TODO: implement a slowdown and turnlock if an attack is currently active

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
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Targetable Surface"));
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            MeleeAnimationLock = true;
            PlayerAnimator.Play("Swing1");
        }
        else
        {
            Debug.LogWarning("Too fast!");
        }
    }

    public void InstantiateSwordTrail()
    {
        if(CurrentTrail == null)
        {
            CurrentTrail = Instantiate(SwordTrailPrefab, SwordObject.transform);
        }
        else
        {
            DestroySwordTrail();
            CurrentTrail = Instantiate(SwordTrailPrefab, SwordObject.transform);
        }
    }

    public void DestroySwordTrail()
    {
        Destroy(CurrentTrail);
    }

    public Vector3 FindRangedAttackTarget()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Targetable Surface"));
        // Debug.Log($"clickpos: {hit.point}");
        return hit.point;
    }

    public void TryRangedAttack(Vector3 target)
    {
        Physics.Raycast(transform.position, (target - transform.position).normalized, out RaycastHit hit, PlayerRange, ~LayerMask.GetMask("Player", "Ignore Raycast"));
        // Debug Markers for ranged attack hit detection
        GameObject hitMarker = Instantiate(HitMarkerPrefab);
        hitMarker.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        // hitMarker.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        // hitMarker.AddComponent<MeshRenderer>();

        // tracer
        hitMarker.GetComponent<Tracer>().SetUp(transform.position, new Vector3(hit.point.x, transform.position.y, hit.point.z), Color.red);

        // collision detection
        if(hit.collider != null)
        {
            if(hit.collider.gameObject.GetComponent<TrainingDummy.TrainingDummy>() != null)
            {
                hit.collider.gameObject.GetComponent<TrainingDummy.TrainingDummy>().StartCoroutine(hit.collider.gameObject.GetComponent<TrainingDummy.TrainingDummy>().ShowHit());
            }
            if(hit.collider.gameObject.GetComponent<Grunt.Grunt>() != null)
            {
                hit.collider.gameObject.GetComponent<Grunt.Grunt>().StartCoroutine(hit.collider.gameObject.GetComponent<Grunt.Grunt>().TakeDamage(1));
            }
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
