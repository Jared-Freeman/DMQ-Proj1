using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GenericProjectile : MonoBehaviour
{
    #region Static Methods

    /// <summary>
    /// Creates a Projectile based on the supplied GenericProjectile component; Context is interpreted from overloaded arguments.
    /// </summary>
    /// <param name="Template"></param>
    /// <param name="InitialPosition"></param>
    /// <param name="LaunchDirection"></param>
    /// <param name="LaunchDirection_2D"></param>
    /// <param name="Target"></param>
    /// <returns>The newly instantiated GenericProjectile</returns>
    public static GenericProjectile SpawnProjectile
        (
        GenericProjectile Template,
        Vector3 InitialPosition = default,
        Vector3 LaunchDirection = default,
        Vector3 LaunchDirection_2D = default,
        GameObject Target = null,
        Actor actorOwner = null
        )
    {
        GameObject GO = Instantiate(Template.gameObject);
        GenericProjectile PR = GO.GetComponent<GenericProjectile>();

        if (PR == null) return null;

        PR.Info = new StateInfo(LaunchDirection, LaunchDirection_2D, Target);

        PR.Info.CurrentMoveType = Template._Data.MoveOptions.MovementType;

        if (actorOwner != null) PR.ActorOwner = actorOwner;
            
        /* Old Info setting model
        switch (PR._Data.MoveOptions.MovementType)
        {
            case ProjectileMoveStyle.None:
                break;

            case ProjectileMoveStyle.LinearSimple:
                PR.Info.InitialDirection = LaunchDirection;
                break;

            case ProjectileMoveStyle.ParabolicSimple:
                PR.Info.InitialHorizontalDirection = new Vector2(LaunchDirection.x, LaunchDirection.z);
                if (Target != null)
                    PR.Info.LaunchDistance = (GO.transform.position - Target.transform.position).magnitude;
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsContinuousForce:
                PR.Info.Direction = LaunchDirection;
                break;

            //Impl in Fixed FixedUpdateMovement()
            case ProjectileMoveStyle.PhysicsImpulse:
                PR.Info.Direction = LaunchDirection;
                break;

            case ProjectileMoveStyle.HomingSimple:
                if(Target != null)
                    PR.Info.Target = Target;   
                break;
        };
        */
        
        GO.transform.position = InitialPosition;

        return GO.GetComponent<GenericProjectile>();
    }

    #endregion


    #region Members
    private Rigidbody RB; //Its a good idea to initially make this Kinematic. Could be done in script idk...
    public Actor ActorOwner;

    public ProjectilePreset _Data;


    public StateInfo Info = new StateInfo();

    [System.Serializable]
    public struct StateInfo
    {
        public StateInfo(Vector3 dir, Vector2 dir_2D, GameObject target)
        {
            StartTimestamp = Time.time;
            Duration = 0f;
            CollisionEnters = 0;
            CollisionStayDuration = 0;
            InitialDirection = dir;
            InitialHorizontalDirection = dir_2D;
            LaunchDistance = 0;
            Direction = dir;
            Target = target;
            CurrentMoveType = ProjectileMoveStyle.None;
        }

        public float StartTimestamp;
        public float Duration;
        public int CollisionEnters;
        public float CollisionStayDuration;

        public ProjectileMoveStyle CurrentMoveType;

        //Style-Specific stuff ---------------------
        public Vector3 InitialDirection;


        public Vector2 InitialHorizontalDirection;
        public float LaunchDistance;


        public Vector3 Direction;


        public GameObject Target;
        //End Style-Specific stuff ---------------------
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

        Info.CurrentMoveType = _Data.MoveOptions.MovementType;
    }

    void Start()
    {
        InitializeMovementMethod();

        _Data.ProjectileFX.StartProjectileEffects.PerformProjectileEffects(this);
    }
    #endregion

    /// <summary>
    /// Change the movement method during runtime
    /// </summary>
    public void ChangeMovementMethod(ProjectileMoveStyle newStyle, StateInfo newInfo)
    {
        Info = newInfo;
        Info.CurrentMoveType = newStyle;
        InitializeMovementMethod();
    }


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


    // Currently, I see no reason to have projectiles proc PFX on Trigger volumes. This check is used to filter out those.
    protected bool CheckOtherIsTrigger(Collider c)
    {
        return c.isTrigger;
    }

    //Collision FX
    private void OnCollisionEnter(Collision collision)
    {
        if (CheckOtherIsTrigger(collision.collider)) return;

        _Data.ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, collision.collider, collision);

        Info.CollisionEnters++;
        if (_Data.DestroyOptions.FLAG_UseCollisionEnters && Info.CollisionEnters >= _Data.DestroyOptions.CollisionEnters)
        {
            DestroyProjectile();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (CheckOtherIsTrigger(collision.collider)) return;

        _Data.ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, collision.collider, collision);

        Info.CollisionStayDuration += Time.fixedDeltaTime;
        if (_Data.DestroyOptions.FLAG_UseCollisionStayDuration && Info.CollisionStayDuration > _Data.DestroyOptions.CollisionStayDuration)
        {
            DestroyProjectile();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (CheckOtherIsTrigger(collision.collider)) return;

        _Data.ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, collision.collider, collision);
        
    }

    //Currently, we consider Trigger / Collision to proc the same Projectile FX
    private void OnTriggerEnter(Collider other)
    {
        if (CheckOtherIsTrigger(other)) return;

        _Data.ProjectileFX.CollisionEnterProjectileEffects.PerformProjectileEffects(this, other);

        Info.CollisionEnters++;
        if (_Data.DestroyOptions.FLAG_UseCollisionEnters && Info.CollisionEnters >= _Data.DestroyOptions.CollisionEnters)
        {
            DestroyProjectile();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (CheckOtherIsTrigger(other)) return;

        _Data.ProjectileFX.CollisionStayProjectileEffects.PerformProjectileEffects(this, other);

        Info.CollisionStayDuration += Time.fixedDeltaTime;
        if (_Data.DestroyOptions.FLAG_UseCollisionStayDuration && Info.CollisionStayDuration > _Data.DestroyOptions.CollisionStayDuration)
        {
            DestroyProjectile();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (CheckOtherIsTrigger(other)) return;

        _Data.ProjectileFX.CollisionExitProjectileEffects.PerformProjectileEffects(this, other);
    }
    #endregion





    #region Movement


    #region Movement Method Initialization
    //Some movement methods need state variables to aid their movement (or to cache to improve performance)
    void InitializeMovementMethod()
    {
        switch (Info.CurrentMoveType)
        {
            case ProjectileMoveStyle.None:
                break;

            case ProjectileMoveStyle.LinearSimple:
                if (_Data.MoveOptions.FLAG_FaceRigidbodyVelocity)
                {
                    Debug.DrawRay(transform.position, Info.InitialDirection.normalized * 5f, Color.cyan, 5f);

                    transform.rotation = Quaternion.LookRotation(Info.InitialDirection.normalized, Vector3.up);
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
        switch (Info.CurrentMoveType)
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

        switch (Info.CurrentMoveType)
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
        transform.position += Info.InitialDirection.normalized * _Data.MoveOptions.MovementTypeOptions.LinearSimpleOptions.Speed * Time.deltaTime;
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
                Info.InitialHorizontalDirection.x
                , 0
                , Info.InitialHorizontalDirection.y)
            * Info.LaunchDistance);

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

        Vector3 NextPosition = (ParaS_ImpactTarget - ParaS_InitialPosition).normalized * AnimationCoefficient * Info.LaunchDistance;
        NextPosition.y += _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime.Evaluate(AnimationCoefficient) * _Data.MoveOptions.MovementTypeOptions.ParabolicSimpleOptions.MaxHeight;

        //tinybrain velocity approximation
        if (_Data.MoveOptions.FLAG_FaceRigidbodyVelocity)
        {
            //MovementTypeOptions.ParabolicSimpleOptions.HeightOverTime.



            transform.rotation = Quaternion.LookRotation(Info.InitialHorizontalDirection, Vector3.up);


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
            RB.AddForce(Info.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Speed, ForceMode.VelocityChange);
        }
        else if (_Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.FLAGScaleForceByMass)
        {
            RB.AddForce(Info.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Force * RB.mass, ForceMode.Impulse);
        }
        else
        {
            RB.AddForce(Info.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsImpulseOptions.Force, ForceMode.Impulse);
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

        if (Info.Direction.sqrMagnitude == 0)
        {
            Debug.LogError("Projectile Direction not set!");
        }
    }
    //fixed update
    void PhysicsContinuousForceMovement()
    {
        if (_Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.FLAGScaleForceByMass)
        {
            RB.AddForce(Info.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.ForcePerSecond * RB.mass * Time.fixedDeltaTime, ForceMode.Force);
        }
        else
        {
            RB.AddForce(Info.Direction.normalized * _Data.MoveOptions.MovementTypeOptions.PhysicsContinuousForceOptions.ForcePerSecond * Time.fixedDeltaTime, ForceMode.Force);
        }
    }



    // ----------------------------------------------------------------------
    //members
    Vector3 HSM_CurrentDirection = Vector3.zero;
    //initialization
    void InitHomingSimple()
    {
        RB.isKinematic = true;
        if (Info.Target == null)
        {
            //Debug.LogError("No GameObject assigned!");
            HSM_CurrentDirection = transform.forward.normalized;
        }
        else
        {
            HSM_CurrentDirection = (Info.Target.transform.position - transform.position).normalized;
        }

    }
    //update
    void HomingSimpleMovement()
    {

        if(Info.Target != null)
        {
            Debug.Log(Info.Target);

            HSM_CurrentDirection = Vector3.RotateTowards(
                HSM_CurrentDirection
                , (Info.Target.transform.position - transform.position)
                , Mathf.Deg2Rad * _Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.TurnRate * Time.deltaTime
                , 0);
            Debug.Log(HSM_CurrentDirection);
        }



        if (_Data.MoveOptions.FLAG_FaceRigidbodyVelocity) transform.forward = HSM_CurrentDirection;

        transform.position += HSM_CurrentDirection * _Data.MoveOptions.MovementTypeOptions.HomingSimpleOptions.Speed * Time.deltaTime;
    }
       
    #endregion



    void FaceVelocityForward()
    {
        switch (Info.CurrentMoveType)
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
