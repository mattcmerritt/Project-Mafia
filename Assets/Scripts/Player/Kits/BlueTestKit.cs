using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueTestKit : PlayerKit
{
    // reminder that vfxGradient exists (inherited from PlayerKit)

    // also, a reference to the PlayerMovement object exists called PlayerMovement - assume it is configured  
    // a reference to the Animator on the player also exists in PlayerAnimator

    [SerializeField] private float PlayerRange;

    [SerializeField] private GameObject HitMarkerPrefab;

    // a function for use on the character select UI buttons
    public void CopyToNewGameObject(PlayerControls destination)
    {
        // make component
        BlueTestKit copy = destination.gameObject.AddComponent<BlueTestKit>() as BlueTestKit;

        // copy stuff from PlayerKit
        copy.vfxGradient = vfxGradient;
        // PlayerKit.Start should handle the rest

        // copy stuff from BlueTestKit
        copy.PlayerRange = PlayerRange;
        copy.HitMarkerPrefab = HitMarkerPrefab;

        // link copy
        destination.SetCharacterKit(copy);
    }

    #region Setup
    public override void OnFieldSetup() 
    {
        PlayerMovement.SetTrailGradient(vfxGradient);
    }
    public override void OffFieldSetup() 
    {
        Debug.Log("blue now off field");
    }
    #endregion Setup

    #region Abilities
    #region Melee
    public override void MeleeAttack() 
    {
        if(!PlayerMovement.MeleeAnimationLock)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Targetable Surface"));
            PlayerMovement.gameObject.transform.LookAt(new Vector3(hit.point.x, PlayerMovement.gameObject.transform.position.y, hit.point.z));
            PlayerMovement.MeleeAnimationLock = true;
            PlayerAnimator.Play("Swing1");
        }
        else
        {
            Debug.LogWarning("Too fast!");
        }
    }
    #endregion Melee

    #region Ranged
    public override void RangedAttack(Vector3 target)
    {
        Physics.Raycast(transform.position, (target - transform.position).normalized, out RaycastHit hit, PlayerRange, ~LayerMask.GetMask("Player", "Ignore Raycast", "Pathfinding"));
        // Debug Markers for ranged attack hit detection
        GameObject hitMarker = Instantiate(HitMarkerPrefab);
        hitMarker.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        // hitMarker.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        // hitMarker.AddComponent<MeshRenderer>();

        // tracer
        hitMarker.GetComponent<Tracer>().SetUp(transform.position, new Vector3(hit.point.x, transform.position.y, hit.point.z), vfxGradient);

        // collision detection
        // TODO: find a better way of doing this.
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
    #endregion Ranged

    #region Block
    public override void Block() 
    {
        Debug.Log("implementation for blue player block pending");
    }
    #endregion Block

    #region OnField
    public override void OnFieldAbilityOne(Vector3 target)
    {
        Debug.Log("implementation for blue player on-field ability 1 pending");
    }
    #endregion Onfield

    #region OffField
    public override void OffFieldAbilityOne(Vector3 target)
    {
        Debug.Log("implementation for blue player off-field ability 1 pending");
    }

    public override void OffFieldAbilityTwo(Vector3 target)
    {
        Debug.Log("implementation for blue player off-field ability 2 pending");
    }
    #endregion OffField
    #endregion Abilities
}