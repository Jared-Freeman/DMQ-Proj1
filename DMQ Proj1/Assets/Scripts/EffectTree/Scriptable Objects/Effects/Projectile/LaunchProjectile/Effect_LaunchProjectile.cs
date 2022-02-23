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
        public bool UseTeamNoCollideLayer = false;

        public enum SpawnContextOptions { InitialDirection, TargetDirection, SurfaceNormal, SurfaceNormal2D, SurfaceReflected, SurfaceReflected2D }
        /// <summary>
        /// Determines the facing of the projectile upon instantiation. 
        /// </summary>
        /// <remarks>
        /// All options will be tried until one succeeds or all fail.
        /// </remarks>
        public SpawnContextOptions SpawnDirection = SpawnContextOptions.InitialDirection;

        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {
                var Projectile = ProjectilePrefab.GetComponent<GenericProjectile>();
                if (Projectile == null) return false;

                var instance = Utils.Projectile.CreateProjectileFromEffectContext(Projectile, ctx, SpawnDirection);
                if(instance == null) return false;

                if(ctx.AttackData._Team != null)
                {
                    if (UseTeamNoCollideLayer)
                    {
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance.gameObject, ctx.AttackData._Team.Options.NoCollideLayer))
                        {
                            Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                        }
                    }
                    else
                    {
                        if(!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance.gameObject, ctx.AttackData._Team.Options.Layer))
                        {
                            Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                        }
                    }
                }

                return true;
            }
            return false;
        }
    }
}