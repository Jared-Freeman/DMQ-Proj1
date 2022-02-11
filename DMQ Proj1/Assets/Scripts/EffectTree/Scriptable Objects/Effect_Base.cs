using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Base class for all EffectTree nodes
    /// </summary>
    public abstract class Effect_Base : ScriptableObject
    {
        /// <summary>
        /// Conditions that must evaluate true for this effect to Invoke()
        /// </summary>
        public Condition.ConditionList Conditions = new Condition.ConditionList();

        /// <summary>
        /// Invoke this Effect's effects! Context may be copied and altered if needed
        /// </summary>
        /// <param name="ctx">Effect context. PASSED BY REFERENCE!!!</param>
        public virtual bool Invoke(ref EffectContext ctx)
        {
            return Conditions.Evaluate(ref ctx);
        }
    }

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