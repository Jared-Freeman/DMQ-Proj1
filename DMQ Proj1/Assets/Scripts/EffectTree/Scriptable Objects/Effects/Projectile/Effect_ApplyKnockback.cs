using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Apply Knockback", order = 1)]
    public class Effect_ApplyKnockback : Effect_Base
    {
        public float Force;

        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                if (ctx.AttackData._TargetGameObject)
                {
                    Rigidbody otherRb = ctx.AttackData._TargetGameObject.GetComponent<Rigidbody>();
                    if (otherRb)
                    {
                        Vector3 attackerDirection = ctx.AttackData._Owner.gameObject.transform.forward;
                        otherRb.AddForce(attackerDirection * Force); 
                    }
                }
                return true;
            }
            return false;
        }
    }
}

