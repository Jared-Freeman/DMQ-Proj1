using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree.Condition
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_PC_", menuName = "Effect Tree/Conditions/Collision Force", order = 5)]
    public class E_C_CollisionForce : E_Condition_Base
    {
        public override bool EvaluateCondition(ref EffectContext ctx)
        {
            if(base.EvaluateCondition(ref ctx))
            {



                return true;
            }
            return false;
        }
    }
}