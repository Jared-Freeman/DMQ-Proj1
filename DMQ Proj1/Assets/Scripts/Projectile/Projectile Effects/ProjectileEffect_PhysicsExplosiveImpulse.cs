using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //For ToList()

[CreateAssetMenu(fileName = "ProjectileEffect_PhysicsExplosiveImpulse", menuName = "ScriptableObjects/ProjectileEffect/ProjectileEffect_PhysicsExplosiveImpulseScriptableObject", order = 1)]
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
        base.PerformPayloadEffect(Projectile);

        //Get all RBs in radius
        List<Rigidbody> RBs = new List<Rigidbody>();

        List<Collider> Colliders;

        Vector3 ExpPosition = Projectile.transform.position;

        Colliders = Physics.OverlapSphere(ExpPosition, Options.ExplosiveRadius).ToList(); //TODO: consider mask arg

        foreach(Collider col in Colliders)
        {
            if(col.GetComponent<Rigidbody>() != null)
            {
                if(Options.FLAG_ScaleForceByTargetMass)
                {
                    col.GetComponent<Rigidbody>().AddExplosionForce(Options.ExplosiveForce * col.GetComponent<Rigidbody>().mass, ExpPosition, Options.ExplosiveRadius, 0, ForceMode.Impulse);
                }
                else
                {
                    col.GetComponent<Rigidbody>().AddExplosionForce(Options.ExplosiveForce, ExpPosition, Options.ExplosiveRadius, 0, ForceMode.Impulse);
                }
            }
        }
    }
}
