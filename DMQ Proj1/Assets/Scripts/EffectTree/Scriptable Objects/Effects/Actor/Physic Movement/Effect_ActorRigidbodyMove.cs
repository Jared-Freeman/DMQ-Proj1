using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CSEventArgs
{
    public class ActorRigidbodyMoveEventArgs : System.EventArgs
    {
        /// <summary>
        /// Context arg of this event
        /// </summary>
        /// <remarks>
        /// This is a REFERENCE. You can mutate the context of any effect trees using this Arg packet!
        /// </remarks>
        public EffectTree.EffectContext ctx;

        /// <summary>
        /// Actor targeted by this movement effect
        /// </summary>
        public Actor TargetedActor;

        /// <summary>
        /// The GameObject with persistent movement data being invoked during this event
        /// </summary>
        public EffectTree.Effect_ActorRigidbodyMove_Helper MovementCookie;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="context">Context arg of this event</param>
        /// <param name="target"> Actor targeted by this movement effect</param>
        /// <param name="cookie">The GameObject with persistent movement data being invoked during this event</param>
        public ActorRigidbodyMoveEventArgs(EffectTree.EffectContext context, Actor target, EffectTree.Effect_ActorRigidbodyMove_Helper cookie)
        {
            ctx = context;
            TargetedActor = target;
            MovementCookie = cookie;
        }
    }
}

namespace EffectTree
{
    /// <summary>
    /// Adds a movement task to the Actor's Rigidbody, and sends an event
    /// </summary>
    /// <remarks>
    /// The helper Component, <see cref="Effect_ActorRigidbodyMove_Helper"/> uses the initial direction to create smooth, interpolated movement over a curve.
    /// </remarks>
    [CreateAssetMenu(fileName = "ActMove_", menuName = "Effect Tree/Actor/Rigidbody Movement Task", order = 1)]
    public class Effect_ActorRigidbodyMove : Effect_Base
    {
        public static event System.EventHandler<CSEventArgs.ActorRigidbodyMoveEventArgs> OnRigidbodyMovementAppliedToActor;

        /// <summary>
        /// Who to apply the RB movement to
        /// </summary>
        public enum TargetSelection { EffectTreeOwner, InitialGameObject, TargetGameObject }

        /// <summary>
        /// When this movement starts, do we cancel out existing velocity?
        /// </summary>
        public bool OverrideCurrentVelocity = false;

        public TargetSelection TargetContext = TargetSelection.EffectTreeOwner;

        public EffectTree.EffectContext.FacingOptions Direction = EffectContext.FacingOptions._InitialDirection;

        [Header("A good default is a constant at 1")]
        [Tooltip("The range 0 to 1 is mapped to Duration")]
        public AnimationCurve VelocityScaleCurve;

        /// <summary>
        /// Length of movement
        /// </summary>
        public float Duration = 1f;

        /// <summary>
        /// The velocity when VelocityScaleCurve eval to 1
        /// </summary>
        public float VelocityDefault = 10f;

        /// <summary>
        /// Amount to decelerate, should the unlikely case occur
        /// </summary>
        public float Deceleration = 10f;

        //TODO: Consider inferring from ctx dir vectors other than _InitialDirection

        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx))
            {
                GameObject go = new GameObject("RigidbodyMovement Cookie");
                var h = go?.AddComponent<Effect_ActorRigidbodyMove_Helper>();

                if(h == null)
                {
                    Destroy(go);
                    return false;
                }

                h.Preset = this;

                h.Direction = ctx.RetrieveDirectionVector(Direction);

                switch (TargetContext)
                {
                    case TargetSelection.EffectTreeOwner:
                        h.Target = ctx.AttackData._Owner;
                        break;
                    case TargetSelection.InitialGameObject:
                        h.Target = ctx.AttackData._InitialGameObject.GetComponent<Actor>();
                        break;
                    case TargetSelection.TargetGameObject:
                        h.Target = ctx.AttackData._TargetGameObject.GetComponent<Actor>();
                        break;

                    default:
                        Debug.LogError("No implementation for specified TargetContext. Does an Implementation exist?");
                        break;
                }


                OnRigidbodyMovementAppliedToActor?.Invoke(this, new CSEventArgs.ActorRigidbodyMoveEventArgs(ctx, h.Target, h));
                return true;
            }
            return false;
        }
    }

}