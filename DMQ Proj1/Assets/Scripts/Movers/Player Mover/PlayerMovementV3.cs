﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

//TODO: Figure out what data to store in this event args message
public struct PlayerMovementEventArgs
{
    public PlayerMovementEventArgs(string str)
    {
        test = str;
    }
    public string test;
};

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Actor))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(PlayerControls))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementV3 : MonoBehaviour
{
    #region Members
    //Flags
    public bool FLAGCollectDebugTelemetry = false;
    public bool FLAGDisplayDebugGizmos = false;
    public bool FLAG_PlayerMovementEnabled = true;


    //Options
    public PlayerMovementV3Options DashOptions = new PlayerMovementV3Options();
    public PMV3_RunProperties RunOptions = new PMV3_RunProperties();
    [Tooltip("Affects how the rigidbody forces are applied")]
    public MovementModel MovementStyle = MovementModel.ContinuousAdvanced;   
    


    //External objects
    public PlayerInput Input { get; protected set; }
    public Actor AttachedActor { get; protected set; }
    public Inventory inventory { get; protected set; }
    public PlayerControls controls { get; protected set; }
    public Rigidbody RB { get; protected set; }
    

    //State info
    public State CurrentState { get; protected set; }

    private StateInfo _Info;
    private PMV3_DebugTelemetry _DebugInfo;


    //More Private State vars
    Vector2 AimDirection = Vector2.zero;

    Vector2 InputMap = Vector2.zero; //Raw input from input events

    Vector3 DesiredVelocity = Vector3.zero;
    Vector3 AddVelocity = Vector2.zero;

    Vector2 VelocityMap = Vector2.zero; //affects horizontal movement velocity. Controlled via input
    Vector2 DragVector = Vector2.zero;



    //obsolete currently
    [Tooltip("Determines current \"north\" that the InputMap direction is relative to")]
    private GameObject HorizontalMovementAngleHost;


    #region Helpers

    public enum State { Standing, Moving, Sliding, Dashing }
    public enum MovementModel { DebugMovementModel, ContinuousAdvanced, ContinuousSimple, VelocityChange };

    [System.Serializable]
    public class PlayerMovementV3Options
    {
        public float _DashCooldown;
        public AnimationCurve _DashSpeedScale;
        public float _DashDuration = .25f;
        public float _DashSpeed = 30f;
        public ImpactFX.ImpactEffect _DashImpactEffect;
    }

    [System.Serializable]
    public class PMV3_RunProperties
    {
        //Behavior properties (TODO: clean up. Not really using diff accelerations rn)
        //[Header("__MOVEMENT PROPERTIES__")]
        [Range(.1f, 2000f)]
        [Tooltip("Used in Velocity Change movement style ONLY. m / sec^2")]
        public float DampAcceleration = 1;

        [Range(.1f, 2000f)]
        [Tooltip("m / sec^2")]
        public float Deceleration = 20;

        [Range(.1f, 2000f)]
        [Tooltip("m / sec^2")]
        public float HorizontalAcceleration = 100f;

        [Tooltip("Meters / sec")]
        public float MoveSpd = 6;

    }

    private struct StateInfo
    {
        public Utils.CooldownTracker Dash_Cooldown;
        public float Dash_LastStartTime;
        public Vector3 _DashDesiredDirection;
    }
    private struct PMV3_DebugTelemetry
    {
        //Debug members
        public Vector3 LargestVelocityChange;
        public float LargestVelocityChangeMagnitude;
        public Vector3 LargestAddVelocityChange;
        public float LargestAddVelocityChangeMagnitude;
    }

    #endregion


    #endregion

    #region EVENTS

    //TODO: figure out what event arg type(s) to use and why
    public class PlayerMovementEvent : UnityEvent<PlayerMovementEventArgs> { };

    //Not that these events are not static (aka they are instantiated, not global events when invoked)
    [SerializeField] public PlayerMovementEvent Event_AttackStart;
    [SerializeField] public PlayerMovementEvent Event_ChangeWeapon;
    [SerializeField] public PlayerMovementEvent Event_SpecialActionStart;

    void InitializeEvents()
    {
        if (Event_AttackStart == null) Event_AttackStart = new PlayerMovementEvent();
        if (Event_ChangeWeapon == null) Event_ChangeWeapon = new PlayerMovementEvent();
        if (Event_SpecialActionStart == null) Event_SpecialActionStart = new PlayerMovementEvent();
    }

    void AttackEvent()
    {
        ShootProjectile(); //TODO: Remove after testing
        Event_AttackStart?.Invoke(new PlayerMovementEventArgs());
    }
    void ChangeWeaponEvent()
    {
        if (inventory.currentEquipNumber == 0)
        {
            inventory.changeToNumber = -1;
        }
        else
        {
            inventory.changeToNumber = 0;
        }
        if (inventory.currentEquipNumber == 1)
        {
            inventory.changeToNumber = -1;
        }
        else
        {
            inventory.changeToNumber = 1;
        }
        Event_ChangeWeapon?.Invoke(new PlayerMovementEventArgs());
    }
    void SpecialActionEvent()
    {
        if(CurrentState != State.Dashing && _Info.Dash_Cooldown.CanUseCooldown())
        {
            ChangeState(State.Dashing);
            Event_SpecialActionStart?.Invoke(new PlayerMovementEventArgs());
        }
    }

    #endregion

    #region Initialization
    private void Awake()
    {
        AttachedActor = GetComponent<Actor>();
        if (AttachedActor == null) Debug.LogError(ToString() + ": No Actor attached!");

        //Refs
        if (inventory == null) inventory = GetComponent<Inventory>();
        if (inventory == null) Debug.LogError("PlayerMovement: No inventory found");

        if (Input == null) Input = GetComponent<PlayerInput>();
        if (Input == null) Debug.LogError("No Input found");
        
        RB = gameObject.GetComponent<Rigidbody>();
        if (RB == null)
        {
            RB = gameObject.AddComponent<Rigidbody>();
            Debug.LogWarning("MovementSystemV3: No Rigidbody detected. Creating one on this gameobject instead.");
        }

        if (HorizontalMovementAngleHost == null) HorizontalMovementAngleHost = gameObject;


        //Dispatch to other Inits
        InitInput();
        InitializeEvents();


        //init state vars
        CurrentState = State.Moving;

        _Info.Dash_Cooldown = new Utils.CooldownTracker(DashOptions._DashCooldown);
        _Info.Dash_Cooldown.InitializeCooldown();
        _Info.Dash_LastStartTime = Time.time;
    }

    void Start()
    {
        //Event_AttackStart.AddListener(testfunction);
        //Event_AttackStart?.Invoke(new PlayerMovementEventArgs("test"));
    }
    //void testfunction(PlayerMovementEventArgs args)
    //{
    //    Debug.Log(args.test);
    //}


    private void OnEnable()
    {
        Input.enabled = true;
        controls.Enable();
    }
    private void OnDisable()
    {
        Input.enabled = false;
        controls.Disable();
    }
    #endregion

    void Update()
    {
        Debug.DrawRay(transform.position, new Vector3(AimDirection.x, transform.position.y, AimDirection.y) * 5f, Color.yellow);
    }

    private void FixedUpdate()
    {
        if(FLAG_PlayerMovementEnabled) 
        {
            switch(CurrentState)
            {
                case State.Moving:
                    HorizontalMovementInput();
                    break;


                case State.Dashing:
                    DoDashV3();
                    break;


                case State.Sliding: //NYI
                    break;


                case State.Standing: //NYI
                    break;


            }
        }
    }

    #region State Transitions

    // Place State start stuff here
    private void ChangeState(State S)
    {        
        StateEnd(S);

        CurrentState = S;

        switch(S)
        {
            case State.Moving:

                break;


            case State.Dashing:

                _Info._DashDesiredDirection = new Vector3(InputMap.x, 0, InputMap.y).normalized * DashOptions._DashSpeed;
                _Info.Dash_LastStartTime = Time.time;
                _Info.Dash_Cooldown.ConsumeCooldown();

                break;


            case State.Sliding:

                break;


            case State.Standing:

                break;


            default:
                Debug.LogError("ChangeState(): unrecognized state: " + S.ToString() + ". Does an implementation exist?");
                break;
        }
    }

    // Place State End stuff here
    private void StateEnd(State S)
    {

        switch (S)
        {
            case State.Moving:

                break;


            case State.Dashing:

                break;


            case State.Sliding:

                break;


            case State.Standing:

                break;


            default:
                Debug.LogError("StateEnd(): unrecognized state: " + S.ToString() + ". Does an implementation exist?");
                break;
        }
    }

    #endregion

    #region Input Event Dispatcher
    // This function is called in Awake(), and creates controls 
    // + registers all the events that may occur due to player input
    private void InitInput()
    {

        controls = new PlayerControls();

        //set up action map
        if(Input.currentControlScheme == controls.MouseAndKeyboardScheme.name)
        {
            Input.SwitchCurrentActionMap(controls.MouseAndKeyboardScheme.name);
        }
        else if (Input.currentControlScheme == controls.GamepadScheme.name)
        {
            Input.SwitchCurrentActionMap(controls.GamepadScheme.name);
        }

        //please someone find a better way to do this (and retain multiplayer functionality)
        Input.onActionTriggered += ctx =>
        {
            ////MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
            if (ctx.action.actionMap.name == controls.MouseAndKeyboardScheme.name)
            {
                if (ctx.performed)
                {
                    //MnK
                    if (ctx.action.name == controls.MouseAndKeyboard.Movement.name) InputMap = ctx.ReadValue<Vector2>();

                    else if (ctx.action.name == controls.MouseAndKeyboard.Attack.name) AttackEvent();

                    else if (ctx.action.name == controls.MouseAndKeyboard.SpecialAction.name) SpecialActionEvent();

                    else if (ctx.action.name == controls.MouseAndKeyboard.Wepon1Equip.name) ChangeWeaponEvent();

                    else if (ctx.action.name == controls.MouseAndKeyboard.Wepon2Equip.name) ChangeWeaponEvent();

                    else if (ctx.action.name == controls.MouseAndKeyboard.Aim.name)
                    {
                        if (Camera.main == null) return;

                        Vector2 In = ctx.ReadValue<Vector2>();

                        //improved aiming using raycast to plane
                        //TODO: Consider a "raycast to model" approach; consider modifying the CursorPlane to be inline with the projectile spawn height
                        if (Camera.main.GetComponent<Topdown_Multitracking_Camera_Rig>() != null)
                        {

                            //TODO: Consider CamDistanceCurrent improvement. We need a distance from camera to EACH PLAYER, projected onto "2d world plane." 
                            //For now this should be a fairly strong approximation (but maybe could be broken)
                            Plane CursorPlane = new Plane(Vector3.up, Camera.main.GetComponent<Topdown_Multitracking_Camera_Rig>().CamDistanceCurrent);

                            Ray RayToCursorPlane = Camera.main.ScreenPointToRay(new Vector3(In.x, In.y, 0));


                            Vector3 Hit = Vector3.zero;
                            if (CursorPlane.Raycast(RayToCursorPlane, out float Point))
                            {
                                Hit = RayToCursorPlane.GetPoint(Point);
                            }

                            //TOOD: Consider this for relaying info to other subsystems
                            //At this point, Hit == the point on plane where player clicked. Could be useful??

                            Vector3 V = Vector3.ProjectOnPlane((Hit - transform.position), Vector3.up);

                            AimDirection = (new Vector2(V.x, V.z)).normalized;
                        }
                        else
                        {
                            Vector3 V = Camera.main.WorldToScreenPoint(transform.position);
                            //Vector3 V = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, ShootableProjectileHeightOffset, 0)); //hmm 

                            AimDirection = (In - new Vector2(V.x, V.y)).normalized;
                        }
                    }
                }


                else if (ctx.canceled)
                {
                    //MnK
                    if (ctx.action.name == controls.MouseAndKeyboard.Movement.name) InputMap = Vector2.zero;
                }
            }



            ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
            else if (ctx.action.actionMap.name == controls.GamepadScheme.name)
            {
                if (ctx.performed)
                {
                    //Gamepad
                    if (ctx.action.name == controls.Gamepad.Movement.name) InputMap = ctx.ReadValue<Vector2>();

                    else if (ctx.action.name == controls.Gamepad.Attack.name) AttackEvent();

                    else if (ctx.action.name == controls.Gamepad.SpecialAction.name) SpecialActionEvent();

                    else if (ctx.action.name == controls.Gamepad.Wepon1Equip.name) ChangeWeaponEvent();

                    else if (ctx.action.name == controls.Gamepad.Wepon2Equip.name) ChangeWeaponEvent();

                    else if (ctx.action.name == controls.Gamepad.Aim.name) AimDirection = ctx.ReadValue<Vector2>().normalized;
                }


                else if (ctx.canceled)
                {
                    //Gamepad
                    if (ctx.action.name == controls.Gamepad.Movement.name) InputMap = Vector2.zero;
                }
            }
        };


        ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
        ////register reading movement values from input
        //controls.Gamepad.Movement.performed += ctx => InputMap = ctx.ReadValue<Vector2>();
        //controls.Gamepad.Movement.canceled += ctx => InputMap = Vector2.zero;
        //controls.Gamepad.Attack.performed += ctx => AttackEvent();
        //controls.Gamepad.SpecialAction.performed += ctx => SpecialActionEvent();
        //controls.Gamepad.Wepon1Equip.performed += ctx => ChangeWeaponEvent();
        //controls.Gamepad.Wepon2Equip.performed += ctx => ChangeWeaponEvent();

        //controls.Gamepad.Aim.performed += ctx => AimDirection = ctx.ReadValue<Vector2>().normalized;
        ////controls.Gamepad.Aim.canceled += ctx => AimDirection = Vector2.zero;

        ////MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
        ////register reading movement values from input
        //controls.MouseAndKeyboard.Movement.performed += ctx => InputMap = ctx.ReadValue<Vector2>();
        //controls.MouseAndKeyboard.Movement.canceled += ctx => InputMap = Vector2.zero;
        //controls.MouseAndKeyboard.Attack.performed += ctx => AttackEvent();
        //controls.MouseAndKeyboard.SpecialAction.performed += ctx => SpecialActionEvent();
        //controls.MouseAndKeyboard.Wepon1Equip.performed += ctx => ChangeWeaponEvent();
        //controls.MouseAndKeyboard.Wepon2Equip.performed += ctx => ChangeWeaponEvent();

        ////Convert to screen space to achieve direction
        //controls.MouseAndKeyboard.Aim.performed += ctx =>
        //{
        //    Vector3 V = Camera.main.WorldToScreenPoint(transform.position);
        //    AimDirection = (ctx.ReadValue<Vector2>() - new Vector2(V.x, V.y)).normalized;
        //};
        ////controls.MouseAndKeyboard.Aim.canceled += ctx => AimDirection = Vector2.zero;

    }
    #endregion

    #region Jump Movement Models
    private void JumpInput()
    {
        StopCoroutine(JumpStart());
        StartCoroutine(JumpStart());
    }
    public IEnumerator JumpNormal(float duration)
    {
        float StartTime = Time.time;
        float LastCurTime = Time.time;
        float CurTime = Time.time;
        while (Mathf.Abs(CurTime - StartTime) < duration)
        {
            LastCurTime = CurTime;
            CurTime = Time.time;
            RB.AddForce(HorizontalMovementAngleHost.transform.forward * 50 * RB.mass * (Mathf.Abs(CurTime - LastCurTime) / duration), ForceMode.Force);

            yield return new WaitForFixedUpdate();
        }

    }
    public IEnumerator JumpStart()
    {
        yield return new WaitForFixedUpdate();

        float JumpHeight = 3f; //TODO: Do we need a jump routine???

        float JumpSpeed = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
        RB.AddForce(JumpSpeed * Vector3.up, ForceMode.VelocityChange);
    }
    #endregion
    
    #region Horizontal Movement Models


    //This method is called in FixedUpdate()
    private void HorizontalMovementInput()
    {
        //In case we ever want to change movement models, MovementStyle is implemented as a public, alterable property using Unity inspector

        switch (MovementStyle)
        {
            case MovementModel.ContinuousAdvanced:
                ContinuousMovementModel();
                break;
            case MovementModel.ContinuousSimple:
                ContinuousMovementModelV2();
                break;
            case MovementModel.VelocityChange:
                VelocityChangeModel();
                break;
            case MovementModel.DebugMovementModel:
                DebugMovementModel();
                break;
            default:
                DebugMovementModel();
                break;
        };
    }


    void DebugMovementModel()
    {

    }


    void ContinuousMovementModel()
    {
        //F = ma = mv/t
        //Cache m/t (we compute v later)
        float ForceCoefficient = RB.mass / Time.fixedDeltaTime;

        Vector3 FilteredRBVelocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);

        //TODO: Improve: Clamp maximum allowable velocity gains based on some Acceleration

        //Create direction of movement desired by player
        Vector3 InputDirection = new Vector3(InputMap.x, 0, InputMap.y); //TODO: processing based on horizontal angle host
        {
            float AngleDiff = Vector3.SignedAngle(FilteredRBVelocity.normalized, InputDirection, Vector3.up);

            //TODO: Consider adding these to movement properties (global scope)
            float MIN_ANGLE_TOLERANCE = 40.0f; //Very important. Controls the arc of player slide control when turning. Stay below 90!
            float MAX_ANGLE_TOLERANCE = 158.4f; //Less important. Controls the angle tolerance for braking. Stay above 90, and stay above MIN!

            //Preprocessing input. Clamp to within 180deg if we are going beyond standard movement speed
            //TODO: verify angle computation is correct in all cases
            if (
                FilteredRBVelocity.sqrMagnitude != 0 
                && (FilteredRBVelocity.sqrMagnitude > RunOptions.MoveSpd * RunOptions.MoveSpd) 
                && Mathf.Abs(AngleDiff) > MIN_ANGLE_TOLERANCE 
                && Mathf.Abs(AngleDiff) < MAX_ANGLE_TOLERANCE
                ) 
            {
                float AngleDelta = 0;
                if (AngleDiff > 0)  AngleDelta = -(MIN_ANGLE_TOLERANCE - Mathf.Abs(AngleDiff));
                else                AngleDelta = (MIN_ANGLE_TOLERANCE - Mathf.Abs(AngleDiff));
                
                float SinD, CosD;
                SinD = Mathf.Sin(Mathf.Deg2Rad * AngleDelta);
                CosD = Mathf.Cos(Mathf.Deg2Rad * AngleDelta);

                //reassign clamped value
                InputDirection = new Vector3(
                    (CosD * InputDirection.x - SinD * InputDirection.z)
                    , InputDirection.y
                    , (SinD * InputDirection.x + CosD * InputDirection.z)
                    );

                if(FLAGDisplayDebugGizmos)
                {
                    float a = 5;
                    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), InputDirection * a, Color.yellow);
                }
            }
        }
        //Naive model
        //if(InputDirection.sqrMagnitude > 1) InputDirection = InputDirection.normalized;
        //DesiredVelocity = Vector3.zero; if (InputDirection.sqrMagnitude > 0) DesiredVelocity = InputDirection * MoveSpd;

        //If a direction exists, accelerate towards it. otherwise decelerate towards 0
        if (InputDirection.sqrMagnitude == 0)
        {
            //apply the brakes

            //for comparison to make sure we dont go in the opposite direction
            Vector3 Temp = DesiredVelocity;

            DesiredVelocity = DesiredVelocity - (DesiredVelocity.normalized * RunOptions.HorizontalAcceleration * Time.fixedDeltaTime);

            //clamp to 0
            if (Vector3.Dot(DesiredVelocity, Temp) < 0) DesiredVelocity = Vector3.zero;
        }
        else
        {
            //accel
            DesiredVelocity = DesiredVelocity + InputDirection.normalized * RunOptions.HorizontalAcceleration * Time.fixedDeltaTime;

            //clamp desired velocity max to move speed max
            if (DesiredVelocity.sqrMagnitude > RunOptions.MoveSpd * RunOptions.MoveSpd)
                DesiredVelocity = DesiredVelocity.normalized * RunOptions.MoveSpd;

        }




        Vector3 DesiredVelocityDiff = Vector3.zero;

        //Filtered velocity represents the maximum velocity we can affect in this timestep (the rigidbody can exceed this by a LOT in normal gameplay)
        Vector3 FilteredVelocity = FilteredRBVelocity;
        //max is clamped to current player-desired velocity max.
        if (FilteredVelocity.sqrMagnitude > DesiredVelocity.sqrMagnitude) FilteredVelocity = DesiredVelocity.magnitude * FilteredVelocity.normalized;

        //compute parallel component of current velocity to desired velocity
        if (DesiredVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
            DesiredVelocityDiff = (Vector3.Dot(DesiredVelocity, FilteredVelocity) / DesiredVelocity.magnitude) * DesiredVelocity.normalized;

        //Recall that we are comparing parallel vectors here
        if(Vector3.Dot(DesiredVelocity, FilteredRBVelocity) < 0)
        {
            //decelerating. Parallel component of velocity can be at MOST Deceleration * Time.FixedDeltaTime            
            Vector3 Parallel = Vector3.zero;
            Vector3 Perpendicular = Vector3.zero;

            Parallel = (Vector3.Dot(DesiredVelocity, FilteredVelocity) / FilteredVelocity.magnitude) * FilteredVelocity.normalized;
            //Perpendicular = DesiredVelocity - Parallel;

            AddVelocity = (Parallel.normalized * RunOptions.Deceleration * Time.fixedDeltaTime) + Perpendicular;
            //AddVelocity = ((-Parallel.normalized * Deceleration * Time.fixedDeltaTime) + Perpendicular);


            //AddVelocity = (DesiredVelocity - DesiredVelocityDiff);
            //if (AddVelocity.sqrMagnitude > Deceleration * Deceleration * Time.fixedDeltaTime * Time.fixedDeltaTime) AddVelocity = AddVelocity.normalized * Deceleration * Time.fixedDeltaTime;
        }
        else
        {
            //sustaining velocity
            AddVelocity = (DesiredVelocity - DesiredVelocityDiff);
        }


        //Damp (within standard movement speed control)
        if (/*InputDirection.sqrMagnitude > 0 && */FilteredRBVelocity.sqrMagnitude <= RunOptions.MoveSpd * RunOptions.MoveSpd) //First check was causing sloppy deceleration within standard MoveSpd
        {
            RB.AddForce(AddVelocity * ForceCoefficient, ForceMode.Force); //TODO: Consider projecting this onto surface normal of whatever we're standing on (for movement along slopes)


            if ((AddVelocity).sqrMagnitude < (FilteredRBVelocity - DesiredVelocityDiff).sqrMagnitude)
            {
                //RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff).normalized * AddVelocity.magnitude * ForceCoefficient, ForceMode.Force);
                RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }
            else
            {
                RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }


            if (FLAGDisplayDebugGizmos)
            {
                float a = 2;
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .75f, transform.position.z), -(FilteredRBVelocity - DesiredVelocityDiff) * a, Color.red);
            }
        }
        //Damp (exceeding movement speed max)
        else
        {
            //"Parachute" idea: Add the velocity * forceCoefficient, but pull "backward" on the rigidbody by the amount needed to maintain current velocity (a direction change, but not a velocity one)

            Vector3 ModifiedVelocity = AddVelocity + FilteredRBVelocity;

            Vector3 ParachuteVelocity = Vector3.zero;
            //if (ModifiedVelocity.sqrMagnitude > FilteredRBVelocity.sqrMagnitude) ParachuteVelocity = -(FilteredRBVelocity - ModifiedVelocity).magnitude * ModifiedVelocity.normalized;
            if (ModifiedVelocity.sqrMagnitude > FilteredRBVelocity.sqrMagnitude) ParachuteVelocity = Mathf.Abs(ModifiedVelocity.magnitude - FilteredRBVelocity.magnitude) * -ModifiedVelocity.normalized; //TODO: Optimize this!

            RB.AddForce((AddVelocity + ParachuteVelocity) * ForceCoefficient, ForceMode.Force); //TODO: Consider projecting this onto surface normal of whatever we're standing on (for movement along slopes)


            if (FLAGDisplayDebugGizmos)
            {
                float a = 2;
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .75f, transform.position.z), ParachuteVelocity * a, Color.green);
            }
        }





        // for debug
        if (FLAGDisplayDebugGizmos)
        {
            float a = 2;
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), AddVelocity * a, Color.cyan);
            //Debug.DrawRay(transform.position, (FilteredRBVelocity + AddVelocity) * a, Color.magenta);
            Debug.DrawRay(transform.position, new Vector3(RB.velocity.x, RB.velocity.y + .1f, RB.velocity.z) * a, Color.white);
        }
        if (FLAGCollectDebugTelemetry)
        {
            //OBSERVATION: We never alter more than MoveSpd's worth of velocity 
            if ((-(FilteredRBVelocity - DesiredVelocityDiff) + AddVelocity).sqrMagnitude > _DebugInfo.LargestVelocityChangeMagnitude * _DebugInfo.LargestVelocityChangeMagnitude)
            {
                _DebugInfo.LargestVelocityChange = (-(FilteredRBVelocity - DesiredVelocityDiff) + AddVelocity);
                _DebugInfo.LargestVelocityChangeMagnitude = (-(FilteredRBVelocity - DesiredVelocityDiff) + AddVelocity).magnitude;
            }
            if ((AddVelocity).sqrMagnitude > _DebugInfo.LargestAddVelocityChangeMagnitude * _DebugInfo.LargestAddVelocityChangeMagnitude)
            {
                _DebugInfo.LargestAddVelocityChange = AddVelocity;
                _DebugInfo.LargestAddVelocityChangeMagnitude = (AddVelocity).magnitude;
            }
        }
    }


    void ContinuousMovementModelV2()
    {

        //float MoveSpd = 10f;
        //float HorizontalAcceleration = 100f;

        float ForceCoefficient = RB.mass / Time.fixedDeltaTime;

        Vector3 InputDirection = new Vector3(InputMap.x, 0, InputMap.y); //TODO: processing based on horizontal angle host
        if (InputDirection.sqrMagnitude == 0)
        {
            //apply the brakes
            Vector3 Temp = DesiredVelocity;

            DesiredVelocity = DesiredVelocity - (DesiredVelocity.normalized * RunOptions.HorizontalAcceleration * Time.fixedDeltaTime);
            //if (Vector3.Angle(DesiredVelocity, Temp) > 60) DesiredVelocity = Vector3.zero;
            if (Vector3.Dot(DesiredVelocity, Temp) < 0) DesiredVelocity = Vector3.zero;
        }
        else
        {
            //accel
            DesiredVelocity = DesiredVelocity + new Vector3(InputMap.x, 0, InputMap.y).normalized * RunOptions.HorizontalAcceleration * Time.fixedDeltaTime;
        }



        if (DesiredVelocity.sqrMagnitude > RunOptions.MoveSpd * RunOptions.MoveSpd)
            DesiredVelocity = DesiredVelocity.normalized * RunOptions.MoveSpd;


        Vector3 DesiredVelocityDiff = Vector3.zero;
        if (DesiredVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
            DesiredVelocityDiff = (Vector3.Dot(DesiredVelocity, RB.velocity) / DesiredVelocity.magnitude) * DesiredVelocity.normalized;


        AddVelocity = (DesiredVelocity - DesiredVelocityDiff);

        // Damp ////////////////////////////////////////////////
        //Vector3 ForceDamp = Vector3.zero;
        //if ((AddVelocity + RB.velocity).sqrMagnitude > MoveSpd * MoveSpd)
        //{
        //    ForceDamp = -RB.velocity.normalized * HorizontalAcceleration * RB.mass;
        //    if (ForceDamp.sqrMagnitude > RB.velocity.sqrMagnitude)
        //        RB.AddForce(-RB.velocity, ForceMode.Force);
        //    else
        //        RB.AddForce(ForceDamp, ForceMode.Force);
        //}
        ////////////////////////////////////////////////


        ////////////////////////////////////////////////
        //DragVector = Vector3.zero;
        ////kill all stuff not in the dir we wanna go
        //if (InputDirection.sqrMagnitude > 0)
        //{
        //    DragVector = Vector2.Perpendicular(InputDirection);
        //}
        //else
        //{
        //    DragVector = -1 * AddVelocity;
        //}
        //DragVector.Normalize();

        //DragVector *= Vector2.Dot(DragVector, -1 * AddVelocity) * Time.fixedDeltaTime * DampAcceleration;
        //if (DragVector.sqrMagnitude > AddVelocity.sqrMagnitude)
        //{
        //    DragVector = Vector2.zero;
        //    AddVelocity = InputDirection.normalized * AddVelocity.magnitude * Vector2.Dot(AddVelocity, InputDirection);
        //}

        //apply additional velocity
        //ProcessedInputMap *= HorizontalAcceleration * Time.fixedDeltaTime;
        //VelocityMap += ProcessedInputMap * HorizontalAcceleration * Time.fixedDeltaTime;
        //AddVelocity += new Vector3(DragVector.x, 0, DragVector.y);
        //////////////////////////////////////////////
        ///
        RB.AddForce(AddVelocity * ForceCoefficient, ForceMode.Force);

        if (InputDirection.sqrMagnitude > 0)
        {
            if ((AddVelocity).sqrMagnitude < (RB.velocity - DesiredVelocityDiff).sqrMagnitude)
            {
                //RB.AddForce(-(RB.velocity - DesiredVelocityDiff).normalized * AddVelocity.magnitude * ForceCoefficient, ForceMode.Force);
                RB.AddForce(-(RB.velocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }
            else
            {
                RB.AddForce(-(RB.velocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }
        }

        // for testing
        if (FLAGDisplayDebugGizmos)
        {
            float a = 2;
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), AddVelocity * a, Color.cyan);
            Debug.DrawRay(transform.position, (RB.velocity + AddVelocity) * a, Color.magenta);
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .75f, transform.position.z), -(RB.velocity - DesiredVelocityDiff) * a, Color.red);
            Debug.DrawRay(transform.position, new Vector3(RB.velocity.x, RB.velocity.y + .1f, RB.velocity.z) * a, Color.gray);
        }
    }


    void VelocityChangeModel()
    {
        //Velocity Change Model
        Vector3 RetainedVelocity = new Vector3(0, RB.velocity.y, 0);

        //Vector3 ConstructionVector1 = Vector3.zero;
        //Vector3 ConstructionVector2 = Vector3.zero;
        //ConstructionVector2 = HorizontalMovementAngleHost.transform.right * InputStrafe;
        //ConstructionVector1 = HorizontalMovementAngleHost.transform.forward * InputFront;

        //ProcessedInputMap.x = ConstructionVector1.x + ConstructionVector2.x;
        //ProcessedInputMap.y = ConstructionVector1.z + ConstructionVector2.z;

        Vector2 ProcessedInputMap = InputMap;
        if (ProcessedInputMap.sqrMagnitude > 1)
        {
            ProcessedInputMap.Normalize();
        }

        DragVector = Vector3.zero;
        //kill all stuff not in the dir we wanna go
        if (ProcessedInputMap.x != 0 || ProcessedInputMap.y != 0)
        {
            DragVector = Vector2.Perpendicular(ProcessedInputMap);
        }
        else
        {
            DragVector = -1 * VelocityMap;
        }
        DragVector.Normalize();

        DragVector *= Vector2.Dot(DragVector, -1 * VelocityMap) * Time.fixedDeltaTime * RunOptions.DampAcceleration;
        if (DragVector.sqrMagnitude > VelocityMap.sqrMagnitude)
        {
            DragVector = Vector2.zero;
            VelocityMap = ProcessedInputMap.normalized * VelocityMap.magnitude * Vector2.Dot(VelocityMap, ProcessedInputMap);
        }

        //apply additional velocity
        //ProcessedInputMap *= HorizontalAcceleration * Time.fixedDeltaTime;
        VelocityMap += ProcessedInputMap * RunOptions.HorizontalAcceleration * Time.fixedDeltaTime;
        VelocityMap += DragVector;


        if (VelocityMap.sqrMagnitude > RunOptions.MoveSpd * RunOptions.MoveSpd * 4)
        {
            VelocityMap = VelocityMap.normalized * RunOptions.MoveSpd * 2; //max spd clamp
        }

        if (FLAGDisplayDebugGizmos)
        {
            Debug.DrawRay(gameObject.transform.position, new Vector3(DragVector.x, 0, DragVector.y), Color.magenta);
            Debug.DrawRay(gameObject.transform.position, new Vector3(ProcessedInputMap.x * 1.5f, .6f, ProcessedInputMap.y * 1.5f), Color.green);
            Debug.DrawRay(gameObject.transform.position, new Vector3(VelocityMap.x, 0, VelocityMap.y), Color.cyan);
        }

        Vector3 VelocityMap3 = new Vector3(VelocityMap.x, 0, VelocityMap.y);
        VelocityMap3 += RetainedVelocity;

        //if ((RB.velocity + VelocityMap3).sqrMagnitude > MoveSpd * MoveSpd)

        RB.AddForce(VelocityMap3 - RB.velocity, ForceMode.VelocityChange);

        //RB.velocity = new Vector3((VelocityMap3 - RB.velocity).x, RB.velocity.y, (VelocityMap3 - RB.velocity).z);

        //Vector3 Displacement = new Vector3(VelocityMap.x, 0, VelocityMap.y) * Time.fixedDeltaTime;

        //RB.MovePosition( RB.position + Displacement );
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        //No rigidbody means we should just slap the wall or whatever
        if(collision.rigidbody == null)
        {
            //IFX
            if (DashOptions._DashImpactEffect != null)
            {
                DashOptions._DashImpactEffect.SpawnImpactEffect(null, collision.contacts[0].point, collision.contacts[0].normal);
            }
            ChangeState(State.Moving);
        }

        //TODO: Consider multiple styles of collision handling
        else if (CurrentState == State.Dashing && collision.rigidbody.mass >= RB.mass)
        {
            //IFX
            if(DashOptions._DashImpactEffect != null)
            {
                DashOptions._DashImpactEffect.SpawnImpactEffect(null, collision.contacts[0].point, collision.contacts[0].normal);
            }

            //impart physic impulse. Cancel remaining dash
            if(collision.rigidbody != null)
            {
                RB.AddForce(-RB.velocity * RB.mass, ForceMode.Impulse);
                collision.rigidbody.AddForce(RB.velocity * RB.mass, ForceMode.Impulse);
            }

            ChangeState(State.Moving);
        }
    }

    #region Dash Movement Models



    private void DoDashV3()
    {
        if (FLAGCollectDebugTelemetry) Debug.Log("Dashing");
                       

        if (Mathf.Abs(Time.time - _Info.Dash_LastStartTime) < Mathf.Abs(DashOptions._DashDuration))
        {
            Vector3 DesiredVelocityDiff = Vector3.zero;
            Vector3 DashAddVelocity = Vector3.zero;
            float DashSpeedCoef = DashOptions._DashSpeedScale.Evaluate((Time.time - _Info.Dash_LastStartTime) / DashOptions._DashDuration);
            Vector3 DashDesiredVelocity = _Info._DashDesiredDirection * DashSpeedCoef;
            float ForceCoefficient = RB.mass / Time.fixedDeltaTime;
            Vector3 FilteredRBVelocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);
            //Filtered velocity represents the maximum velocity we can affect in this timestep (the rigidbody can exceed this by a LOT in normal gameplay)
            Vector3 FilteredVelocity = FilteredRBVelocity;

            //max is clamped to current player-desired velocity max.
            if (FilteredVelocity.sqrMagnitude > DashDesiredVelocity.sqrMagnitude) FilteredVelocity = DashDesiredVelocity.magnitude * FilteredVelocity.normalized;
            

            //compute parallel component of current velocity to desired velocity
            if (DashDesiredVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
                DesiredVelocityDiff = (Vector3.Dot(DashDesiredVelocity, FilteredVelocity) / DashDesiredVelocity.magnitude) * DashDesiredVelocity.normalized;

            //Recall that we are comparing parallel vectors here
            if (Vector3.Dot(DashDesiredVelocity, FilteredRBVelocity) < 0)
            {
                //decelerating. Parallel component of velocity can be at MOST Deceleration * Time.FixedDeltaTime            
                Vector3 Parallel = Vector3.zero;
                Vector3 Perpendicular = Vector3.zero;

                Parallel = (Vector3.Dot(DashDesiredVelocity, FilteredVelocity) / FilteredVelocity.magnitude) * FilteredVelocity.normalized;
                //Perpendicular = DashDesiredVelocity - Parallel;

                DashAddVelocity = (Parallel.normalized * RunOptions.Deceleration * Time.fixedDeltaTime) + Perpendicular;
                //DashAddVelocity = ((-Parallel.normalized * Deceleration * Time.fixedDeltaTime) + Perpendicular);


                //DashAddVelocity = (DashDesiredVelocity - DesiredVelocityDiff);
                //if (DashAddVelocity.sqrMagnitude > Deceleration * Deceleration * Time.fixedDeltaTime * Time.fixedDeltaTime) DashAddVelocity = DashAddVelocity.normalized * Deceleration * Time.fixedDeltaTime;
            }
            else
            {
                //sustaining velocity
                DashAddVelocity = (DashDesiredVelocity - DesiredVelocityDiff);
            }

            //TODO: Figure out + impl best practice for dash Dampening
            //Damp (within standard movement speed control)
            if (/*InputDirection.sqrMagnitude > 0 && */FilteredRBVelocity.sqrMagnitude <= RunOptions.MoveSpd * RunOptions.MoveSpd || true) //First check was causing sloppy deceleration within standard MoveSpd
            {
                RB.AddForce(DashAddVelocity * ForceCoefficient, ForceMode.Force); //TODO: Consider projecting this onto surface normal of whatever we're standing on (for movement along slopes)


                if ((DashAddVelocity).sqrMagnitude < (FilteredRBVelocity - DesiredVelocityDiff).sqrMagnitude)
                {
                    //RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff).normalized * DashAddVelocity.magnitude * ForceCoefficient, ForceMode.Force);
                    RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
                }
                else
                {
                    RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
                }


                if (FLAGDisplayDebugGizmos)
                {
                    float a = 2;
                    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .75f, transform.position.z), -(FilteredRBVelocity - DesiredVelocityDiff) * a, Color.red);
                }
            }

        }
        else
        {
            ChangeState(State.Moving);
        }

    }

    //Old Impl
    private IEnumerator DoDashV2()
    {
        ChangeState(State.Dashing);

        yield return new WaitForFixedUpdate();


        Vector3 DesiredVelocityDiff = Vector3.zero;
        float ForceCoefficient = RB.mass / Time.fixedDeltaTime;

        Vector3 FilteredRBVelocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);

        Vector3 DashDesiredVelocityRaw = new Vector3(InputMap.x, 0, InputMap.y).normalized * DashOptions._DashSpeed;
        Vector3 DashDesiredVelocity = DashDesiredVelocityRaw;
        Vector3 DashAddVelocity = Vector3.zero;

        //Filtered velocity represents the maximum velocity we can affect in this timestep (the rigidbody can exceed this by a LOT in normal gameplay)
        Vector3 FilteredVelocity = FilteredRBVelocity;

        float DashSpeedCoef = 0;

        float TimeStart = Time.time;
        float TimeCurrent = Time.time;
        while(TimeCurrent - TimeStart < DashOptions._DashDuration)
        {
            TimeCurrent = Time.time;
            DashSpeedCoef = DashOptions._DashSpeedScale.Evaluate((TimeCurrent - TimeStart) / DashOptions._DashDuration);
            DashDesiredVelocity = DashDesiredVelocityRaw * DashSpeedCoef;

            ForceCoefficient = RB.mass / Time.fixedDeltaTime;

            FilteredRBVelocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);

            FilteredVelocity = FilteredRBVelocity;
            //max is clamped to current player-desired velocity max.
            if (FilteredVelocity.sqrMagnitude > DashDesiredVelocity.sqrMagnitude) FilteredVelocity = DashDesiredVelocity.magnitude * FilteredVelocity.normalized;



            //compute parallel component of current velocity to desired velocity
            if (DashDesiredVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
                DesiredVelocityDiff = (Vector3.Dot(DashDesiredVelocity, FilteredVelocity) / DashDesiredVelocity.magnitude) * DashDesiredVelocity.normalized;

            //Recall that we are comparing parallel vectors here
            if (Vector3.Dot(DashDesiredVelocity, FilteredRBVelocity) < 0)
            {
                //decelerating. Parallel component of velocity can be at MOST Deceleration * Time.FixedDeltaTime            
                Vector3 Parallel = Vector3.zero;
                Vector3 Perpendicular = Vector3.zero;

                Parallel = (Vector3.Dot(DashDesiredVelocity, FilteredVelocity) / FilteredVelocity.magnitude) * FilteredVelocity.normalized;
                //Perpendicular = DashDesiredVelocity - Parallel;

                DashAddVelocity = (Parallel.normalized * RunOptions.Deceleration * Time.fixedDeltaTime) + Perpendicular;
                //DashAddVelocity = ((-Parallel.normalized * Deceleration * Time.fixedDeltaTime) + Perpendicular);


                //DashAddVelocity = (DashDesiredVelocity - DesiredVelocityDiff);
                //if (DashAddVelocity.sqrMagnitude > Deceleration * Deceleration * Time.fixedDeltaTime * Time.fixedDeltaTime) DashAddVelocity = DashAddVelocity.normalized * Deceleration * Time.fixedDeltaTime;
            }
            else
            {
                //sustaining velocity
                DashAddVelocity = (DashDesiredVelocity - DesiredVelocityDiff);
            }

            //TODO: Figure out + impl best practice for dash Dampening
            //Damp (within standard movement speed control)
            if (/*InputDirection.sqrMagnitude > 0 && */FilteredRBVelocity.sqrMagnitude <= RunOptions.MoveSpd * RunOptions.MoveSpd || true) //First check was causing sloppy deceleration within standard MoveSpd
            {
                RB.AddForce(DashAddVelocity * ForceCoefficient, ForceMode.Force); //TODO: Consider projecting this onto surface normal of whatever we're standing on (for movement along slopes)


                if ((DashAddVelocity).sqrMagnitude < (FilteredRBVelocity - DesiredVelocityDiff).sqrMagnitude)
                {
                    //RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff).normalized * DashAddVelocity.magnitude * ForceCoefficient, ForceMode.Force);
                    RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
                }
                else
                {
                    RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
                }


                if (FLAGDisplayDebugGizmos)
                {
                    float a = 2;
                    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .75f, transform.position.z), -(FilteredRBVelocity - DesiredVelocityDiff) * a, Color.red);
                }
            }

            yield return new WaitForFixedUpdate();
        }


        ChangeState(State.Moving);
    }

    //Old Impl
    private IEnumerator DoDash()
    {
        //Simple impulse model
        float ImpulseForce = 10f;
        RB.AddForce(new Vector3(InputMap.x, 0, InputMap.y).normalized * ImpulseForce * RB.mass, ForceMode.Impulse);
        yield return new WaitForFixedUpdate();
    }

    #endregion

    //TODO: Move this to another script
    public GameObject ShootableProjectile;
    public float ShootableProjectileHeightOffset = 1f;
    void ShootProjectile()
    {
        GenericProjectile P_Template = ShootableProjectile.GetComponent<GenericProjectile>();
        if (ShootableProjectile != null && P_Template != null)
        {
            Vector3 AimDir3 = new Vector3(AimDirection.x, 0, AimDirection.y);
            
            GameObject P = Instantiate(ShootableProjectile);
            GenericProjectile Proj = P.GetComponent<GenericProjectile>();

            if (AttachedActor != null)
            {
                Proj.ActorOwner = AttachedActor; //TODO: get better solution...
                Proj.gameObject.layer = AttachedActor._Team.Options.Layer;
            }
            else Debug.LogError(this.ToString() + ": No Actor attached");


            P.transform.position = transform.position + (AimDir3).normalized * 1.5f + new Vector3(0, ShootableProjectileHeightOffset, 0); //arbitrary spawn loc
            Proj.Mover.MovementTypeOptions.PhysicsImpulseOptions.Direction = AimDir3.normalized;
        }
    }
}
