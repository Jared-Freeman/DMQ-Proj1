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
        #region Static Methods

        /// <summary>
        /// Infers <see cref="EffectContextInfo"/> from the supplied <see cref="Collision"/>.
        /// </summary>
        /// <param name="collision">Collision to glean info frmo</param>
        /// <remarks>Uses the first contact point, and the first collider from that contact point to infer surface reflection. 
        /// Could be inaccurate depending largely on Unity's internal implementation.</remarks>
        /// <returns>A newly instantiated ContextInfo object built on the supplied arg</returns>
        public static EffectContextInfo CreateContextDataFromCollision(Collision collision)
        {
            if (collision != null && collision.contactCount > 0)
            {
                EffectContextInfo ctxI = new EffectContextInfo();
                var firstCollisionDir3 = collision.GetContact(0).thisCollider.gameObject.transform.forward;

                //computations
                var reflect2 = Vector2.Reflect(new Vector2(firstCollisionDir3.x, firstCollisionDir3.z)
                    , new Vector2(collision.GetContact(0).normal.x, collision.GetContact(0).normal.z));

                var reflect2_3 = new Vector3(reflect2.x, 0, reflect2.y);

                var reflect3 = Vector3.Reflect(firstCollisionDir3, collision.GetContact(0).normal);

                //EffectContextInfo assignments
                ctxI._TriggeringCollision = collision;
                ctxI._TriggeringCollider = collision.collider;

                ctxI._NormalVector = collision.GetContact(0).normal;
                ctxI._NormalVector2D = new Vector3(collision.GetContact(0).normal.x, 0, collision.GetContact(0).normal.z);
                ctxI._ReflectionVector = reflect3;
                ctxI._ReflectionVector2D = reflect2_3;

                return ctxI;
            }
            return null;
        }

        #endregion

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
        /// Options for obtaining a direction vector from the Context
        /// </summary>
        public enum FacingOptions
        {
            None, _InitialDirection, _TargetDirection
                , CollisionNormal, CollisionNormal2D
                , CollisionReflected, CollisionReflected2D
                , FromInitialToTarget
        }
        /// <summary>
        /// Options for obtaining a position vector from the Context
        /// </summary>
        public enum PositionOptions
        {
            None, _InitialPosition, _TargetPosition
                , CollisionImpactPoint
        }

        /// <summary>
        /// Options for obtaining a GameObject from the Context
        /// </summary>
        public enum TargetOptions
        {
            Caster, _InitialGameObject, _TargetGameObject
        }


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

        #region Methods

        /// <summary>
        /// Utility for getting a direction from the context using an option interface
        /// </summary>
        /// <param name="option">Choice of direction</param>
        /// <returns>The specified Direction or zero, if none exists</returns>
        /// <remarks>
        /// Results may not be normalized. Check for Vector3.zero to see if this operation failed.
        /// </remarks>
        public Vector3 RetrieveDirectionVector(FacingOptions option)
        {
            Vector3 zeroVector = Vector3.zero;

            switch(option)
            {
                case FacingOptions.None:
                    return zeroVector;

                case FacingOptions._InitialDirection:
                    return AttackData._InitialDirection;

                case FacingOptions._TargetDirection:
                    return AttackData._TargetDirection;
                
                case FacingOptions.CollisionNormal:
                    return ContextData._NormalVector;
                
                case FacingOptions.CollisionNormal2D:
                    return ContextData._NormalVector2D;
                
                case FacingOptions.CollisionReflected:
                    return ContextData._ReflectionVector;
               
                case FacingOptions.CollisionReflected2D:
                    return ContextData._ReflectionVector2D;
                
                case FacingOptions.FromInitialToTarget:
                    if(AttackData._InitialGameObject != null && AttackData._TargetGameObject != null)
                    {
                        return (AttackData._InitialGameObject.transform.position - AttackData._TargetGameObject.transform.position).normalized;
                    }
                    return zeroVector;

                default:
                    Debug.LogError("No direction found for FacingOption specified! Does a direction exist?");
                    return zeroVector;
            }
        }

        /// <summary>
        /// Returns the first valid direction vector in this Context.
        /// </summary>
        /// <returns>First valid direction, or Vector3.forward if none exists.</returns>
        public Vector3 GetAnyDirectionVector()
        {
            Vector3 result;

            //go through each FacingOption and try to retrieve a dir vector
            foreach (FacingOptions f in System.Enum.GetValues(typeof(FacingOptions)))
            {
                result = RetrieveDirectionVector(f);

                if (result.sqrMagnitude > 0) return result;
            }

            return Vector3.forward;
        }

        /// <summary>
        /// Returns a position vector based on input options
        /// </summary>
        /// <param name="option">The type of position to retrieve</param>
        /// <param name="result">Where to store the result, if one exists.</param>
        /// <returns>True, if a position was retrieved.</returns>
        public bool RetrievePosition(PositionOptions option, ref Vector3 result)
        {
            switch(option)
            {
                case PositionOptions.None:
                    return true;

                case PositionOptions._InitialPosition:
                    result = AttackData._InitialPosition;
                    return true;

                case PositionOptions._TargetPosition:
                    result = AttackData._TargetPosition;
                    return true;

                case PositionOptions.CollisionImpactPoint:
                    if(ContextData._TriggeringCollision != null && ContextData._TriggeringCollision.contactCount > 0)
                    {
                        result = ContextData._TriggeringCollision.GetContact(0).point;
                        return true;
                    }
                    return false;

                default:
                    Debug.LogError("Context did not interpret the PositionOption! Does an impl exist?");
                    break;
            }
            return false;
        }

        /// <summary>
        /// Retrieves gameobject from the context.
        /// </summary>
        /// <param name="option"></param>
        /// <returns>The GameObject specified or null if none exists.</returns>
        public GameObject RetrieveGameObject(TargetOptions option)
        {
            switch(option)
            {
                case TargetOptions.Caster:
                    return AttackData._Owner.gameObject;

                case TargetOptions._InitialGameObject:
                    return AttackData._InitialGameObject;

                case TargetOptions._TargetGameObject:
                    return AttackData._TargetGameObject;

                default:
                    Debug.LogError("Context did not interpret the TargetOption! Does an impl exist?");
                    break;
            }

            return null;
        }

        #endregion
    }
}