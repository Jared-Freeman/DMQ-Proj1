using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "LP_", menuName = "Effect Tree/Launch Projectile", order = 1)]
    public class Effect_LaunchProjectile : Effect_Base
    {
        public GenericProjectile Projectile;

        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {
                Utils.Projectile.CreateProjectileFromAttackContext(Projectile, ctx.AttackData);
                return true;
            }
            return false;
        }
    }
}