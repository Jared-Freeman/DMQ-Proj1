using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree.Condition
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_PC_", menuName = "Effect Tree/Conditions/Collision Force", order = 2)]
    public class E_C_CollisionForce : E_Condition_Base
    {
        /// <summary>
        /// Controls how we interpret force data from this collision
        /// </summary>
        public E_C_CollisionForceOptions CollisionInterpreterStyle = E_C_CollisionForceOptions.UseFirstContactPoint;
        /// <summary>
        /// We compare the sqr magnitude of contact force to this amount^2
        /// </summary>
        public float ForceAmount;


        public enum E_C_CollisionForceOptions { UseFirstContactPoint, UseAverageImpulse, UseHighestImpulse, UseLowestImpulse };


        public override bool EvaluateCondition(ref EffectContext ctx)
        {
            if(base.EvaluateCondition(ref ctx))
            {
                if(E_C_CollisionExists.CollisionExists(ref ctx))
                {


                    return true;
                }
            }
            return false;
        }
    }
}