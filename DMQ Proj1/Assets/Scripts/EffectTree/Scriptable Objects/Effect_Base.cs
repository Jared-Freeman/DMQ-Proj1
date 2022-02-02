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
        /// Invoke this Effect's effects! Context may be copied and altered if needed
        /// </summary>
        /// <param name="ctx">Effect context. PASSED BY REFERENCE!!!</param>
        public virtual void Invoke(ref EffectContext ctx)
        {

        }
    }

    /// <summary>
    /// Any and all info needed for effect tree logic. Append more stuff here as needed. Context may be modified during effect tree invocation
    /// </summary>
    [System.Serializable]
    public struct EffectContext
    {

    }
}