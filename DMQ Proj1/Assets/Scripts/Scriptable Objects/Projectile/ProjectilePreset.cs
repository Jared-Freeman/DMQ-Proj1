using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public Vector3 InitialDirection;
        public float Speed;
    };
    
    /// <summary>
    /// Moves in a simple parabolic arc based on an initial 2D direction, distance, travel time, and an AnimationCurve to describe height over time
    /// </summary>
    [System.Serializable]
    public struct ParabolicSimpleMovementOptions
    {
        public Vector2 InitialHorizontalDirection;
        public float LaunchDistance;
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

        public Vector3 Direction;
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
        public Vector3 Direction;
        public float ForcePerSecond;
    };

    /// <summary>
    /// Turns towards the target at a rate of TurnRate degrees per second
    /// </summary>
    [System.Serializable]
    public struct HomingSimpleMovementOptions
    {
        public GameObject Target;
        public float Speed;
        [Min(0f)]
        [Tooltip("Degrees per second")]
        public float TurnRate; //Deg / Sec
    };
    #endregion
}
