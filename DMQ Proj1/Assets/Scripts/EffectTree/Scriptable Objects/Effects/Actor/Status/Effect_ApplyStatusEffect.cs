using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ActorSystem.StatusEffect;

namespace EffectTree
{
    /// <summary>
    /// Applies status effect(s) to target actor, if one exists
    /// </summary>
    [CreateAssetMenu(fileName = "ASE_", menuName = "Effect Tree/Actor/Apply Status Effect", order = 2)]
    public class Effect_ApplyStatusEffect : Effect_Base
    {
        public TargetFilterOptions TargetFilters = new TargetFilterOptions();
        /// <summary>
        /// List of statuses to apply to target
        /// </summary>
        public List<SE_StatusEffect_Base> List_StatusEffects;

        public override bool Invoke(ref EffectContext ctx)
        {
            Actor actor = ctx.AttackData._TargetGameObject.GetComponent<Actor>();

            //this branch includes a short circuit eval
            if(actor != null && TargetFilters.TargetIsAllowed(ctx.AttackData._Team, actor) && base.Invoke(ref ctx))
            {
                foreach(SE_StatusEffect_Base fx in List_StatusEffects)
                {
                    actor.Stats.AddStatusEffect(fx.CreateInstance(actor.gameObject));
                }

                return true;
            }
            return false;
        }
    }

}