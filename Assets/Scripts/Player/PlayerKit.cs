using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// NOTE: this class should be treated as abstract - do NOT instantiate it, only use children

// all unique playable characters are meant to be a child of this class
// a child of this class will be attached to all "players" after character select

// any universal behavior that every character has but functions differently on a per character basis should be defined here

// for example, movement works the same for everyone and shouldn't be in here
// but a dash might be implemented differently for each character and would go in here
public class PlayerKit : NetworkBehaviour
{
    [SerializeField] protected Gradient vfxGradient; // for use in sword swing, ranged tracers, etc

    protected PlayerMovement PlayerMovement;
    protected Animator PlayerAnimator;

    // attach PlayerMovement
    public void Start() 
    {
        PlayerMovement = FindObjectOfType<PlayerMovement>();
        PlayerAnimator = PlayerMovement.gameObject.GetComponentInChildren<Animator>();
    }

    public Gradient GetVFXGradient()
    {
        return vfxGradient;
    }

    // NOTE: any ability that has the potential to be targeted or used remotely must take the target parameter

    // this is because it needs to get the on-field client's mouse position and use that for the future calls,
    // but this player may not be the on-field client, so the position business is handled in PlayerMovement.
    // if an ability is not supposed to be targeted in a child method (like a burst around the player), simply don't use the parameter.

    // functions that will be called whenever the player takes a role
    // note that OnFieldSetup and OffFieldSetup should ALWAYS call base method in child implementations if isLocalPlayer is needed
    // this is because the fields of NetworkBehavior like isLocalPlayer are null in grandchildren for some reason
    public virtual void OnFieldSetup() {}
    public virtual void OffFieldSetup() {}

    // the minimum abilities that any player will have
    public virtual void MeleeAttack(Vector3 target) {}
    public virtual void RangedAttack(Vector3 target) {}
    public virtual void Block() {} // TODO: should this be generic instead, and move back to PlayerControls?
    public virtual void OnFieldAbilityOne(Vector3 target) {}
    public virtual void OffFieldAbilityOne(Vector3 target) {}
    public virtual void OffFieldAbilityTwo(Vector3 target) {}
}
