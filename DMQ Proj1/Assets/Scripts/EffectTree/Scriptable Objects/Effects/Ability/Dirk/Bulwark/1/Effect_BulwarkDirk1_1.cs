using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Bulwark/Plant Tower Shield", order = 1)]
    public class Effect_BulwarkDirk1_1 : Effect_Base
    {
        public GameObject ShieldPrefab;
        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                GameObject shield = Instantiate(ShieldPrefab, ctx.AttackData._InitialPosition,Quaternion.identity);
                DirkBulwarkTowerShield towerShield = shield.GetComponent<DirkBulwarkTowerShield>();
                towerShield.ctx = ctx;
                return true;
            }
            return false;
        }
    }
}

