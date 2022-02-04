using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Applies damage to target actor, if one exists
    /// </summary>
    [CreateAssetMenu(fileName = "AD_", menuName = "Effect Tree/Apply Damage", order = 2)]
    public class Effect_ApplyDamage : Effect_Base
    {
        AP2_DamageMessagePreset DamageMessage;

        public override bool Invoke(ref EffectContext ctx)
        {
            Actor actor = ctx.AttackData._TargetGameObject.GetComponent<Actor>();

            if(actor != null && DamageMessage != null && base.Invoke(ref ctx))
            {
                Actor_DamageMessage msg = new Actor_DamageMessage();

                //supply preset info
                msg._DamageInfo = DamageMessage;

                //supply state info
                msg._Caster = ctx.AttackData._Owner.gameObject;
                msg._DamageSource = ctx.AttackData._InitialGameObject;
                msg._Team = ctx.AttackData._Team;

                actor.Stats.ApplyDamage(msg);

                return true;
            }
            return false;
        }
    }

}