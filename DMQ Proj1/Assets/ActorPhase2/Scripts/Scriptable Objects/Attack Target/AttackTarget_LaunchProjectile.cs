using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AP2
{
    [CreateAssetMenu(fileName = "ActorAction", menuName = "ScriptableObjects/Actor Actions/Attack/Attack Target/Launch Projectile", order = 1)]

    public class AttackTarget_LaunchProjectile : AP2.AP2_ActorAction_AttackTarget
    {
        [System.Serializable]
        public class MoreOptions
        {
            public GameObject Projectile;

            //TODO: Consider changing how projectile offset is handled
            [Tooltip("Negative Values == backwards")]
            public float Offset_Forward = 1f;
            //[Tooltip("Negative Values == left")]
            //public float Offset_Horizontal = 0f;
            //[Tooltip("Negative Values == down")]
            //public float Offset_Vertical = 0f;
        }

        [Header("Add the Damage Message to the Projectile as a ProjectileEffect!")]
        public MoreOptions SpecialOptions;

        public override void PerformAction(Actor Owner)
        {
            base.PerformAction(Owner);

            if (FLAG_Debug) Debug.Log(Owner.name);

            GenericProjectile Projectile_Template = SpecialOptions.Projectile.GetComponent<GenericProjectile>();
            if(Projectile_Template != null)
            {
                //TODO: Consider event dispatch here

                Vector3 LaunchDir = (Options.Target.transform.position - Owner.gameObject.transform.position).normalized;
                Vector3 LaunchPos = Owner.gameObject.transform.position
                    + LaunchDir * SpecialOptions.Offset_Forward;

                GenericProjectile Proj = GenericProjectile.SpawnProjectile(Projectile_Template, LaunchPos, LaunchDir, Options.Target);


                //if (Proj != null)
                //{
                //    Proj.Mover.MovementTypeOptions.PhysicsImpulseOptions.Direction = LaunchDir.normalized;
                //}
            }
            else
            {
                Debug.LogError("AttackTarget_LaunchProjectile: Projectile Prefab is missing GenericProjectile Script!");
            }
        }
    }
}
