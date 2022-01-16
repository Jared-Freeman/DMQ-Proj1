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
            public float Offset_Vertical = 1f;
            //[Tooltip("Negative Values == left")]
            //public float Offset_Horizontal = 0f;
            //[Tooltip("Negative Values == down")]
            //public float Offset_Vertical = 0f;
        }

        [Header("Add the Damage Message to the Projectile as a ProjectileEffect!")]
        public MoreOptions SpecialOptions;

        public override void AttackTarget(Actor Owner, GameObject Target)
        {
            base.AttackTarget(Owner, Target);

            if (FLAG_Debug) Debug.Log(Owner.name);

            GenericProjectile Projectile_Template = SpecialOptions.Projectile.GetComponent<GenericProjectile>();
            if(Projectile_Template != null)
            {
                //TODO: Consider event dispatch here

                Vector3 LaunchDir = (Target.transform.position - Owner.gameObject.transform.position).normalized;
                Vector3 LaunchPos = Owner.gameObject.transform.position
                    + LaunchDir * SpecialOptions.Offset_Forward;
                LaunchPos.y = SpecialOptions.Offset_Vertical;

                GenericProjectile Proj = GenericProjectile.SpawnProjectile(Projectile_Template, LaunchPos, LaunchDir, Target);


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
