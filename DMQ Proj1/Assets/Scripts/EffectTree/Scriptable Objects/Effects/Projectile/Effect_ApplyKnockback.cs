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
                Rigidbody rb = ctx.AttackData._TargetGameObject.GetComponent<Rigidbody>();
                if(rb)
                {
                    rb.AddForce(-rb.transform.forward * Force); //Backwards force?
                }
                return true;
            }
            return false;
        }
    }
}

