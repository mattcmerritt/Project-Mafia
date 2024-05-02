using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BlueTestKit : PlayerKit
{
    // reminder that vfxGradient exists (inherited from PlayerKit)

    // also, a reference to the PlayerMovement object exists called PlayerMovement - assume it is configured  
    // a reference to the Animator on the player also exists in PlayerAnimator

    [SerializeField] private float PlayerRange;

    [SerializeField] private GameObject HitMarkerPrefab;

    // state tracking information
    private Coroutine MeleeCoroutine, RangedCoroutine;
    private bool RangedAttackReady = true;

    // a function for use on the character select UI buttons
    [Command(requiresAuthority = false)]
    public void CopyToNewGameObject(PlayerControls destination)
    {
        CopyToNewGameObjectForClients(destination);
    }

    [ClientRpc]
    public void CopyToNewGameObjectForClients(PlayerControls destination)
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
        // Debug.Log("yellow now on field");

        // TODO: reimplement to avoid null reference
        // if(GetComponent<NetworkBehaviour>().isLocalPlayer)
        // {
        //     // try an entrance attack if charge is sufficient
        //     PlayerControls pc = GetComponent<PlayerControls>();
        //     if(pc.CheckCharge(20f))
        //     {
        //         pc.CmdExpendCharge(20f);
        //         CmdBlueOnFieldAttack();
        //     }
        // }
    }

    [Command(requiresAuthority = false)]
    public void CmdBlueOnFieldAttack()
    {
        RpcYellowOnFieldAttack();
    }

    [ClientRpc]
    public void RpcYellowOnFieldAttack()
    {
        Debug.Log("blue player took the field with an attack");
    }

    public override void OffFieldSetup() 
    {
        // Debug.Log("blue now off field");
    }
    #endregion Setup

    #region Abilities
    #region Melee
    public override void MeleeAttack(Vector3 target) 
    {
        if(!PlayerMovement.MeleeAnimationLock)
        {
            Physics.Raycast(transform.position, (target - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Targetable Surface"));
            PlayerMovement.gameObject.transform.LookAt(new Vector3(hit.point.x, PlayerMovement.gameObject.transform.position.y, hit.point.z));
            PlayerMovement.MeleeAnimationLock = true;
            PlayerAnimator.SetBool("SwingInput", true);

            if (MeleeCoroutine == null)
            {
                MeleeCoroutine = StartCoroutine(MeleeCooldown());
            }
        }
        else
        {
            Debug.LogWarning("Combo continued!");
            PlayerAnimator.SetBool("SwingInput", true);
        }
    }

    // after a second of an animation plays, allow player to move again
    private IEnumerator MeleeCooldown()
    {
        bool shouldSwing = PlayerAnimator.GetBool("SwingInput");
        PlayerAnimator.SetBool("Swinging", shouldSwing);
        PlayerAnimator.SetBool("SwingInput", false);

        if (shouldSwing)
        {
            // enable hitbox
            SwordHitbox sword = FindObjectOfType<SwordHitbox>();
            sword.gameObject.GetComponent<Collider>().enabled = true;

            yield return new WaitForSeconds(0.35f); // TODO: improve to be more animation based
            PlayerAnimator.SetBool("Swinging", false);
            PlayerMovement.MeleeAnimationLock = false;
            sword.gameObject.GetComponent<Collider>().enabled = false;

            MeleeCoroutine = StartCoroutine(MeleeCooldown());
        }
        else
        {
            MeleeCoroutine = null;
        }
    }
    #endregion Melee

    #region Ranged
    public override void RangedAttack(Vector3 target)
    {
        if(RangedAttackReady)
        {
            Physics.Raycast(transform.position + Vector3.up, (target - transform.position).normalized, out RaycastHit hit, PlayerRange, ~LayerMask.GetMask("Player", "Ignore Raycast", "Pathfinding"));
            
            // if an enemy is hit, center shot
            // Debug.Log(hit.collider.gameObject.layer);
            // if(hit.collider.gameObject.layer == LayerMask.GetMask("Enemy"))
            // {
            //     GameObject hitMarker = Instantiate(HitMarkerPrefab);
            //     hitMarker.transform.position = new Vector3(hit.collider.gameObject.transform.position.x, transform.position.y, hit.collider.gameObject.transform.position.z);
            // }
            // else
            // {
                // Debug Markers for ranged attack hit detection
                GameObject hitMarker = Instantiate(HitMarkerPrefab);
                hitMarker.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                // hitMarker.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
                // hitMarker.AddComponent<MeshRenderer>();

                // tracer
                hitMarker.GetComponent<Tracer>().SetUp(transform.position, new Vector3(hit.point.x, transform.position.y, hit.point.z), vfxGradient);
            // }

            // collision detection
            // TODO: if we choose to not use hitscan, then this should be handled by a projectile script like the sword hitbox
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Agent>() != null)
            {
                // TODO: determine damage from player and stats
                hit.collider.gameObject.GetComponent<Agent>().TakeDamage(1);
            }

            // cooldown
            RangedCoroutine = StartCoroutine(RangedCooldown());
        }
        else
        {
            Debug.LogWarning("Ranged attack still on cooldown!");
        }
    }

    private IEnumerator RangedCooldown()
    {
        RangedAttackReady = false;
        yield return new WaitForSeconds(0.5f); // TODO: fine tune number
        RangedAttackReady = true;
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
