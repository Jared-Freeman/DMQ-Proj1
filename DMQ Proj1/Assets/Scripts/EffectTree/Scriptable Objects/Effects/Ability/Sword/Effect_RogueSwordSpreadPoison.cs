using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Rogue/Spread Poison", order = 1)]
    public class Effect_RogueSwordSpreadPoison : Effect_Base
    {
        public float radius;
        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                Collider[] hitColliders = Physics.OverlapSphere(ctx.AttackData._InitialPosition, radius);
                foreach(var hitCollider in hitColliders)
                {
                    if(hitCollider.GetComponent<Actor>())
                    {
                        Actor otherActor = hitCollider.GetComponent<Actor>(); 
                        if(otherActor._Team != ctx.AttackData._Team) //Is the team the caster of the ability? Or the owner of the status effect?
                        {

                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}

