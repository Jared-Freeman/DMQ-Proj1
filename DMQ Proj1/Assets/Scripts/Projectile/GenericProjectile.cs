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

        public void PerformProjectileEffects(GenericProjectile Projectile, Collider Col = null)
        {
            foreach (ProjectileEffect PE in ProjectileFXList)
            {
                PE.PerformPayloadEffect(Projectile, Col);
            }
        }
    };

    [System.Serializable]
    public struct GenericProjectileEffectOptions
    {
        public GenericProjectileMessagePayload EndProjectileEffects;
        public GenericProjectileMessagePayload CollisionEnterProjectileEffects;
        public GenericProjectileMessagePayload CollisionStayProjectileEffects;
        public GenericProjectileMessagePayload CollisionExitProjectileEffects;
        public GenericProjectileMessagePayload StartProjectileEffects;
    };
    public GenericProjectileEffectOptions ProjectileFX;

    #endregion

    #region Events

    #endregion

    #region Initialization

    private void Awake()
    {
        Mover = GetComponent<GenericProjectileMover>();
        if (Mover == null) Mover = new GenericProjectileMover();
        RB = GetComponent<Rigidbody>();
        if (RB == null) RB = new Rigidbody();
    }

    void Start()
    {
        ProjectileFX.StartProjectileEffects.PerformProjectileEffects(this);
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
        ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, collision.collider);
    }
    private void OnCollisionStay(Collision collision)
    {
        ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, collision.collider);
    }
    private void OnCollisionExit(Collision collision)
    {
        ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, collision.collider);
    }

    //Currently, we consider Trigger / Collision to proc the same Projectile FX
    private void OnTriggerEnter(Collider other)
    {
        ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, other);
    }
    private void OnTriggerStay(Collider other)
    {
        ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, other);
    }
    private void OnTriggerExit(Collider other)
    {
        ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, other);
    }
    #endregion
}
