using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HOW TO ADD A NEW MOVEMENT STYLE:
// 1. Add a MovementStyle enum
// 2. Add a case in the UpdateMovement() AND FixedUpdateMovement() switch statements for your enum (with an appropriately named method). 
//    * See how I did it if you're not super sure what to do. Update should either occur in fixed update OR update (mutually exclusive); fixed update supports Rigidbody Physics movers.
// 3. Add a [System.Serializable] struct (using your naming convention) containing public movement options for your method
// 4. Add this struct as a public member to the MovementStyleOptions struct
// 5. Implement your method! Note that your method is being called in Update()

// This mover is designed to be super modular. You may add your own MovementStyle by following the format above.
// Since MovementStyle's are implemented JUST in this class, I dont have a good solution for keeping state variable memory footprint low... maybe we can figure something out eventually
// For now just implement your state variables in this class if/when needed. I provided a few basic ones like ProjectileTimeAlive for you.
public class GenericProjectileMover : MonoBehaviour
{
    #region Members

    //flags
    public bool FLAG_FaceRigidbodyVelocity = false;

    public enum MovementStyle { None, LinearSimple, ParabolicSimple, PhysicsImpulse, PhysicsContinuousForce, HomingSimple };
    public MovementStyle MovementType = MovementStyle.LinearSimple; //DO NOT CHANGE THIS DURING RUNTIME!    

    public MovementStyleOptions MovementTypeOptions;

    //state variables (you can use these however you want in your mover)
    //counters
    float ProjectileTimeAlive = 0;
    float ProjectileCollisions = 0;

    private Rigidbody RB;

    #endregion

    #region Initialization

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        if (RB == null) RB = new Rigidbody();
    }

    private void Start()
    {
        InitializeMovementMethod();
    }

    #endregion

    #region Movement Method Initialization
    //Some movement methods need state variables to aid their movement (or to cache to improve performance)
    void InitializeMovementMethod()
    {
        switch (MovementType)
        {
            case MovementStyle.None:
                break;

            case MovementStyle.LinearSimple:

                break;

            case MovementStyle.ParabolicSimple:
                InitParabolicSimple();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsContinuousForce:
                InitPhysicsContinuous();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsImpulse:
                InitPhysicsImpulse();
                break;

            case MovementStyle.HomingSimple:
                InitHomingSimple();
                break;
        };
    }
    #endregion

    #region Update Methods
    void Update()
    {
        if(FLAG_FaceRigidbodyVelocity) FaceVelocityForward();
        UpdateMovement();
        ProjectileTimeAlive += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        FixedUpdateMovement();

    }

    void UpdateMovement()
    {
        switch(MovementType)
        {
            case MovementStyle.None:
                break;

            case MovementStyle.LinearSimple:
                LinearSimpleMovement();
                break;

            case MovementStyle.ParabolicSimple:
                ParabolicSimpleMovement();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsContinuousForce:
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsImpulse:
                break;

            case MovementStyle.HomingSimple:
                HomingSimpleMovement();
                break;

            default:
                Debug.LogError("MovementStyle has no implementation method specified!");
                break;
        };
    }

    void FixedUpdateMovement()
    {

        switch (MovementType)
        {
            case MovementStyle.None:
                break;

            //Impl in Fixed UpdateMovement()
            case MovementStyle.LinearSimple:
                break;

            //Impl in Fixed UpdateMovement()
            case MovementStyle.ParabolicSimple:
                break;

            case MovementStyle.PhysicsContinuousForce:
                PhysicsContinuousForceMovement();
                break;

            case MovementStyle.PhysicsImpulse:
                PhysicsImpulseMovement();
                break;

            //Impl in Fixed UpdateMovement()
            case MovementStyle.HomingSimple:
                break;

            default:
                Debug.LogError("MovementStyle has no implementation method specified!");
                break;
        };
    }
    #endregion

    #region Movement Methods (Option Struct's, Update's and Start's)
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

        public float MaxProjectileDuration;
        public int MaxCollisions;
    };

    void LinearSimpleMovement()
    {
        //transform.position += MovementTypeOptions.LinearSimpleOptions.InitialDirection.normalized * MovementTypeOptions.LinearSimpleOptions.Speed * Time.deltaTime;

        transform.Translate(MovementTypeOptions.LinearSimpleOptions.InitialDirection.normalized * MovementTypeOptions.LinearSimpleOptions.Speed * Time.deltaTime);
        if(ProjectileTimeAlive > MovementTypeOptions.LinearSimpleOptions.MaxProjectileDuration)
        {

        }
    }



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

    //members
    Vector3 ParaS_ImpactTarget = Vector3.zero;
    Vector3 ParaS_InitialPosition = Vector3.zero;
    float ParaS_CurrentTimestamp = 0f;

    //init
    void InitParabolicSimple()
    {
        ParaS_ImpactTarget = transform.position
            + (new Vector3(
                MovementTypeOptions.ParabolicSimpleOptions.InitialHorizontalDirection.x
                , 0
                , MovementTypeOptions.ParabolicSimpleOptions.InitialHorizontalDirection.y)
            * MovementTypeOptions.ParabolicSimpleOptions.LaunchDistance);

        ParaS_InitialPosition = transform.position;

        AnimationCurve Curv = MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime;
        if (Curv == null)
        {
            //This creates a faux linear projectile
            MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime = AnimationCurve.Constant(0, 1, 0);
            //Add a keyframe for apex of parabolic arc
            MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime.AddKey(new Keyframe(.5f, 1));
        }
    }
    //update
    void ParabolicSimpleMovement()
    {
        ParaS_CurrentTimestamp += Time.deltaTime;
        ParaS_CurrentTimestamp = Mathf.Clamp(ParaS_CurrentTimestamp, 0, MovementTypeOptions.ParabolicSimpleOptions.TravelTime);

        float AnimationCoefficient = ParaS_CurrentTimestamp / MovementTypeOptions.ParabolicSimpleOptions.TravelTime;

        Vector3 NextPosition = (ParaS_ImpactTarget - ParaS_InitialPosition).normalized * AnimationCoefficient * MovementTypeOptions.ParabolicSimpleOptions.LaunchDistance;
        NextPosition.y += MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime.Evaluate(AnimationCoefficient) * MovementTypeOptions.ParabolicSimpleOptions.MaxHeight;

        transform.position = NextPosition;
    }





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

    //init
    void InitPhysicsImpulse()
    {
        RB.isKinematic = false;

        if(MovementTypeOptions.PhysicsImpulseOptions.FLAGUseSpeedComputationInsteadOfForce)
        {
            RB.AddForce(MovementTypeOptions.PhysicsImpulseOptions.Direction.normalized * MovementTypeOptions.PhysicsImpulseOptions.Speed, ForceMode.VelocityChange);
        }
        else if(MovementTypeOptions.PhysicsImpulseOptions.FLAGScaleForceByMass)
        {
            RB.AddForce(MovementTypeOptions.PhysicsImpulseOptions.Direction.normalized * MovementTypeOptions.PhysicsImpulseOptions.Force * RB.mass, ForceMode.Impulse);
        }
        else
        {
            RB.AddForce(MovementTypeOptions.PhysicsImpulseOptions.Direction.normalized * MovementTypeOptions.PhysicsImpulseOptions.Force, ForceMode.Impulse);
        }
    }

    //fixed update
    void PhysicsImpulseMovement()
    {

    }



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

    //init
    void InitPhysicsContinuous()
    {
        RB.isKinematic = false;

        if (MovementTypeOptions.PhysicsContinuousForceOptions.Direction.sqrMagnitude == 0)
        {
            Debug.LogError("Projectile Direction not set!");
        }
    }

    //fixed update
    void PhysicsContinuousForceMovement()
    {
        if(MovementTypeOptions.PhysicsContinuousForceOptions.FLAGScaleForceByMass)
        {
            RB.AddForce(MovementTypeOptions.PhysicsContinuousForceOptions.Direction.normalized * MovementTypeOptions.PhysicsContinuousForceOptions.ForcePerSecond * RB.mass * Time.fixedDeltaTime, ForceMode.Force);
        }
        else
        {
            RB.AddForce(MovementTypeOptions.PhysicsContinuousForceOptions.Direction.normalized * MovementTypeOptions.PhysicsContinuousForceOptions.ForcePerSecond * Time.fixedDeltaTime, ForceMode.Force);
        }
    }



    /// <summary>
    /// Turns towards the target at a rate of TurnRate degrees per second
    /// </summary>
    [System.Serializable]
    public struct HomingSimpleMovementOptions
    {
        public GameObject Target;
        public float Speed;
        [Min(0f)] [Tooltip("Degrees per second")]
        public float TurnRate; //Deg / Sec
    };

    //members
    Vector3 HSM_CurrentDirection = Vector3.zero;

    //initialization
    void InitHomingSimple()
    {
        RB.isKinematic = true;
        if (MovementTypeOptions.HomingSimpleOptions.Target == null)
        {
            Debug.LogError("No GameObject assigned!");
        }
        else
        {
            HSM_CurrentDirection = (MovementTypeOptions.HomingSimpleOptions.Target.transform.position - transform.position).normalized;
        }

    }

    //update
    void HomingSimpleMovement()
    {
        HSM_CurrentDirection = Vector3.RotateTowards(
            HSM_CurrentDirection
            , (MovementTypeOptions.HomingSimpleOptions.Target.transform.position - transform.position).normalized
            , Mathf.Deg2Rad * MovementTypeOptions.HomingSimpleOptions.TurnRate * Time.deltaTime
            , 0);

        transform.position += HSM_CurrentDirection * MovementTypeOptions.HomingSimpleOptions.Speed * Time.deltaTime;
    }



    #endregion

    void FaceVelocityForward()
    {
        if(RB.velocity.sqrMagnitude != 0) transform.forward = RB.velocity.normalized;
    }

    //Can probably add event handlers PER movement style by just adding a switch statement into any event handler here
    #region Event Handlers
    private void OnCollisionEnter(Collision collision)
    {
        ProjectileCollisions++;
    }
    #endregion
}
