using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Physics/[Experimental] Apply Radial Force", order = 1)]
    public class Effect_ApplyRadialForce : Effect_Base
    {
        public float BaseForce;
        public float radius;
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
                        if (ctx.AttackData._TargetGameObject.GetComponent<Actor>())
                        {
                            ActualForce = 1.5f * BaseForce; //This way the force actually pushes other actors around but doesn't obliterate cubes lol
                        }
                        Vector3 attackerDirection = ctx.AttackData._InitialPosition + otherRb.gameObject.transform.position;
                        otherRb.AddExplosionForce(ActualForce, ctx.AttackData._InitialPosition, radius);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
