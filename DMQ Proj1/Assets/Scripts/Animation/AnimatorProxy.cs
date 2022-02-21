using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.Weapons;
using EffectTree;
public class AnimatorProxy : MonoBehaviour
{
    public Animator animator;
    public Actor actor;
    public AllPossibleWeapons weapons;
    public Transform[] handpositions = new Transform[2];
    public GameObject weapon;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        if (!animator)
            Debug.Log(gameObject.name + " has no animator");
        actor = gameObject.GetComponent<Actor>();
        if (!animator)
            Debug.Log(gameObject.name + " has no actor");


        animator.fireEvents = false;
    }

    void Start()
    {
        EventSubscribe();

    }
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("lastStrike"))
        {
            animator.ResetTrigger("AttackTrigger");
        }
    }
    void EventSubscribe()
    {
        //Movement updates
        PlayerMovementV3.OnVelocityUpdate += PlayerMovementV3_OnVelocityUpdate;
        PlayerMovementV3.OnDash += PlayerMovementV3_OnDash;
        //attack inputs
        Effect_Dev_MeleeAttack.OnMeleeAttack += Effect_Dev_MeleeAttack_OnMeleeAttack;
        //Weapon change
        Inventory_Player.OnWeaponChanged += Inventory_Player_OnWeaponChanged;
        //Ability inputs
        
    }

    private void Effect_Dev_MeleeAttack_OnMeleeAttack(object sender, MeleeAttackEventArgs e)
    {
        if(e.actor == actor)
        {
            animator.SetTrigger("AttackTrigger");
        }
    }

    private void Inventory_Player_OnWeaponChanged(object sender, InventoryEventArgs e)
    {
        GameObject g = e.obj;
        Actor a = g.GetComponent<Actor>();
        int i = e.weaponSlot;
        if (a == actor)
        {
            animator.SetTrigger("WeaponChangeTrigger");
            animator.SetInteger("Weapon", i);
            GameObject w = Instantiate(weapons.weaponList[i].model[i], handpositions[i].position, handpositions[i].rotation);
            w.transform.parent = handpositions[i];
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

    void EventUnsubscribe()
    {
        PlayerMovementV3.OnVelocityUpdate -= PlayerMovementV3_OnVelocityUpdate;
        PlayerMovementV3.OnDash -= PlayerMovementV3_OnDash;
        Inventory_Player.OnWeaponChanged -= Inventory_Player_OnWeaponChanged;

    }
}
