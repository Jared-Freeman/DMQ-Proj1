using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Rotates DIRECTION vectors about the Y axis. 
    /// </summary>
    /// <remarks>
    /// ORDER MATTERS in euler rotation transformations. This script only does yaw for 2D applications.
    /// </remarks>
    [CreateAssetMenu(fileName = "EulerRot_", menuName = "Effect Tree/Modify Context/Apply Euler Rotation to Direction Vectors", order = 1)]
    public class Effect_ApplyEulerRotation : Effect_Base
    {
        public List<Effect_Base> List_NextEffects = new List<Effect_Base>();

        [Header("Rotation about X, Y, and Z Axes")]
        public Vector3 EulerRotation;

        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                //COPY CTOR, refs are not passed along
                EffectContext newContext = new EffectContext(ctx);

                Quaternion rot = Quaternion.Euler(EulerRotation);

                //Update Attack Data
                newContext.AttackData._InitialDirection = rot * newContext.AttackData._InitialDirection;
                newContext.AttackData._TargetDirection = rot * newContext.AttackData._TargetDirection;

                //update Context Data
                newContext.ContextData._ReflectionVector = rot * newContext.ContextData._ReflectionVector;
                newContext.ContextData._NormalVector = rot * newContext.ContextData._NormalVector;

                //update 2D projections
                newContext.ContextData._ReflectionVector2D = Vector3.ProjectOnPlane(newContext.ContextData._ReflectionVector, Vector3.up);
                newContext.ContextData._NormalVector2D = Vector3.ProjectOnPlane(newContext.ContextData._NormalVector, Vector3.up);

                foreach (var e in List_NextEffects)
                {
                    e.Invoke(ref newContext);
                }
                return true;
            }
            return false;
        }
    }

}