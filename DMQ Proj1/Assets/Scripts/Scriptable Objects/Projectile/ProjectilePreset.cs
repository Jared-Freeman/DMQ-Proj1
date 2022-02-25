using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EffectTree;


public enum ProjectileMoveStyle { None, LinearSimple, ParabolicSimple, PhysicsImpulse, PhysicsContinuousForce, HomingSimple, /*NYI*/DirectionGuided };


//TODO: State variables still exist in Mover helper data structures. Move them over to GenericProjectile.cs

[CreateAssetMenu(fileName = "PR_", menuName = "ScriptableObjects/Projectile Preset", order = 1)]
public class ProjectilePreset : ScriptableObject
{
    //FROM GENERIC PROJECTILE OPTIONS
    // -------------------------------------------------------------------
    public GenericProjectileEffectOptions ProjectileFX;
    public GenericProjectileEffectDestroyOptions DestroyOptions;

    [System.Serializable]
    public struct GenericProjectileMessagePayload
    {
        public List<Effect_Base> EffectList;
        [Header("Deprecated. Use EffectList instead")]
        public List<ProjectileEffect> ProjectileFXList;

        public void PerformProjectileEffects(GenericProjectile Projectile, Collider Col = null, Collision collision = null)
        {
            EffectContext c = new EffectContext();

            c.AttackData._InitialGameObject = Projectile.gameObject;
            c.AttackData._InitialDirection = Projectile.transform.forward;
            c.AttackData._InitialPosition = Projectile.gameObject.transform.position;

            if(collision != null)
            {
                //computations
                var reflect2 = Vector2.Reflect(new Vector2(Projectile.transform.forward.x, Projectile.transform.forward.z)
                    , new Vector2(collision.GetContact(0).normal.x, collision.GetContact(0).normal.z));

                var reflect2_3 = new Vector3(reflect2.x, 0, reflect2.y);

                var reflect3 = Vector3.Reflect(Projectile.transform.forward
                    , collision.GetContact(0).normal);

                //ctx assignments
                c.ContextData._TriggeringCollision = collision;
                //Done later
                //c.ContextData._TriggeringCollider = collision.collider; 

                c.ContextData._NormalVector = collision.GetContact(0).normal;
                c.ContextData._NormalVector2D = new Vector3(collision.GetContact(0).normal.x, 0, collision.GetContact(0).normal.z);
                c.ContextData._ReflectionVector = reflect3;
                c.ContextData._ReflectionVector2D = reflect2_3;

                //Here we override the InitialPosition with more specific data
                c.AttackData._InitialPosition = collision.GetContact(0).point;


                //Debug.DrawRay(collision.GetContact(0).point, reflect3 * 3, Color.green, 5f);
                //Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal * 3, Color.red, 5f);
            }

            c.AttackData._Owner = Projectile.ActorOwner;
            if (Projectile.ActorOwner != null)
                c.AttackData._Team = Projectile.ActorOwner._Team;

            if(Col != null)
            {
                c.AttackData._TargetGameObject = Col.gameObject;
                c.AttackData._TargetDirection = Projectile.transform.position - Col.gameObject.transform.position;
                c.AttackData._TargetPosition = Col.gameObject.transform.position;

                c.ContextData._TriggeringCollider = Col;
            }

            //TODO: Grab collision data and throw it into the remaining c.AttackData fields for Target

            foreach (var e in EffectList)
            {
                e.Invoke(ref c);
            }

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
    // -------------------------------------------------------------------






    //FROM PROJECTILE MOVER OPTIONS
    // -------------------------------------------------------------------
    public ProjMoverOptions MoveOptions;

    [System.Serializable]
    public struct ProjMoverOptions
    {
        public bool FLAG_FaceRigidbodyVelocity;

        public ProjectileMoveStyle MovementType; //DO NOT CHANGE THIS DURING RUNTIME!    

        public MovementStyleOptions MovementTypeOptions;

    }
    // -------------------------------------------------------------------






    #region Movement Helpers

    /// <summary>
    /// A container struct for organizing options in the inspector. All MovementOptions structs should have a member here!
    /// </summary>
    [System.Serializable]
    public struct MovementStyleOptions
    {
        public LinearSimpleMovementOptions LinearSimpleOptions;
        public ParabolicSimpleMovementOptions ParabolicSimpleOptions;
        public PhysicsContinuousForceMovementOptions PhysicsContinuousForceOptions;
        public PhysicsImpulseMovementOptions PhysicsImpulseOptions;
        public HomingSimpleMovementOptions HomingSimpleOptions;
    }

    /// <summary>
    /// Linear Simple movement simply moves the object in a direction at a given speed. 
    /// Designers may choose to have the projectile time out and/or disappear after a certain collision count
    /// </summary>
    [System.Serializable]
    public struct LinearSimpleMovementOptions
    {
        public float Speed;
    };
    
    /// <summary>
    /// Moves in a simple parabolic arc based on an initial 2D direction, distance, travel time, and an AnimationCurve to describe height over time
    /// </summary>
    [System.Serializable]
    public struct ParabolicSimpleMovementOptions
    {
        [Min(.0001f)]
        public float TravelTime; //Speed == LaunchDistance / TravelTime

        [Tooltip("Intervals [0,1] are scaled to 0...MaxHeight and 0...TravelTime")]
        public AnimationCurve HeightOverTime;

        [Min(.0001f)]
        public float MaxHeight;
    };

    /// <summary>
    /// Simply applies a physics impulse to the object. There are a few options to scale the force or to use a velocity change instead
    /// </summary>
    [System.Serializable]
    public struct PhysicsImpulseMovementOptions
    {
        public bool FLAGScaleForceByMass;
        public bool FLAGUseSpeedComputationInsteadOfForce; //Switches the behavior to compute outgoing speed of object instead of a raw force

        public float Force;
        public float Speed;
    };

    /// <summary>
    /// Basically just attaching a thruster to the back of the projectile. 
    /// Designers can disable gravity on the rigidbody to make this MovementType similar to a missile.
    /// </summary>
    [System.Serializable]
    public struct PhysicsContinuousForceMovementOptions
    {
        public bool FLAGScaleForceByMass;
        public float ForcePerSecond;
    };

    public enum HomingSimpleMovement_Dimensionality { Homing2D, Homing3D }
    /// <summary>
    /// Turns towards the target at a rate of TurnRate degrees per second
    /// </summary>
    [System.Serializable]
    public struct HomingSimpleMovementOptions
    {
        public HomingSimpleMovement_Dimensionality MovementStyle;

        public float Speed;
        [Min(0f)]
        [Tooltip("Degrees per second")]
        public float TurnRate; //Deg / Sec
    };
    #endregion
}
