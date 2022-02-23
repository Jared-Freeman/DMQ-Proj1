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
        #region Constructors

        /// <summary>
        /// Attaches the supplied <paramref name="ctx"/> arg to this class
        /// </summary>
        /// <param name="ctx"></param>
        public EffectContext(Utils.AttackContext ctx, EffectContextInfo effectInfo = null)
        {
            AttackData = ctx;
            if(effectInfo != null)
            {
                ContextData = effectInfo;
            }
        }
        public EffectContext()
        {
            AttackData = new Utils.AttackContext();
            ContextData = new EffectContextInfo();
        }
        #endregion


        #region Members

        /// <summary>
        /// Context supplied from Attack message pattern.
        /// </summary>
        /// <remarks>See <see cref="Utils"/> namespace for def</remarks>
        public Utils.AttackContext AttackData = new Utils.AttackContext();
        /// <summary>
        /// Supplementary context supplied from <see cref="EffectTree"/> context message.
        /// </summary>
        public EffectContextInfo ContextData = new EffectContextInfo();

        #region Helpers

        public class EffectContextInfo
        {
            /// <summary>
            /// Collision data that triggered this effect chain. Mutable.
            /// </summary>
            /// <remarks> 
            /// Null if not loaded in context. Can be supplied from OnCollsion Unity Messages  
            /// </remarks>
            public Collision _TriggeringCollision = null;
            /// <summary>
            /// Collider data that triggered this effect chain. Mutable.
            /// </summary>
            /// <remarks> 
            /// Null if not loaded in context. Can be supplied from OnCollsion or OnTrigger Unity Messages 
            /// </remarks>
            public Collider _TriggeringCollider = null;
            /// <summary>
            /// An inferred Normal Vector. Mutable.
            /// </summary>
            /// <remarks> 
            /// If not loaded in context, sqrMagnitude == 0. 
            /// </remarks>
            public Vector3 _NormalVector = Vector3.zero;
            /// <summary>
            /// An inferred Reflection Vector. Mutable.
            /// </summary>
            /// <remarks> 
            /// If not loaded in context, sqrMagnitude == 0. 
            /// </remarks>
            public Vector3 _ReflectionVector2D = Vector3.zero;
        }

        #endregion

        #endregion
    }
}