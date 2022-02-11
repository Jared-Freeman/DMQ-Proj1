using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ActorSystem;

namespace EffectTree
{
    /// <summary>
    /// Applies damage to target actor, if one exists
    /// </summary>
    [CreateAssetMenu(fileName = "AD_", menuName = "Effect Tree/Actor/Heal Actor", order = 2)]
    public class Effect_HealActor : Effect_Base
    {
        [Tooltip("Should modifier multiply use a percentage of max stat value? (default uses current value such as current hp)")]
        public bool MultiplyByMaxValue = false;

        [Header("Only use Modifier fields!")]
        [Tooltip
            (
            "Default values are NOT used in this field. " +
            "\n*Multiply* will ADD a multiplied amount given by CURRENT or MAX value based on inspector flag. " +
            "\n*Add* will simply ADD the amount to the actor stats."
            )]
        public ActorStatsData Modifier = new ActorStatsData(0, 0);

        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx))
            {
                var a = ctx.AttackData._TargetGameObject.GetComponent<Actor>();
                if(a != null)
                {
                    a.Stats.ReceiveHealing(Modifier);
                }
                return true;
            }
            return false;
        }
    }
}
