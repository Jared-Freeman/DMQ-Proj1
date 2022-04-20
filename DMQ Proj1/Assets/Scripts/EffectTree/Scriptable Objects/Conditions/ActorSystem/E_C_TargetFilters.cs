using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree.Condition
{
    /// <summary>
    /// Checks that a TargetFilter is satisfied
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_TF_", menuName = "Effect Tree/Conditions/Target Filters", order = 2)]
    public class E_C_TargetFilters : E_Condition_Base
    {
        public Utils.TargetFilterOptions TargetFilters = new Utils.TargetFilterOptions();

        public EffectContext.TargetOptions TargetToCheck = EffectContext.TargetOptions._TargetGameObject;

        public override bool EvaluateCondition(ref EffectContext ctx)
        {
            if (base.EvaluateCondition(ref ctx) && ctx.AttackData._Team != null)
            {
                GameObject go = ctx.RetrieveGameObject(TargetToCheck);
                if(go != null)
                {
                    Actor targetActor = go.GetComponent<Actor>();

                    if (targetActor != null && TargetFilters.TargetIsAllowed(ctx.AttackData._Team, targetActor))
                    {
                        return true;
                    } 
                }
            }
            return false;
        }
    }

}