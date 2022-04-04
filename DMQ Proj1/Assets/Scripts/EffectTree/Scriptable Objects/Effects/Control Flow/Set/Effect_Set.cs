using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Dispatches a list of effects
    /// </summary>
    [CreateAssetMenu(fileName = "SET_", menuName = "Effect Tree/Set", order = 1)]
    public class Effect_Set : Effect_Base
    {
        public List<Effect_Base> Effects = new List<Effect_Base>();

        public override bool Invoke(ref EffectContext ctx)
        {
            if (Globals.s_LogEffectTree) Debug.Log("SET: " + ToString());

            if(base.Invoke(ref ctx))
            {
                foreach (var e in Effects)
                {
                    e.Invoke(ref ctx);
                }
                return true;
            }

            return false;

        }
    }
}