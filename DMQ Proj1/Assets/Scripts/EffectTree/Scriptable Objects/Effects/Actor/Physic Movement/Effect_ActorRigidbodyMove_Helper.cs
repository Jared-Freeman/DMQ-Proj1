using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class Effect_ActorRigidbodyMove_HelperEventArgs : System.EventArgs
    {
        public EffectTree.Effect_ActorRigidbodyMove_Helper MovementHelper;

        public Effect_ActorRigidbodyMove_HelperEventArgs(EffectTree.Effect_ActorRigidbodyMove_Helper helper)
        {
            MovementHelper = helper;
        }
    }
}

namespace EffectTree
{
    /// <summary>
    /// Monobehavior to contain the persistent data across a RigidbodyMove sequence
    /// </summary>
    public class Effect_ActorRigidbodyMove_Helper : MonoBehaviour
    {
        //events
        public event System.EventHandler<CSEventArgs.Effect_ActorRigidbodyMove_HelperEventArgs> OnMovementComplete;

        //assigned by Effect
        public Effect_ActorRigidbodyMove Preset;
        public EffectContext Context;
        public Actor Target;

        //assigned by self
        protected Rigidbody TargetRB;
        protected float StartTime;

        void Start()
        {
            if (Preset == null) DestroyHelper();

            TargetRB = Target.GetComponent<Rigidbody>();
            if (TargetRB == null) DestroyHelper();

            StartTime = Time.time;
            StartCoroutine(I_ContinueMovement(Preset.Duration));
        }

        public IEnumerator I_ContinueMovement(float duration)
        {
            duration = Mathf.Abs(duration);

            yield return new WaitForFixedUpdate(); //this needs to come first

            if(Preset.OverrideCurrentVelocity && TargetRB != null && Mathf.Abs(Time.time - StartTime) < duration)
            {
                TargetRB.AddForce(
                    Preset.VelocityScaleCurve.Evaluate((Time.time - StartTime) / duration) * Context.AttackData._InitialDirection.normalized
                    , ForceMode.VelocityChange
                    );
                yield return new WaitForFixedUpdate();
            }

            while (Mathf.Abs(Time.time - StartTime) < duration)
            {

                if (TargetRB == null) DestroyHelper();

                //Yes, this code is disgusting. No, it's not worth the time to debug and polish right now.

                Vector3 DesiredVelocityDiff = Vector3.zero;
                Vector3 DashAddVelocity = Vector3.zero;
                float DashSpeedCoef = Preset.VelocityScaleCurve.Evaluate((Time.time - StartTime) / duration);
                Vector3 DashDesiredVelocity = Context.AttackData._InitialDirection.normalized * DashSpeedCoef * Preset.VelocityDefault;
                float ForceCoefficient = TargetRB.mass / Time.fixedDeltaTime;
                Vector3 FilteredRBVelocity = new Vector3(TargetRB.velocity.x, 0, TargetRB.velocity.z);
                //Filtered velocity represents the maximum velocity we can affect in this timestep (the rigidbody can exceed this by a LOT in normal gameplay)
                Vector3 FilteredVelocity = FilteredRBVelocity;

                //max is clamped to current player-desired velocity max.
                if (FilteredVelocity.sqrMagnitude > DashDesiredVelocity.sqrMagnitude) FilteredVelocity = DashDesiredVelocity.magnitude * FilteredVelocity.normalized;


                //compute parallel component of current velocity to desired velocity
                if (DashDesiredVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
                    DesiredVelocityDiff = (Vector3.Dot(DashDesiredVelocity, FilteredVelocity) / DashDesiredVelocity.magnitude) * DashDesiredVelocity.normalized;

                //Recall that we are comparing parallel vectors here
                if (Vector3.Dot(DashDesiredVelocity, FilteredRBVelocity) < 0)
                {
                    //decelerating. Parallel component of velocity can be at MOST Deceleration * Time.FixedDeltaTime            
                    Vector3 Parallel = Vector3.zero;
                    Vector3 Perpendicular = Vector3.zero;

                    Parallel = (Vector3.Dot(DashDesiredVelocity, FilteredVelocity) / FilteredVelocity.magnitude) * FilteredVelocity.normalized;

                    DashAddVelocity = (Parallel.normalized * Preset.Deceleration * Time.fixedDeltaTime) + Perpendicular;
                    
                }
                else
                {
                    //sustaining velocity
                    DashAddVelocity = (DashDesiredVelocity - DesiredVelocityDiff);
                }

                TargetRB.AddForce(DashAddVelocity * ForceCoefficient, ForceMode.Force);
                TargetRB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);

                yield return new WaitForFixedUpdate();

            }
            DestroyHelper();
        }

        /// <summary>
        /// Cleans up GameObject and invokes completion event
        /// </summary>
        protected void DestroyHelper()
        {
            OnMovementComplete?.Invoke(this, new CSEventArgs.Effect_ActorRigidbodyMove_HelperEventArgs(this));
            Destroy(gameObject);
        }
    }

    
}