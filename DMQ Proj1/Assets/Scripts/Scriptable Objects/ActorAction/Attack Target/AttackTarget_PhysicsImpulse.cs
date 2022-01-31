using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem;

namespace AP2
{
    [CreateAssetMenu(fileName = "ActorAction", menuName = "ScriptableObjects/Actor Actions/Attack/Attack Target/Physic Impulse", order = 1)]
    public class AttackTarget_PhysicsImpulse : AP2_ActorAction_AttackTarget
    {
        [System.Serializable]
        public class MoreOptions
        {
            public bool IgnoreHeightDifference = true; //if true, project the direction onto the XZ plane, effectively making the force "2D." I highly recommend doing this!
            public float Force = 1f;
        }

        public MoreOptions SpecialOptions;


        public override void AttackTarget(Actor Owner, GameObject Target, Vector3 DirectionNormal = default)
        {
            base.AttackTarget(Owner, Target, DirectionNormal);

            if(Target.GetComponent<Rigidbody>() != null)
            {
                if (FLAG_Debug) Debug.Log("PI: " + Target.name);

                if(SpecialOptions.IgnoreHeightDifference)
                {
                    if(DirectionNormal != default && DirectionNormal.sqrMagnitude > 0)
                    {
                        Target.GetComponent<Rigidbody>().AddForce
                            (
                            Vector3.ProjectOnPlane(DirectionNormal, Vector3.up).normalized * SpecialOptions.Force
                            , ForceMode.Impulse
                            );
                    }
                    else
                    {
                        Target.GetComponent<Rigidbody>().AddForce
                            (
                            Vector3.ProjectOnPlane((Target.gameObject.transform.position - Owner.gameObject.transform.position), Vector3.up).normalized * SpecialOptions.Force
                            , ForceMode.Impulse
                            );
                    }
                }
                else
                {
                    if (DirectionNormal != default && DirectionNormal.sqrMagnitude > 0)
                    {
                        Target.GetComponent<Rigidbody>().AddForce
                            (
                            DirectionNormal * SpecialOptions.Force
                            , ForceMode.Impulse
                            );
                    }
                    else
                    {
                        Target.GetComponent<Rigidbody>().AddForce
                            (
                            (Target.gameObject.transform.position - Owner.gameObject.transform.position).normalized * SpecialOptions.Force
                            , ForceMode.Impulse
                            );
                    }
                }
            }
        }
    }

}