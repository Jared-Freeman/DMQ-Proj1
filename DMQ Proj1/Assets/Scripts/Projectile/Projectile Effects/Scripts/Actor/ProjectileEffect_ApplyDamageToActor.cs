using System.Collections;
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
        base.PerformPayloadEffect(Projectile, Col);
        if (Col != null)
        {
            //Check to see if target is an actor
            ActorStats Stats = Col.GetComponent<ActorStats>();

            if (Stats != null)
            {
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
