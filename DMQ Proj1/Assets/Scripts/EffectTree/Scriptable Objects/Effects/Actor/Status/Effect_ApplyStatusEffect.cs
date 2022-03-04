using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
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

        public SE_StatusEffect_Base.DurationApplicationBehavior ApplyDurationBehavior = SE_StatusEffect_Base.DurationApplicationBehavior.ResetDuration;

        public EffectContext.TargetOptions Target = EffectContext.TargetOptions._TargetGameObject;

        [Header("Won't exceed Preset's max stacks")]
        public int Stacks = 1;

        public override bool Invoke(ref EffectContext ctx)
        {
            GameObject g = ctx.RetrieveGameObject(Target);
            if (g == null) return false;

            Actor actor = g.GetComponent<Actor>();

            //this branch includes a short circuit eval
            if(actor != null && TargetFilters.TargetIsAllowed(ctx.AttackData._Team, actor) && base.Invoke(ref ctx))
            {
                foreach(SE_StatusEffect_Base fx in List_StatusEffects)
                {
                    var inst = fx.CreateInstance(actor.gameObject);
                    inst.AddStacks(Stacks);
                    actor.Stats.AddStatusEffect(inst);

                }

                return true;
            }
            return false;
        }
    }

}