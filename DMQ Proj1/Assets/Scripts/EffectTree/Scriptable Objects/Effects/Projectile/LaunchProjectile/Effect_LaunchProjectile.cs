using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "LP_", menuName = "Effect Tree/Projectile/Launch Projectile", order = 1)]
    public class Effect_LaunchProjectile : Effect_Base
    {
        public GameObject ProjectilePrefab;
        public bool UseTeamNoCollideLayer = false;
        public bool DoNotChangeProjectileLayer = false;

        [Header("To prevent collider clipping")]
        [Tooltip("Can set this to (<length of projectile colldier> / 2) + <tiny offset> for good results.")]
        public float ForwardOffset = 0f;

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

                var instance = Utils.Projectile.CreateProjectileFromEffectContext(Projectile, ctx, SpawnDirection, ForwardOffset);
                if(instance == null) return false;

                //apply forward offset
                instance.transform.position += instance.transform.forward * ForwardOffset;

                //if(Utils.Physics.GameobjectCollisionExists(instance.gameObject))
                //{
                //    Destroy(instance.gameObject);
                //    return false;
                //}

                if (ctx.AttackData._Team != null && !DoNotChangeProjectileLayer)
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