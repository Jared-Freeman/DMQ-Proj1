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
        public GameObject ProjectilePrefab;

        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {
                var Projectile = ProjectilePrefab.GetComponent<GenericProjectile>();
                if (Projectile == null) return false;

                var instance = Utils.Projectile.CreateProjectileFromAttackContext(Projectile, ctx.AttackData);
                if(instance != null) return true;
            }
            return false;
        }
    }
}