using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.Weapons;
using EffectTree;
public class AnimatorProxy : MonoBehaviour
{
    public Animator animator { get; set; }
    public Actor actor { get; set; }
    public GameObject[] weapons;
    public Transform[] handpositions = new Transform[2];
    public GameObject equippedWeapon;
    public weaponLocalTransform[] weaponLocalTransforms;
    [System.Serializable]
    public struct weaponLocalTransform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        actor = GetComponent<Actor>();


        if (!Utils.Testing.ReferenceIsValid(animator)) Destroy(this);
        if (!Utils.Testing.ReferenceIsValid(actor)) Destroy(this);


        animator.fireEvents = false;
    }

    protected virtual void Start()
    {
        EventSubscribe();
    }

    protected virtual void Update()
    {
        if(animator == null) { Debug.LogError("???"); return; }

        if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("lastStrike"))
        {
            animator.ResetTrigger("AttackTrigger");
        }
    }

    protected virtual void OnDestroy()
    {
        EventUnsubscribe();
    }

    protected virtual void EventSubscribe()
    {
        //Movement updates
        PlayerMovementV3.OnVelocityUpdate += PlayerMovementV3_OnVelocityUpdate;
        PlayerMovementV3.OnDash += PlayerMovementV3_OnDash;
        //attack inputs
        Item_Weapon_ClassSpecific.OnAbilityBasicAttackInvoked += HandleBasicAttack;
        //Weapon change
        Inventory_Player.OnWeaponChanged += Inventory_Player_OnWeaponChanged;
        //Ability inputs

    }
    protected virtual void EventUnsubscribe()
    {
        PlayerMovementV3.OnVelocityUpdate -= PlayerMovementV3_OnVelocityUpdate;
        PlayerMovementV3.OnDash -= PlayerMovementV3_OnDash;
        Inventory_Player.OnWeaponChanged -= Inventory_Player_OnWeaponChanged;

    }

    #region Event Handlers

    private void HandleBasicAttack(object sender, ItemSystem.Weapons.Item_Weapon_ClassSpecific_EventArgs e)
    {
        if(e._Weapon.CurrentPlayer == actor)
        {
            animator.SetTrigger("AttackTrigger");
        }
    }

    private void Inventory_Player_OnWeaponChanged(object sender, InventoryEventArgs e)
    {
        GameObject g = e.obj;
        Actor a = g.GetComponent<Actor>();
        if (a == actor)
        {
            if (equippedWeapon)
                Destroy(equippedWeapon);
            int i = e.weaponSlot;
            int animIndex = e.weapon.BaseWeaponPreset.BaseWeaponOptions.AnimatorIndex;
            animator.SetTrigger("WeaponChangeTrigger");
            animator.SetInteger("Weapon", animIndex);
            equippedWeapon = Instantiate(weapons[animIndex], handpositions[0].position, handpositions[0].rotation);
            equippedWeapon.transform.parent = handpositions[0]; //For now, all weapons go in right hand.
            equippedWeapon.transform.localRotation = Quaternion.Euler(weaponLocalTransforms[animIndex].rotation);
            equippedWeapon.transform.localPosition = weaponLocalTransforms[animIndex].position;
            equippedWeapon.transform.localScale = weaponLocalTransforms[animIndex].scale;
        }
    }

 

    private void PlayerMovementV3_OnDash(object sender, PlayerMovementEventArgs e)
    {
        if(e.actor == actor)
        {
            animator.SetTrigger("SpecialAction"); //Doesn't work for some reason?
           
        }
    }

    private void PlayerMovementV3_OnVelocityUpdate(object sender, PlayerMovementEventArgs e)
    {
        if(e.actor == actor)
        {
            float velocity = e.velocity;
            animator.SetFloat("Velocity", velocity);
            if (velocity > 0.1f)
            {
                animator.SetBool("Moving", true);
            }
            else
            {
                animator.SetBool("Moving", false);
            }
        }
    }

    #endregion

}
