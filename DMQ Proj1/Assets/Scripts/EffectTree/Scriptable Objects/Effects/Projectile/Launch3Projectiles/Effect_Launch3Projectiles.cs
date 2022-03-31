using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// DEPRECATED. DO NOT USE.
    /// </summary>
    /// <remarks>
    /// Can use <see cref="Effect_LaunchProjectile"/> with an <see cref="Effect_Persistent"/>, an <see cref="Effect_Set"/>,  
    /// or a ModifyContext such as <see cref="Effect_ApplyEulerRotation"/>, etc. to get this functionality.
    /// </remarks>
    //[CreateAssetMenu(fileName = "LP_", menuName = "Effect Tree/Launch 3 Projectiles", order = 1)]
    public class Effect_Launch3Projectiles : Effect_Base
    {
        public GameObject ProjectilePrefab;
        public bool UseTeamNoCollideLayer = false;
        public float radius;
        public float angleStep;
        public override bool Invoke(ref EffectContext ctx)
        {
            var Projectile = ProjectilePrefab.GetComponent<GenericProjectile>();
            if (Projectile == null) return false;
            if (base.Invoke(ref ctx))
            {
                float angle = 0;
                Vector3 startPoint = ctx.AttackData._InitialDirection;
                for(int i = 0;i<3;i++)
                {
                    float projectileDirXPos = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                    float projectileDirYPos = startPoint.z + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                    angle += angleStep;
                    ctx.AttackData._InitialDirection = new Vector3(projectileDirXPos, 0, projectileDirYPos);
                    var proj = Utils.Projectile.CreateProjectileFromAttackContext(Projectile, ctx.AttackData);
                    if (ctx.AttackData._Team != null)
                    {
                        if (UseTeamNoCollideLayer)
                        {
                            if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(proj.gameObject, ctx.AttackData._Team.Options.NoCollideLayer))
                            {
                                Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                            }
                        }
                        else
                        {
                            if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(proj.gameObject, ctx.AttackData._Team.Options.Layer))
                            {
                                Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}

