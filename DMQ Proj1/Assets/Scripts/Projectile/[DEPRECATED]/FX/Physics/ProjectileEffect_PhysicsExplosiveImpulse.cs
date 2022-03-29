using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "ProjectileEffect_PhysicsExplosiveImpulse", menuName = "ScriptableObjects/ProjectileEffect/Physics/Explosive Impulse to Target", order = 1)]
public class ProjectileEffect_PhysicsExplosiveImpulse : ProjectileEffect
{
    #region Members

    [System.Serializable]
    public struct ExpImpulseOptions
    {
        public bool FLAG_ScaleForceByTargetMass;
        public float ExplosiveForce;
        public float ExplosiveRadius;

    };
    public ExpImpulseOptions Options;

    #endregion


    public override void PerformPayloadEffect(GenericProjectile Projectile, Collider Col = null)
    {
        base.PerformPayloadEffect(Projectile, Col);

        Vector3 ExpPosition = Projectile.transform.position;

        if (Col != null && Col.GetComponent<Rigidbody>() != null)
        {
            if (Col.GetComponent<Rigidbody>() != null)
            {
                if (Options.FLAG_ScaleForceByTargetMass)
                {
                    Col.GetComponent<Rigidbody>().AddExplosionForce(Options.ExplosiveForce * Col.GetComponent<Rigidbody>().mass, ExpPosition, Options.ExplosiveRadius, 0, ForceMode.Impulse);
                }
                else
                {
                    Col.GetComponent<Rigidbody>().AddExplosionForce(Options.ExplosiveForce, ExpPosition, Options.ExplosiveRadius, 0, ForceMode.Impulse);
                }
            }
        }
    }
}
