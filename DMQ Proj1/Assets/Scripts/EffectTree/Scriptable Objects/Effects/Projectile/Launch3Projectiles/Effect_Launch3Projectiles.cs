using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "LP_", menuName = "Effect Tree/Launch 3 Projectiles", order = 1)]
    public class Effect_Launch3Projectiles : Effect_Base
    {
        public GameObject ProjectilePrefab;
        public bool UseTeamNoCollideLayer = false;

        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                var Projectile = ProjectilePrefab.GetComponent<GenericProjectile>();
                if (Projectile == null) return false;

                //Give the projectiles some spread? Not sure how to do this. 

                var instance1= Utils.Projectile.CreateProjectileFromAttackContext(Projectile, ctx.AttackData);
                if (instance1== null) return false;

                var instance2 = Utils.Projectile.CreateProjectileFromAttackContext(Projectile, ctx.AttackData);
                if (instance2 == null) return false;

                var instance3 = Utils.Projectile.CreateProjectileFromAttackContext(Projectile, ctx.AttackData);
                if (instance3 == null) return false;

                if (ctx.AttackData._Team != null)
                {
                    if (UseTeamNoCollideLayer)
                    {
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance1.gameObject, ctx.AttackData._Team.Options.NoCollideLayer))
                        {
                            Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                        }
                    }
                    else
                    {
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance1.gameObject, ctx.AttackData._Team.Options.Layer))
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

