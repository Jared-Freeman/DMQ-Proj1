using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem.StatusEffect;

namespace EffectTree
{
    /// <summary>
    /// Removes Status Effect(s) from target actor by type, if an actor exists.
    /// </summary>
    [CreateAssetMenu(fileName = "RSE_", menuName = "Effect Tree/Actor/Remove Status Effect", order = 2)]
    public class Effect_RemoveStatusEffect : Effect_Base
    {
        public TargetFilterOptions TargetFilters = new TargetFilterOptions();
        public List<E_RSE_StatusConfig> ListStatusEffects = new List<E_RSE_StatusConfig>();

        public override bool Invoke(ref EffectContext ctx)
        {
            Actor actor = ctx.AttackData._TargetGameObject.GetComponent<Actor>();

            if(actor != null && TargetFilters.TargetIsAllowed(ctx.AttackData._Team, actor) && base.Invoke(ref ctx)) 
            {
                foreach(E_RSE_StatusConfig s in ListStatusEffects)
                {
                    if (s.StatusEffect != null) actor.Stats.RemoveStatusEffect(s.StatusEffect, s.MaxCount, s.RemoveHighestRemainingDurationFirst);
                }

                return true;
            }
            return false;
        }

        public class E_RSE_StatusConfig
        {
            public SE_StatusEffect_Base StatusEffect;
            public int MaxCount = 1;
            public bool RemoveHighestRemainingDurationFirst = true;
        }
    }

}