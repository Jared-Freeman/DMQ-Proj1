using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Any and all info needed for effect tree logic. Append more stuff here as needed. Context may be modified during effect tree invocation
    /// </summary>
    [System.Serializable]
    public class EffectContext
    {
        /// <summary>
        /// Attaches the supplied <paramref name="ctx"/> arg to this class
        /// </summary>
        /// <param name="ctx"></param>
        public EffectContext(Utils.AttackContext ctx)
        {
            AttackData = ctx;
        }
        public EffectContext()
        {
            AttackData = new Utils.AttackContext();
        }

        public Utils.AttackContext AttackData = new Utils.AttackContext();
    }
}