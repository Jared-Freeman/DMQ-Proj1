using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "LP_", menuName = "Effect Tree/Set Homing Target to Owned Actor", order = 1)]
    public class Effect_SetHomingProjectileTargetToOwnedActor : Effect_Base
    {
        public GameObject ProjectilePrefab;
        public bool UseTeamNoCollideLayer = false;

        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {
                var Projectile = ProjectilePrefab.GetComponent<GenericProjectile>();
                if (Projectile == null) return false;

                var instance = Utils.Projectile.CreateProjectileFromAttackContext(Projectile, ctx.AttackData);
                if (instance == null) return false;

                instance.Info.Target = ctx.AttackData._Owner.gameObject;
                if (ctx.AttackData._Team != null)
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
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance.gameObject, ctx.AttackData._Team.Options.Layer))
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

