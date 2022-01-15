using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GenericProjectileMover))]
public class GenericProjectile : MonoBehaviour
{
    #region Static Methods

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

    #endregion


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

    private StateInfo Info;
    private class StateInfo
    {
        public float StartTimestamp = Time.time;
        public float Duration = 0;
        public int CollisionEnters = 0;
        public float CollisionStayDuration = 0;
    }

    //public-facing state info
    public float TimeAlive
    {
        get
        {
            return Info.Duration;
        }
    }
    public int CollisionCount
    {
        get
        {
            return Info.CollisionEnters;
        }
    }

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
        Info = new StateInfo();
        ProjectileFX.StartProjectileEffects.PerformProjectileEffects(this);
    }
    #endregion

    private void FixedUpdate()
    {
        Info.Duration += Time.fixedDeltaTime;
        if(DestroyOptions.FLAG_UseDuration && Mathf.Abs(Info.StartTimestamp - Time.time) > Mathf.Abs(DestroyOptions.Duration))
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        ProjectileFX.EndProjectileEffects.PerformProjectileEffects(this);
        Destroy(gameObject);
    }

    #region Collision FX Invokers
    //Collision FX
    private void OnCollisionEnter(Collision collision)
    {
        ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, collision.collider);

        Info.CollisionEnters++;
        if(DestroyOptions.FLAG_UseCollisionEnters && Info.CollisionEnters >= DestroyOptions.CollisionEnters)
        {
            DestroyProjectile();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, collision.collider);

        Info.CollisionStayDuration += Time.fixedDeltaTime;
        if(DestroyOptions.FLAG_UseCollisionStayDuration && Info.CollisionStayDuration > DestroyOptions.CollisionStayDuration)
        {
            DestroyProjectile();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, collision.collider);
    }

    //Currently, we consider Trigger / Collision to proc the same Projectile FX
    private void OnTriggerEnter(Collider other)
    {
        ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, other);

        Info.CollisionEnters++;
        if (DestroyOptions.FLAG_UseCollisionEnters && Info.CollisionEnters >= DestroyOptions.CollisionEnters)
        {
            DestroyProjectile();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, other);

        Info.CollisionStayDuration += Time.fixedDeltaTime;
        if (DestroyOptions.FLAG_UseCollisionStayDuration && Info.CollisionStayDuration > DestroyOptions.CollisionStayDuration)
        {
            DestroyProjectile();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, other);
    }
    #endregion
}
