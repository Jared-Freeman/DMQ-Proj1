using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Apply Knockback", order = 1)]
    public class Effect_ApplyKnockback : Effect_Base
    {
        public float BaseForce;

        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                if (ctx.AttackData._TargetGameObject) //If there is a target set
                {
                    float ActualForce = BaseForce;
                    Rigidbody otherRb = ctx.AttackData._TargetGameObject.GetComponent<Rigidbody>();
                    if (otherRb)
                    {
                        if(ctx.AttackData._TargetGameObject.GetComponent<Actor>())
                        {
                            ActualForce *= 1.5f; //This way the force actually pushes other actors around but doesn't obliterate cubes lol
                        }    
                        Vector3 attackerDirection = ctx.AttackData._Owner.gameObject.transform.forward;
                        otherRb.AddForce(attackerDirection * ActualForce); 
                    }
                }
                return true;
            }
            return false;
        }
    }
}

