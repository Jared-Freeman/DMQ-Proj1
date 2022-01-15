using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GenericProjectileMover))]
public class GenericProjectile : MonoBehaviour
{
    public static GenericProjectile SpawnProjectile
        (
        GenericProjectile Template,
        Vector3 InitialPosition = default,
        Vector3 LaunchDirection = default, 
        GameObject Target = null
        )
    {
        GameObject GO = Instantiate(Template.gameObject);
        GenericProjectile PR = GO.GetComponent<GenericProjectile>();

        if (PR == null) return null;
                       
        switch (PR.Mover.MovementType)
        {
            case GenericProjectileMover.MovementStyle.None:
                break;

            case GenericProjectileMover.MovementStyle.LinearSimple:
                PR.Mover.MovementTypeOptions.LinearSimpleOptions.InitialDirection = LaunchDirection;
                break;

            case GenericProjectileMover.MovementStyle.ParabolicSimple:
                PR.Mover.MovementTypeOptions.ParabolicSimpleOptions.InitialHorizontalDirection = new Vector2(LaunchDirection.x, LaunchDirection.z);
                if (Target != null)
                    PR.Mover.MovementTypeOptions.ParabolicSimpleOptions.LaunchDistance = (GO.transform.position - Target.transform.position).magnitude;
                break;

            //Impl in Fixed FixedUpdateMovement()
            case GenericProjectileMover.MovementStyle.PhysicsContinuousForce:
                PR.Mover.MovementTypeOptions.PhysicsContinuousForceOptions.Direction = LaunchDirection;
                break;

            //Impl in Fixed FixedUpdateMovement()
            case GenericProjectileMover.MovementStyle.PhysicsImpulse:
                PR.Mover.MovementTypeOptions.PhysicsImpulseOptions.Direction = LaunchDirection;
                break;

            case GenericProjectileMover.MovementStyle.HomingSimple:
                if(Target != null)
                    PR.Mover.MovementTypeOptions.HomingSimpleOptions.Target = Target;   
                break;
        };
        
        GO.transform.position = InitialPosition;

        return GO.GetComponent<GenericProjectile>();
    }

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

    //My best attempt at describing how a projectile is allowed to despawn
    [System.Serializable]
    public struct GenericProjectileEffectDestroyOptions
    {
        public bool FLAG_UseDuration;
        public float Duration;
        
        [Header("Doubles as TriggerEnter if Collider == Trigger")]
        public bool FLAG_UseCollisionEnters;
        public int CollisionEnters;

        [Header("Doubles as TriggerStay if Collider == Trigger")]
        public bool FLAG_UseCollisionStayDuration;
        public int CollisionStayDuration;

    }
    public GenericProjectileEffectDestroyOptions DestroyOptions;

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
