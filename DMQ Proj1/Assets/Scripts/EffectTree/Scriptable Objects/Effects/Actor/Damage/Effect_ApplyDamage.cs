using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Applies damage to target actor, if one exists
    /// </summary>
    [CreateAssetMenu(fileName = "AD_", menuName = "Effect Tree/Actor/Apply Damage", order = 2)]
    public class Effect_ApplyDamage : Effect_Base
    {
        public AP2_DamageMessagePreset DamageMessage;

        public override bool Invoke(ref EffectContext ctx)
        {
            Actor actor = ctx.AttackData._TargetGameObject.GetComponent<Actor>();

            if(actor != null && DamageMessage != null && base.Invoke(ref ctx))
            {
                Actor_DamageMessage msg = new Actor_DamageMessage();

                //supply preset info
                msg._DamageInfo = DamageMessage;

                //supply state info
                if(ctx.AttackData._Owner != null) msg._Caster = ctx.AttackData._Owner.gameObject;
                if (ctx.AttackData._InitialGameObject != null) msg._DamageSource = ctx.AttackData._InitialGameObject;
                if (ctx.AttackData._Team != null) msg._Team = ctx.AttackData._Team;

                actor.Stats.ApplyDamage(msg);

                return true;
            }
            return false;
        }
    }

}