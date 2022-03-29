using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree.Condition
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_PC_", menuName = "Effect Tree/Conditions/Collision Force", order = 2)]
    public class E_C_CollisionExists : E_Condition_Base
    {
        public E_C_ColExistsOptions AllowedCollisions = E_C_ColExistsOptions.CollisionsAllowed;

        public enum E_C_ColExistsOptions { TriggersAllowed, CollisionsAllowed, TriggersAndCollisionAllowed }

        /// <summary>
        /// Public-facing version of this Condition eval. 
        /// </summary>
        /// <remarks>
        /// Useful for a few other conditions to reuse this logic. See <see cref="E_C_CollisionForce"/>.
        /// </remarks>
        public static bool CollisionExists(ref EffectContext ctx)
        {
            if (ctx.ContextData._TriggeringCollision != null) return true;

            return false;
        }
        /// <summary>
        /// Public-facing version of this Condition eval. 
        /// </summary>
        /// <remarks>
        /// Useful for a few other conditions to reuse this logic.
        /// </remarks>
        public static bool TriggerExists(ref EffectContext ctx)
        {
            if (ctx.ContextData._TriggeringCollider != null && ctx.ContextData._TriggeringCollision == null) return true;

            return false;
        }

        public override bool EvaluateCondition(ref EffectContext ctx)
        {
            if( base.EvaluateCondition(ref ctx))
            {
                switch(AllowedCollisions)
                {

                    case E_C_ColExistsOptions.CollisionsAllowed:
                        if (CollisionExists(ref ctx)) return true;
                        break;

                    case E_C_ColExistsOptions.TriggersAllowed:
                        if (TriggerExists(ref ctx)) return true;
                        break;

                    case E_C_ColExistsOptions.TriggersAndCollisionAllowed:
                        if (CollisionExists(ref ctx) || TriggerExists(ref ctx)) return true;
                        break;

                    default:
                        Debug.LogError("No impl found for condition option!");
                        return false;   

                }

                return true;
            }
            return false;
        }

    }

}