using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
                       
        switch (PR._Data.MoveOptions.MovementType)
        {
            case ProjectileMoveStyle.None:
                break;

            case ProjectileMoveStyle.LinearSimple:
                PR._Data.MoveOptions.MovementTypeOptions.LinearSimpleOptions.InitialDirection = LaunchDirection;
                break;

            case ProjectileMoveStyle.ParabolicSimple:
                PR._Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.InitialHorizontalDirection = new Vector2(LaunchDirection.x, LaunchDirection.z);
                if (Target != null)
                    PR._Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.LaunchDistance = (GO.transform.position - Target.transform.position).magnitude;
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsContinuousForce:
                PR._Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.Direction = LaunchDirection;
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsImpulse:
                PR._Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Direction = LaunchDirection;
                break;

            case ProjectileMoveStyle.HomingSimple:
                if(Target != null)
                    PR._Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.Target = Target;   
                break;
        };
        
        GO.transform.position = InitialPosition;

        return GO.GetComponent<GenericProjectile>();
    }

    #endregion


    #region Members
    private Rigidbody RB; //Its a good idea to initially make this Kinematic. Could be done in script idk...
    public Actor ActorOwner;

    public ProjectilePreset _Data;

    /* old on-board data
    // -------------------------------------------------------------------
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
    // -------------------------------------------------------------------
    */


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
        RB = GetComponent<Rigidbody>();
        if (RB == null) RB = new Rigidbody();

        if(_Data == null)
        {
            Debug.LogError(ToString() + ": No Projectile Data loaded! Destroying object.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Info = new StateInfo();

        InitializeMovementMethod();

        _Data.ProjectileFX.StartProjectileEffects.PerformProjectileEffects(this);
    }
    #endregion

    private void FixedUpdate()
    {
        UpdateDuration();

        FixedUpdateMovement();
    }

    void Update()
    {
        if (_Data.MoveOptions.FLAG_FaceRigidbodyVelocity) FaceVelocityForward();
        UpdateMovement();
    }


    void UpdateDuration()
    {
        Info.Duration += Time.fixedDeltaTime;
        if (_Data.DestroyOptions.FLAG_UseDuration && Mathf.Abs(Info.StartTimestamp - Time.time) > Mathf.Abs(_Data.DestroyOptions.Duration))
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        _Data.ProjectileFX.EndProjectileEffects.PerformProjectileEffects(this);
        Destroy(gameObject);
    }

    #region Collision FX Invokers

    /// <summary>
    /// Returns a CollisionFilterContext for this object. Useless currently :(
    /// </summary>
    /// <returns></returns>
    public CollisionFilterContext GetCFContext()
    {
        if(ActorOwner != null)
        {
            return new CollisionFilterContext(gameObject, ActorOwner, ActorOwner._Team);
        }
        else
        {
            return new CollisionFilterContext(gameObject, null, null);
        }
    }

    //Collision FX
    private void OnCollisionEnter(Collision collision)
    {
        _Data.ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, collision.collider);

        Info.CollisionEnters++;
        if (_Data.DestroyOptions.FLAG_UseCollisionEnters && Info.CollisionEnters >= _Data.DestroyOptions.CollisionEnters)
        {
            DestroyProjectile();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        _Data.ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, collision.collider);

        Info.CollisionStayDuration += Time.fixedDeltaTime;
        if (_Data.DestroyOptions.FLAG_UseCollisionStayDuration && Info.CollisionStayDuration > _Data.DestroyOptions.CollisionStayDuration)
        {
            DestroyProjectile();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        _Data.ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, collision.collider);
        
    }

    //Currently, we consider Trigger / Collision to proc the same Projectile FX
    private void OnTriggerEnter(Collider other)
    {
        _Data.ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, other);

        Info.CollisionEnters++;
        if (_Data.DestroyOptions.FLAG_UseCollisionEnters && Info.CollisionEnters >= _Data.DestroyOptions.CollisionEnters)
        {
            DestroyProjectile();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        _Data.ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, other);

        Info.CollisionStayDuration += Time.fixedDeltaTime;
        if (_Data.DestroyOptions.FLAG_UseCollisionStayDuration && Info.CollisionStayDuration > _Data.DestroyOptions.CollisionStayDuration)
        {
            DestroyProjectile();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _Data.ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, other);
    }
    #endregion





    #region Movement


    #region Movement Method Initialization
    //Some movement methods need state variables to aid their movement (or to cache to improve performance)
    void InitializeMovementMethod()
    {
        switch (_Data.MoveOptions.MovementType)
        {
            case ProjectileMoveStyle.None:
                break;

            case ProjectileMoveStyle.LinearSimple:
                if (_Data.MoveOptions.FLAG_FaceRigidbodyVelocity)
                {
                    Debug.DrawRay(transform.position, _Data.MoveOptions.MovementTypeOptions.LinearSimpleOptions.InitialDirection.normalized * 5f, Color.cyan, 5f);

                    transform.rotation = Quaternion.LookRotation(_Data.MoveOptions.MovementTypeOptions.LinearSimpleOptions.InitialDirection.normalized, Vector3.up);
                }
                break;

            case ProjectileMoveStyle.ParabolicSimple:
                InitParabolicSimple();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsContinuousForce:
                InitPhysicsContinuous();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsImpulse:
                InitPhysicsImpulse();
                break;

            case ProjectileMoveStyle.HomingSimple:
                InitHomingSimple();
                break;
        };
    }
    #endregion

    #region Update Methods
    void UpdateMovement()
    {
        switch (_Data.MoveOptions.MovementType)
        {
            case ProjectileMoveStyle.None:
                break;

            case ProjectileMoveStyle.LinearSimple:
                LinearSimpleMovement();
                break;

            case ProjectileMoveStyle.ParabolicSimple:
                ParabolicSimpleMovement();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsContinuousForce:
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsImpulse:
                break;

            case ProjectileMoveStyle.HomingSimple:
                HomingSimpleMovement();
                break;

            default:
                Debug.LogError("ProjectileMoveStyle has no implementation method specified!");
                break;
        };
    }

    void FixedUpdateMovement()
    {

        switch (_Data.MoveOptions.MovementType)
        {
            case ProjectileMoveStyle.None:
                break;

            //Impl in Fixed UpdateMovement()
            case ProjectileMoveStyle.LinearSimple:
                break;

            //Impl in Fixed UpdateMovement()
            case ProjectileMoveStyle.ParabolicSimple:
                break;

            case ProjectileMoveStyle.PhysicsContinuousForce:
                PhysicsContinuousForceMovement();
                break;

            case ProjectileMoveStyle.PhysicsImpulse:
                PhysicsImpulseMovement();
                break;

            //Impl in Fixed UpdateMovement()
            case ProjectileMoveStyle.HomingSimple:
                break;

            default:
                Debug.LogError("ProjectileMoveStyle has no implementation method specified!");
                break;
        };
    }
    #endregion

    #region Movement Methods (Option Struct's, Update's and Start's)
    void LinearSimpleMovement()
    {
        transform.position += _Data.MoveOptions.MovementTypeOptions.LinearSimpleOptions.InitialDirection.normalized * _Data.MoveOptions.MovementTypeOptions.LinearSimpleOptions.Speed * Time.deltaTime;
    }

    // ---------------------------------------------------------------------- 
    //members
    Vector3 ParaS_ImpactTarget = Vector3.zero;
    Vector3 ParaS_InitialPosition = Vector3.zero;
    float ParaS_CurrentTimestamp = 0f;
    //init
    void InitParabolicSimple()
    {
        ParaS_ImpactTarget = transform.position
            + (new Vector3(
                _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.InitialHorizontalDirection.x
                , 0
                , _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.InitialHorizontalDirection.y)
            * _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.LaunchDistance);

        ParaS_InitialPosition = transform.position;

        AnimationCurve Curv = _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime;
        if (Curv == null)
        {
            //This creates a faux linear projectile
            _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime = AnimationCurve.Constant(0, 1, 0);
            //Add a keyframe for apex of parabolic arc
            _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime.AddKey(new Keyframe(.5f, 1));
        }
    }
    //update
    void ParabolicSimpleMovement()
    {
        ParaS_CurrentTimestamp += Time.deltaTime;
        ParaS_CurrentTimestamp = Mathf.Clamp(ParaS_CurrentTimestamp, 0, _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.TravelTime);

        float AnimationCoefficient = ParaS_CurrentTimestamp / _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.TravelTime;

        Vector3 NextPosition = (ParaS_ImpactTarget - ParaS_InitialPosition).normalized * AnimationCoefficient * _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.LaunchDistance;
        NextPosition.y += _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime.Evaluate(AnimationCoefficient) * _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.MaxHeight;

        //tinybrain velocity approximation
        if (_Data.MoveOptions.FLAG_FaceRigidbodyVelocity)
        {
            //MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime.



            transform.rotation = Quaternion.LookRotation(_Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.InitialHorizontalDirection, Vector3.up);


        }
        transform.position = NextPosition;
    }


    // ----------------------------------------------------------------------
    //init
    void InitPhysicsImpulse()
    {
        RB.isKinematic = false;

        if (_Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.FLAGUseSpeedComputationInsteadOfForce)
        {
            RB.AddForce(_Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Speed, ForceMode.VelocityChange);
        }
        else if (_Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.FLAGScaleForceByMass)
        {
            RB.AddForce(_Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Force * RB.mass, ForceMode.Impulse);
        }
        else
        {
            RB.AddForce(_Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Force, ForceMode.Impulse);
        }
    }
    //fixed update
    void PhysicsImpulseMovement()
    {

    }



    // ----------------------------------------------------------------------
    //init
    void InitPhysicsContinuous()
    {
        RB.isKinematic = false;

        if (_Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.Direction.sqrMagnitude == 0)
        {
            Debug.LogError("Projectile Direction not set!");
        }
    }
    //fixed update
    void PhysicsContinuousForceMovement()
    {
        if (_Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.FLAGScaleForceByMass)
        {
            RB.AddForce(_Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.ForcePerSecond * RB.mass * Time.fixedDeltaTime, ForceMode.Force);
        }
        else
        {
            RB.AddForce(_Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.ForcePerSecond * Time.fixedDeltaTime, ForceMode.Force);
        }
    }



    // ----------------------------------------------------------------------
    //members
    Vector3 HSM_CurrentDirection = Vector3.zero;
    //initialization
    void InitHomingSimple()
    {
        RB.isKinematic = true;
        if (_Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.Target == null)
        {
            Debug.LogError("No GameObject assigned!");
        }
        else
        {
            HSM_CurrentDirection = (_Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.Target.transform.position - transform.position).normalized;
        }

    }
    //update
    void HomingSimpleMovement()
    {
        HSM_CurrentDirection = Vector3.RotateTowards(
            HSM_CurrentDirection
            , (_Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.Target.transform.position - transform.position).normalized
            , Mathf.Deg2Rad * _Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.TurnRate * Time.deltaTime
            , 0);

        transform.position += HSM_CurrentDirection * _Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.Speed * Time.deltaTime;
    }
       
    #endregion



    void FaceVelocityForward()
    {
        switch (_Data.MoveOptions.MovementType)
        {
            case ProjectileMoveStyle.PhysicsContinuousForce:
            case ProjectileMoveStyle.PhysicsImpulse:
                if (RB.velocity.sqrMagnitude != 0) transform.forward = RB.velocity.normalized;
                break;


            default:
                break;
        }
    }


    #endregion
}
