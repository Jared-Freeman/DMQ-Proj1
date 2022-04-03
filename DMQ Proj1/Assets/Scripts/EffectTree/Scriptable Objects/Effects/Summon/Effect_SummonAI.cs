using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Summon AI", order = 1)]
    public class Effect_SummonAI : Effect_Base
    {
        public GameObject summonPrefab;
        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx))
            {
                GameObject summon = Instantiate(summonPrefab,ctx.AttackData._InitialPosition,Quaternion.identity);
                Actor summonActor = summon.GetComponent<Actor>();
                summonActor._Team = ctx.AttackData._Team;
                return true;
            }
            return false;
        }
    }
}

