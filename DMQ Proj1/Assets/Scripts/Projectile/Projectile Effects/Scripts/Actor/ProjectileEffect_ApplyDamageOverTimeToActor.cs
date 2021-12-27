using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Consider a less expensive way to compute this (a Coroutine with OnDOTEnter() OnDOTExit() and lower frequency???)
[CreateAssetMenu(fileName = "ProjectileEffect", menuName = "ScriptableObjects/ProjectileEffect/Actor/Apply Damage Over Time to Actor (Fixed DT)", order = 1)]
public class ProjectileEffect_ApplyDamageOverTimeToActor : ProjectileEffect
{
    [System.Serializable]
    public struct ActorApplyDamageOptions
    {
        public float DamagePerSecond;

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
                    amount = Options.DamagePerSecond * Time.fixedDeltaTime,
                    damageSource = Projectile.transform.position,
                    direction = (Col.gameObject.transform.position - Projectile.transform.position).normalized,
                    FLAG_IgnoreInvulnerability = true
                };

                Stats.ApplyDamage(Message);
            }
        }

    }
}
