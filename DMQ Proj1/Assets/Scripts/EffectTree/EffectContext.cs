using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class EffectContextEventArgs : System.EventArgs
    {
        /// <summary>
        /// Context arg of this event
        /// </summary>
        /// <remarks>
        /// This is a REFERENCE. You can mutate the context of any effect trees using this Arg packet!
        /// </remarks>
        public EffectTree.EffectContext ctx;

        public EffectContextEventArgs(EffectTree.EffectContext context)
        {
            ctx = context;
        }
    }
}

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
        /// <remarks>
        /// THIS IS NOT A COPY CONSTRUCTOR. The objects are acquired by REFERENCE. 
        /// See <see cref="EffectContext"/> for Constructor definitions.
        /// </remarks>
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
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Object values to copy into this <see cref="EffectContext"/></param>
        public EffectContext(EffectContext other)
        {
            //we use this class' copy ctor
            AttackData = new Utils.AttackContext(other.AttackData);

            ContextData = new EffectContextInfo();

            ContextData._NormalVector = other.ContextData._NormalVector;
            ContextData._NormalVector2D = other.ContextData._NormalVector2D;
            ContextData._ReflectionVector = other.ContextData._ReflectionVector;
            ContextData._ReflectionVector2D = other.ContextData._ReflectionVector2D;
            ContextData._TriggeringCollider = other.ContextData._TriggeringCollider;
            ContextData._TriggeringCollision = other.ContextData._TriggeringCollision;
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
            public Vector3 _ReflectionVector = Vector3.zero;
            /// <summary>
            /// An inferred Normal Vector PROJECTED ONTO THE XZ PLANE. Mutable. 2D
            /// </summary>
            /// <remarks> 
            /// If not loaded in context, sqrMagnitude == 0. 
            /// </remarks>
            public Vector3 _NormalVector2D = Vector3.zero;
            /// <summary>
            /// An inferred Reflection Vector PROJECTED ONTO THE XZ PLANE. Mutable. 2D
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