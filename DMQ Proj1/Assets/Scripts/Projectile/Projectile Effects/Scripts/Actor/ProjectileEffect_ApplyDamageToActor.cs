﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileEffect", menuName = "ScriptableObjects/ProjectileEffect/Actor/Apply Damage to Actor", order = 1)]
public class ProjectileEffect_ApplyDamageToActor : ProjectileEffect
{
    [System.Serializable]
    public struct ActorApplyDamageOptions
    {
        public float DamageAmount;

    };
    public ActorApplyDamageOptions Options;

    public override void PerformPayloadEffect(GenericProjectile Projectile, Collider Col = null)
    {
        if(Col != null)
        {
            //Check to see if target is an actor
            if (Col.GetComponent<ActorStats>() != null)
            {
                ActorStats Stats = Col.GetComponent<ActorStats>();
                //Send Damage message to actor
                DamageMessage Message = new DamageMessage
                {
                    amount = Options.DamageAmount,
                    damageSource = Projectile.transform.position,
                    direction = (Col.gameObject.transform.position - Projectile.transform.position).normalized
                };

                Stats.ApplyDamage(Message);
            }
        }
    }
}
