using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "CPMT_", menuName = "Effect Tree/Projectile/Change Projectile Movement Type", order = 1)]
    public class Effect_ChangeProjectileMovementType : Effect_Base
    {
        public ProjectileMoveStyle NewMovementStyle;

        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx))
            {
                if (ctx.AttackData._InitialGameObject == null) return false;

                GenericProjectile proj = ctx.AttackData._InitialGameObject.GetComponent<GenericProjectile>();

                GenericProjectile.StateInfo newInfo = new GenericProjectile.StateInfo(
                    ctx.AttackData._InitialDirection
                    , new Vector2(ctx.AttackData._InitialDirection.x, ctx.AttackData._InitialDirection.z)
                    , ctx.AttackData._TargetGameObject);

                proj.ChangeMovementMethod(NewMovementStyle, newInfo);

                return true;
            }
            return false;
        }
    }

}