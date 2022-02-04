using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class Projectile
    {
        /// <summary>
        /// Creates a projectile based on the given AttackContext, projectile prefab; optionally respects a supplied cooldown
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="ctx"></param>
        /// <param name="attached_cooldown">optional cooldown instance to respect</param>
        /// <returns>the newly created projectile, or null if something fails</returns>
        public static GenericProjectile CreateProjectileFromAttackContext(GenericProjectile proj_prefab, AttackContext ctx, Utils.CooldownTracker attached_cooldown = null)
        {
            if(attached_cooldown != null)
            {
                if (!attached_cooldown.CooldownAvailable) return null;
                else attached_cooldown.ConsumeCooldown();
            }

            if (proj_prefab != null)
            {
                var iDir = ctx._InitialDirection;
                var instance = GenericProjectile.SpawnProjectile(proj_prefab, ctx._InitialPosition, iDir, new Vector2(iDir.x, iDir.z), ctx._TargetGameObject, ctx._Owner);

                return instance;
            }

            return null;
        }
    }
}