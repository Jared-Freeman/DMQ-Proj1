using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Dispatches a list of effects
    /// </summary>
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Set", order = 1)]
    public class Effect_LaunchProjectile : Effect_Base
    {
        public GenericProjectile Projectile;

        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {

                return true;
            }
            return false;
        }
    }
}