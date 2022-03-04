using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree
{
    /// <summary>
    /// Finds the nearest Actor in the specified radius (from InitialPosition)
    /// </summary>
    [CreateAssetMenu(fileName = "CPMT_", menuName = "Effect Tree/Persistent Effect", order = 1)]
    public class Effect_Persistent : Effect_Base
    {
        public float Duration;
        public float Period;

        public List<Effect_Base> List_StartEffects = new List<Effect_Base>();
        public List<Effect_Base> List_PeriodicEffects = new List<Effect_Base>();
        public List<Effect_Base> List_DestroyEffects = new List<Effect_Base>();

        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx))
            {
                Effect_Persistent_Instance.CreateInstance(this, ctx);       

                return true;
            }
            return false;
        }
    }

}