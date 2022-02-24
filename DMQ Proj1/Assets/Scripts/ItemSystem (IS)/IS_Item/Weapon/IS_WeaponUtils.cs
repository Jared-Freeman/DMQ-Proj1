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
        /// <param name="attached_cooldown">optional cooldown instance to respect. Mostly obsolete</param>
        /// <returns>the newly created projectile, or null if something fails</returns>
        public static GenericProjectile CreateProjectileFromAttackContext(
            GenericProjectile proj_prefab
            , AttackContext ctx
            , EffectTree.Effect_LaunchProjectile.SpawnContextOptions preferredSpawnDirection = EffectTree.Effect_LaunchProjectile.SpawnContextOptions.InitialDirection
            , float forwardOffset = 0f
            , Utils.CooldownTracker attached_cooldown = null)
        {
            return CreateProjectileFromEffectContext(proj_prefab, new EffectTree.EffectContext(ctx), preferredSpawnDirection, forwardOffset, attached_cooldown);
        }


        /// <summary>
        /// Creates a projectile based on the given EffectContext.
        /// </summary>
        /// <returns>the newly created projectile, or null if something fails</returns>
        public static GenericProjectile CreateProjectileFromEffectContext(
            GenericProjectile proj_prefab
            , EffectTree.EffectContext ctx
            , EffectTree.Effect_LaunchProjectile.SpawnContextOptions preferredSpawnDirection = EffectTree.Effect_LaunchProjectile.SpawnContextOptions.InitialDirection
            , float forwardOffset = 0f
            , Utils.CooldownTracker attached_cooldown = null
            )
        {
            if (attached_cooldown != null)
            {
                if (!attached_cooldown.CooldownAvailable) return null;
                else attached_cooldown.ConsumeCooldown();
            }

            if (proj_prefab != null)
            {
                Vector3 iDir = Helper_CreateProjectileFromAttackContext_GetDirection(preferredSpawnDirection, ctx);
                
                //try all enums
                if(iDir == Vector3.zero)
                {
                    foreach(EffectTree.Effect_LaunchProjectile.SpawnContextOptions s in System.Enum.GetValues(typeof(EffectTree.Effect_LaunchProjectile.SpawnContextOptions)))
                    {
                        iDir = Helper_CreateProjectileFromAttackContext_GetDirection(s, ctx);

                        if (iDir != Vector3.zero) break; //gross but quick
                    }
                }

                if (iDir != Vector3.zero)
                {
                    EffectTree.Effect_LaunchProjectile.SpawnContextOptions curOption = preferredSpawnDirection;


                    var instance = GenericProjectile.SpawnProjectile(proj_prefab
                        , ctx.AttackData._InitialPosition
                        , iDir
                        , new Vector2(iDir.x, iDir.z)
                        , ctx.AttackData._TargetGameObject
                        , ctx.AttackData._Owner);

                    //EXPERIMENTAL
                    //instance.transform.position += iDir.normalized * forwardOffset;

                    return instance;
                }
            }

            return null;

        }
        private static Vector3 Helper_CreateProjectileFromAttackContext_GetDirection(EffectTree.Effect_LaunchProjectile.SpawnContextOptions options, EffectTree.EffectContext ctx)
        {
            switch (options)
            {
                case EffectTree.Effect_LaunchProjectile.SpawnContextOptions.InitialDirection:
                    return ctx.AttackData._InitialDirection;
                case EffectTree.Effect_LaunchProjectile.SpawnContextOptions.TargetDirection:
                    return ctx.AttackData._TargetDirection;
                case EffectTree.Effect_LaunchProjectile.SpawnContextOptions.SurfaceNormal:
                    return ctx.ContextData._NormalVector;
                case EffectTree.Effect_LaunchProjectile.SpawnContextOptions.SurfaceNormal2D:
                    return ctx.ContextData._NormalVector2D;
                case EffectTree.Effect_LaunchProjectile.SpawnContextOptions.SurfaceReflected:
                    return ctx.ContextData._ReflectionVector;
                case EffectTree.Effect_LaunchProjectile.SpawnContextOptions.SurfaceReflected2D:
                    return ctx.ContextData._ReflectionVector2D;

                default:
                    Debug.LogError("Unrecognized SpawnContextOption! Is the spawn implementation written?");
                    return Vector3.zero;
            }
        }
    }
}