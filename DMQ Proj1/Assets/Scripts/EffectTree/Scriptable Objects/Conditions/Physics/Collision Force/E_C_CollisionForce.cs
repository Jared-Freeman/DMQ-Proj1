using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree.Condition
{
    /// <summary>
    /// Validates that a given collision force passes the criteria supplied to this SO's options.
    /// </summary>
    /// <remarks>
    /// For example, this Condition implements: "Collision Force is Less Than 30" and "Collision Force is Equal To 10.234f."
    /// </remarks>
    [CreateAssetMenu(fileName = "Cond_PC_", menuName = "Effect Tree/Conditions/Collision Force", order = 2)]
    public class E_C_CollisionForce : E_Condition_Base
    {
        private static bool s_FLAG_DEBUG = false;

        /// <summary>
        /// We compare the sqr magnitude of contact force to this amount^2
        /// </summary>
        public float ForceAmount;

        [Header("Read as: \"The Context Force is _____ ForceAmount\"")]
        public Utils.Compare.FloatComparison EvaluationCriterion = new Utils.Compare.FloatComparison();

        public override bool EvaluateCondition(ref EffectContext ctx)
        {
            if(base.EvaluateCondition(ref ctx))
            {
                if(E_C_CollisionExists.CollisionExists(ref ctx))
                {
                    Vector3 colForce;
                    colForce = ctx.ContextData._TriggeringCollision.impulse;
                    ////Remove time variable from solution
                    //colForce /= Time.fixedDeltaTime;

#if UNITY_EDITOR
                    if (s_FLAG_DEBUG)
                    {
                        Debug.Log(colForce.magnitude);
                        Debug.DrawRay(ctx.ContextData._TriggeringCollision.GetContact(0).point, colForce, Color.white, 2f);
                    }
#endif

                    return EvaluationCriterion.Compare(colForce.sqrMagnitude, Mathf.Pow(ForceAmount, 2));
                }
            }
            return false;
        }
    }
}