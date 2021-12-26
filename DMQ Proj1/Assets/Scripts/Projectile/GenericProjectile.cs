using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GenericProjectileMover))]
public class GenericProjectile : MonoBehaviour
{
    #region Members
    public GenericProjectileMover Mover;
    private Rigidbody RB; //Its a good idea to initially make this Kinematic. Could be done in script idk...

    [System.Serializable]
    public struct GenericProjectileMessagePayload
    {
        public List<ProjectileEffect> ProjectileFXList;

        public void PerformProjectileEffects(GenericProjectile Projectile)
        {
            foreach (ProjectileEffect PE in ProjectileFXList)
            {
                PE.PerformPayloadEffect(Projectile);
            }
        }
    };

    public GenericProjectileMessagePayload EndProjectileEffects; //TODO: make this extendable or something
    public GenericProjectileMessagePayload CollisionEnterProjectileEffects;
    public GenericProjectileMessagePayload CollisionStayProjectileEffects;
    public GenericProjectileMessagePayload CollisionExitProjectileEffects;
    public GenericProjectileMessagePayload StartProjectileEffects; //TODO: make this extendable or something
    #endregion

    #region Events

    #endregion

    #region Initialization
    void Start()
    {
        Mover = GetComponent<GenericProjectileMover>();
        if (Mover == null) Mover = new GenericProjectileMover();
        RB = GetComponent<Rigidbody>();
        if (RB == null) RB = new Rigidbody();


        StartProjectileEffects.PerformProjectileEffects(this);
    }
    #endregion


    void DestroyProjectile()
    {
        Destroy(this);
    }

    #region Collision FX Invokers
    //Collision FX
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Enter!");
        CollisionEnterProjectileEffects.PerformProjectileEffects(this);
    }
    private void OnCollisionStay(Collision collision)
    {
        CollisionStayProjectileEffects.PerformProjectileEffects(this);
    }
    private void OnCollisionExit(Collision collision)
    {
        CollisionExitProjectileEffects.PerformProjectileEffects(this);
    }

    //Currently, we consider Trigger / Collision to proc the same Projectile FX
    private void OnTriggerEnter(Collider other)
    {
        CollisionEnterProjectileEffects.PerformProjectileEffects(this);
    }
    private void OnTriggerStay(Collider other)
    {
        CollisionStayProjectileEffects.PerformProjectileEffects(this);
    }
    private void OnTriggerExit(Collider other)
    {
        CollisionExitProjectileEffects.PerformProjectileEffects(this);
    }
    #endregion
}
