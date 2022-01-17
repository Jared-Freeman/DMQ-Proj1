using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAction_MeleeAttack : MonoBehaviour 
{

    //For now, just apply the damage. Update later to add functionality for hit detection, animation, etc. 

    #region Members
    ActorAI_Logic_Melee Logic;
    public float DamageAmount = 3.0f;
    #endregion


    // Start is called before the first frame update
     void Start()
    {
        Logic = GetComponent<ActorAI_Logic_Melee>();
    }

    #region Methods
    public void OnActionStart()
    {

    }
    public void ActionUpdate()
    {
        
    }
    public void OnActionEnd()
    {
        Logic.Animator.SetTrigger("Attack1h1");
        ActorDamage AttackDamage = new ActorDamage(null, DamageAmount, ActorDamageType.Internal);
        Logic.TargetActor.TakeDamage(AttackDamage);
        Debug.Log(Logic.TargetActor.name + "is hit");
    }
    #endregion
}
