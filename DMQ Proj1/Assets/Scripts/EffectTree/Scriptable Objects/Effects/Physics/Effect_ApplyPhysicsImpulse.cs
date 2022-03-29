using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace EffectTree
{
    /// <summary>
    /// Applies a physic impulse to the target
    /// </summary>
    [CreateAssetMenu(fileName = "PItT_", menuName = "Effect Tree/Physics/Physics Impulse to Target", order = 2)]
    public class Effect_ApplyPhysicsImpulse : Effect_Base
    {
        public EffectContext.TargetOptions Target = EffectContext.TargetOptions._TargetGameObject;
        public enum ImpulseTargetStyles { FromCasterToTarget, ctxInitialDirection, ctxTargetDirection }
        public enum ImpulseForceStyles { Speed, ImpulseForce, ImpulseForceScaledByMass }

        /// <summary>
        /// How the direction is inferred from EffectContext
        /// </summary>
        [Tooltip("How the direction is inferred from EffectContext")]
        public ImpulseTargetStyles Direction = ImpulseTargetStyles.ctxInitialDirection;
        /// <summary>
        /// How force is calculated
        /// </summary>
        public ImpulseForceStyles ForceStyle;
        /// <summary>
        /// Options container
        /// </summary>
        public ImpOptions Options = new ImpOptions();

        [System.Serializable]
        public class ImpOptions
        {
            public bool TargetActorsOnly = false;
            public bool DontTargetActors = false;

            public float Amount = 5f;

            public Utils.TargetFilterOptions TargetFilters = new Utils.TargetFilterOptions();
        }


        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx))
            {
                GameObject goTarget = ctx.RetrieveGameObject(Target);

                //validations
                if (goTarget == null)
                {
                    return false;
                }

                Rigidbody targetRB = goTarget.GetComponentInChildren<Rigidbody>();
                Actor targetActor = goTarget.GetComponentInChildren<Actor>();

                if (targetRB == null)
                {
                    return false;
                }
                //Actor validation based on flags
                if ((Options.TargetActorsOnly && targetActor == null) || (Options.DontTargetActors && targetActor != null)) return false;

                //target filter checks
                //We allow targets that do NOT have an Actor component
                if (
                    ctx.AttackData._Team != null 
                    && targetActor != null 
                    && !Options.TargetFilters.TargetIsAllowed(ctx.AttackData._Team, targetActor) //We return false if target is not allowed.
                    ) 
                {
                    return false;
                }

                Vector3 forceDir;

                switch(Direction)
                {
                    case ImpulseTargetStyles.ctxInitialDirection:
                        forceDir = ctx.AttackData._InitialDirection;
                        break;
                    case ImpulseTargetStyles.ctxTargetDirection:
                        forceDir = ctx.AttackData._TargetDirection;
                        break;
                    case ImpulseTargetStyles.FromCasterToTarget:
                        if(ctx.AttackData._TargetGameObject != null  && ctx.AttackData._InitialGameObject != null)
                        {
                            forceDir = (ctx.AttackData._InitialGameObject.transform.position - ctx.AttackData._TargetGameObject.transform.position);
                        }
                        else
                        {
                            forceDir = ctx.AttackData._InitialDirection;
                        }
                        break;

                    default:
                        Debug.LogError("Unrecognized Direction. Does an impl exist?");
                        return false;
                }
                
                switch(ForceStyle)
                {
                    case ImpulseForceStyles.ImpulseForce:
                        targetRB.AddForce(forceDir.normalized * Options.Amount, ForceMode.Impulse);
                        break;
                    case ImpulseForceStyles.ImpulseForceScaledByMass:
                        targetRB.AddForce(forceDir.normalized * Options.Amount * targetRB.mass, ForceMode.Impulse);
                        break;
                    case ImpulseForceStyles.Speed:
                        targetRB.AddForce(forceDir.normalized * Options.Amount, ForceMode.VelocityChange);
                        break;

                    default:
                        Debug.LogError("Unrecognized ForceStyle. Does an impl exist?");
                        break;
                }

                return true;
            }
            return false;
        }
    }

}