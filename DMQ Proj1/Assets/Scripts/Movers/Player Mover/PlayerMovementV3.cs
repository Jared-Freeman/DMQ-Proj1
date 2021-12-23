using System.Collections;
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

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementV3 : MonoBehaviour
{
    #region Members
    //Flags
    public bool FLAGCollectDebugTelemetry = false;
    public bool FLAGDisplayDebugGizmos = false;

    //Behavior properties (TODO: clean up. Not really using diff accelerations rn)
    [Range(.1f, 5000f)] [Tooltip("m / sec^2")]
    public float FrontAcceleration = 1;
    [Range(.1f, 5000f)] [Tooltip("m / sec^2")]
    public float DampAcceleration = 1;
    [Range(.1f, 5000f)] [Tooltip("m / sec^2")]
    public float Deceleration = 20;
    [Tooltip("Meters / sec")]
    public float MoveSpd = 1;
    [Tooltip("Affects how the rigidbody forces are applied")]
    public MovementModel MovementStyle = MovementModel.ContinuousAdvanced;

    //TODO: impl this stuff
    [Header("Currently Obsolete")]
    [Tooltip("1.5x true value seems to be ideal rn")]
    public float JumpHeight = 1;
    [Tooltip("Determines current \"north\" that the InputMap direction is relative to")]
    public GameObject HorizontalMovementAngleHost;
    [Tooltip("Scaled by Mass")] public float AddedDownwardForce = 1;


    //External objects
    [Space(10)]
    [Header("External Object Refs")]
    public PlayerInput Input;
    [SerializeField] private Inventory inventory;
    private PlayerControls controls;
    [SerializeField] private Rigidbody RB;

    //Horizontal movement state vectors (TODO: clean up)
    [Space(10)]
    [SerializeField] Vector2 VelocityMap = Vector2.zero; //affects horizontal movement velocity. Controlled via input
    [SerializeField] Vector2 InputMap = Vector2.zero;
    [SerializeField] Vector2 DragVector = Vector2.zero;
    [SerializeField] Vector3 AddVelocity = Vector2.zero;

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
        Event_SpecialActionStart?.Invoke(new PlayerMovementEventArgs());
    }

    #endregion

    #region Init
    private void Awake()
    {
        InitInput();
        InitializeEvents();
    }

    void Start()
    {
        if (inventory == null) inventory = GetComponent<Inventory>();
        if (inventory == null) Debug.LogException(new System.Exception("PlayerMovement: No inventory found"));

        RB = gameObject.GetComponent<Rigidbody>();
        if (RB == null) RB = gameObject.AddComponent<Rigidbody>();

        if (HorizontalMovementAngleHost == null) HorizontalMovementAngleHost = gameObject;

        //Event_AttackStart.AddListener(testfunction);
        //Event_AttackStart?.Invoke(new PlayerMovementEventArgs("test"));
    }
    //void testfunction(PlayerMovementEventArgs args)
    //{
    //    Debug.Log(args.test);
    //}


    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion

    #region Update Methods
    void Update()
    {
        //if (new Vector3(RB.velocity.x, 0, RB.velocity.z).sqrMagnitude > MoveSpd * MoveSpd * 4)
        //{
        //    Debug.Log("Exceeding");
        //}

        UpdateMovementStates();

    }
    private void UpdateMovementStates()
    {
        //grounded

        //exceeding influencible speed (maybe)

        //[maybe] crouched
        //[maybe] sprint
    }
    private void FixedUpdate()
    {
        HorizontalMovementInput();
    }
    #endregion
        
    #region Input Event Dispatcher
    // This function is called in Awake(), and creates controls 
    // + registers all the events that may occur due to player input
    private void InitInput()
    {
        controls = new PlayerControls();

        //GAMEPAD EVENTS REGISTER //////////////////////////////////////
        //register reading movement values from input
        controls.Gamepad.Movement.performed += ctx => InputMap = ctx.ReadValue<Vector2>();
        controls.Gamepad.Movement.canceled += ctx => InputMap = Vector2.zero;
        controls.Gamepad.Attack.performed += ctx => AttackEvent();
        controls.Gamepad.SpecialAction.performed += ctx => SpecialActionEvent();
        controls.Gamepad.Wepon1Equip.performed += ctx => ChangeWeaponEvent();
        controls.Gamepad.Wepon2Equip.performed += ctx => ChangeWeaponEvent();

        //MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
        //register reading movement values from input
        controls.MouseAndKeyboard.Movement.performed += ctx => InputMap = ctx.ReadValue<Vector2>();
        controls.MouseAndKeyboard.Movement.canceled += ctx => InputMap = Vector2.zero;
        controls.MouseAndKeyboard.Attack.performed += ctx => AttackEvent();
        controls.MouseAndKeyboard.SpecialAction.performed += ctx => SpecialActionEvent();
        controls.MouseAndKeyboard.Wepon1Equip.performed += ctx => ChangeWeaponEvent();
        controls.MouseAndKeyboard.Wepon2Equip.performed += ctx => ChangeWeaponEvent();

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

        float JumpSpeed = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
        RB.AddForce(JumpSpeed * Vector3.up, ForceMode.VelocityChange);
    }
    #endregion
    
    #region Horizontal Movement Models

    //TODO: Move these over to / combine with behavior properties
    [SerializeField] Vector3 TestVelocity = Vector3.zero;
    [SerializeField] float TestAccel = 100f;

    //Debug members
    [SerializeField] Vector3 LargestVelocityChange = Vector3.zero;
    [SerializeField] float LargestVelocityChangeMagnitude = 0;
    [SerializeField] Vector3 LargestAddVelocityChange = Vector3.zero;
    [SerializeField] float LargestAddVelocityChangeMagnitude = 0;
    
    public enum MovementModel { DebugMovementModel, ContinuousAdvanced, ContinuousSimple, VelocityChange };

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

        //Create direction of movement desired by player
        Vector3 InputDirection = new Vector3(InputMap.x, 0, InputMap.y); //TODO: processing based on horizontal angle host
        {
            float AngleDiff = Vector3.SignedAngle(RB.velocity.normalized, InputDirection, Vector3.up);

            //TODO: Consider adding these to movement properties (global scope)
            float MIN_ANGLE_TOLERANCE = 38.0f; //Very important. Controls the arc of player slide control when turning. Stay below 90!
            float MAX_ANGLE_TOLERANCE = 134.9f; //Less important. Controls the angle tolerance for braking. Stay above 90, and stay above MIN!

            //Preprocessing input. Clamp to within 180deg if we are going beyond standard movement speed
            //TODO: verify angle computation is correct in all cases
            if (
                RB.velocity.sqrMagnitude != 0 
                && (RB.velocity.sqrMagnitude > MoveSpd * MoveSpd) 
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
        //TestVelocity = Vector3.zero; if (InputDirection.sqrMagnitude > 0) TestVelocity = InputDirection * MoveSpd;

        //If a direction exists, accelerate towards it. otherwise decelerate towards 0
        if (InputDirection.sqrMagnitude == 0)
        {
            //apply the brakes

            //for comparison to make sure we dont go in the opposite direction
            Vector3 Temp = TestVelocity;

            TestVelocity = TestVelocity - (TestVelocity.normalized * TestAccel * Time.fixedDeltaTime);

            //clamp to 0
            if (Vector3.Dot(TestVelocity, Temp) < 0) TestVelocity = Vector3.zero;
        }
        else
        {
            //accel
            TestVelocity = TestVelocity + InputDirection.normalized * TestAccel * Time.fixedDeltaTime;

            //clamp desired velocity max to move speed max
            if (TestVelocity.sqrMagnitude > MoveSpd * MoveSpd)
                TestVelocity = TestVelocity.normalized * MoveSpd;

        }




        Vector3 TestVelocityDiff = Vector3.zero;

        //Filtered velocity represents the maximum velocity we can affect in this timestep (the rigidbody can exceed this by a LOT in normal gameplay)
        Vector3 FilteredVelocity = RB.velocity;
        //max is clamped to current player-desired velocity max.
        if (FilteredVelocity.sqrMagnitude > TestVelocity.sqrMagnitude) FilteredVelocity = TestVelocity.magnitude * FilteredVelocity.normalized;

        //compute parallel component of current velocity to desired velocity
        if (TestVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
            TestVelocityDiff = (Vector3.Dot(TestVelocity, FilteredVelocity) / TestVelocity.magnitude) * TestVelocity.normalized;

        //Recall that we are comparing parallel vectors here
        if(Vector3.Dot(TestVelocity, RB.velocity) < 0)
        {
            //decelerating. Parallel component of velocity can be at MOST Deceleration * Time.FixedDeltaTime            
            Vector3 Parallel = Vector3.zero;
            Vector3 Perpendicular = Vector3.zero;

            Parallel = (Vector3.Dot(TestVelocity, FilteredVelocity) / FilteredVelocity.magnitude) * FilteredVelocity.normalized;
            //Perpendicular = TestVelocity - Parallel;

            AddVelocity = (Parallel.normalized * Deceleration * Time.fixedDeltaTime) + Perpendicular;
            //AddVelocity = ((-Parallel.normalized * Deceleration * Time.fixedDeltaTime) + Perpendicular);


            //AddVelocity = (TestVelocity - TestVelocityDiff);
            //if (AddVelocity.sqrMagnitude > Deceleration * Deceleration * Time.fixedDeltaTime * Time.fixedDeltaTime) AddVelocity = AddVelocity.normalized * Deceleration * Time.fixedDeltaTime;
        }
        else
        {
            //sustaining velocity
            AddVelocity = (TestVelocity - TestVelocityDiff);
        }


        //Damp (within standard movement speed control)
        if (InputDirection.sqrMagnitude > 0 && RB.velocity.sqrMagnitude <= MoveSpd * MoveSpd)
        {
            RB.AddForce(AddVelocity * ForceCoefficient, ForceMode.Force); //TODO: Consider projecting this onto surface normal of whatever we're standing on (for movement along slopes)


            if ((AddVelocity).sqrMagnitude < (RB.velocity - TestVelocityDiff).sqrMagnitude)
            {
                //RB.AddForce(-(RB.velocity - TestVelocityDiff).normalized * AddVelocity.magnitude * ForceCoefficient, ForceMode.Force);
                RB.AddForce(-(RB.velocity - TestVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }
            else
            {
                RB.AddForce(-(RB.velocity - TestVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }


            if (FLAGDisplayDebugGizmos)
            {
                float a = 2;
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .75f, transform.position.z), -(RB.velocity - TestVelocityDiff) * a, Color.red);
            }
        }
        //Damp (exceeding movement speed max)
        else
        {
            //"Parachute" idea: Add the velocity * forceCoefficient, but pull "backward" on the rigidbody by the amount needed to maintain current velocity (a direction change, but not a velocity one)

            Vector3 ModifiedVelocity = AddVelocity + RB.velocity;

            Vector3 ParachuteVelocity = Vector3.zero;
            //if (ModifiedVelocity.sqrMagnitude > RB.velocity.sqrMagnitude) ParachuteVelocity = -(RB.velocity - ModifiedVelocity).magnitude * ModifiedVelocity.normalized;
            if (ModifiedVelocity.sqrMagnitude > RB.velocity.sqrMagnitude) ParachuteVelocity = Mathf.Abs(ModifiedVelocity.magnitude - RB.velocity.magnitude) * -ModifiedVelocity.normalized; //TODO: Optimize this!

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
            //Debug.DrawRay(transform.position, (RB.velocity + AddVelocity) * a, Color.magenta);
            Debug.DrawRay(transform.position, new Vector3(RB.velocity.x, RB.velocity.y + .1f, RB.velocity.z) * a, Color.white);
        }
        if (FLAGCollectDebugTelemetry)
        {
            //OBSERVATION: We never alter more than MoveSpd's worth of velocity 
            if ((-(RB.velocity - TestVelocityDiff) + AddVelocity).sqrMagnitude > LargestVelocityChangeMagnitude * LargestVelocityChangeMagnitude)
            {
                LargestVelocityChange = (-(RB.velocity - TestVelocityDiff) + AddVelocity);
                LargestVelocityChangeMagnitude = (-(RB.velocity - TestVelocityDiff) + AddVelocity).magnitude;
            }
            if ((AddVelocity).sqrMagnitude > LargestAddVelocityChangeMagnitude * LargestAddVelocityChangeMagnitude)
            {
                LargestAddVelocityChange = AddVelocity;
                LargestAddVelocityChangeMagnitude = (AddVelocity).magnitude;
            }
        }
    }


    void ContinuousMovementModelV2()
    {

        //float MoveSpd = 10f;
        //float TestAccel = 100f;

        float ForceCoefficient = RB.mass / Time.fixedDeltaTime;

        Vector3 InputDirection = new Vector3(InputMap.x, 0, InputMap.y); //TODO: processing based on horizontal angle host
        if (InputDirection.sqrMagnitude == 0)
        {
            //apply the brakes
            Vector3 Temp = TestVelocity;

            TestVelocity = TestVelocity - (TestVelocity.normalized * TestAccel * Time.fixedDeltaTime);
            //if (Vector3.Angle(TestVelocity, Temp) > 60) TestVelocity = Vector3.zero;
            if (Vector3.Dot(TestVelocity, Temp) < 0) TestVelocity = Vector3.zero;
        }
        else
        {
            //accel
            TestVelocity = TestVelocity + new Vector3(InputMap.x, 0, InputMap.y).normalized * TestAccel * Time.fixedDeltaTime;
        }



        if (TestVelocity.sqrMagnitude > MoveSpd * MoveSpd)
            TestVelocity = TestVelocity.normalized * MoveSpd;


        Vector3 TestVelocityDiff = Vector3.zero;
        if (TestVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
            TestVelocityDiff = (Vector3.Dot(TestVelocity, RB.velocity) / TestVelocity.magnitude) * TestVelocity.normalized;


        AddVelocity = (TestVelocity - TestVelocityDiff);

        // Damp ////////////////////////////////////////////////
        //Vector3 ForceDamp = Vector3.zero;
        //if ((AddVelocity + RB.velocity).sqrMagnitude > MoveSpd * MoveSpd)
        //{
        //    ForceDamp = -RB.velocity.normalized * TestAccel * RB.mass;
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
        //ProcessedInputMap *= FrontAcceleration * Time.fixedDeltaTime;
        //VelocityMap += ProcessedInputMap * FrontAcceleration * Time.fixedDeltaTime;
        //AddVelocity += new Vector3(DragVector.x, 0, DragVector.y);
        //////////////////////////////////////////////
        ///
        RB.AddForce(AddVelocity * ForceCoefficient, ForceMode.Force);

        if (InputDirection.sqrMagnitude > 0)
        {
            if ((AddVelocity).sqrMagnitude < (RB.velocity - TestVelocityDiff).sqrMagnitude)
            {
                //RB.AddForce(-(RB.velocity - TestVelocityDiff).normalized * AddVelocity.magnitude * ForceCoefficient, ForceMode.Force);
                RB.AddForce(-(RB.velocity - TestVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }
            else
            {
                RB.AddForce(-(RB.velocity - TestVelocityDiff) * ForceCoefficient, ForceMode.Force);
            }
        }

        // for testing
        if (FLAGDisplayDebugGizmos)
        {
            float a = 2;
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), AddVelocity * a, Color.cyan);
            Debug.DrawRay(transform.position, (RB.velocity + AddVelocity) * a, Color.magenta);
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .75f, transform.position.z), -(RB.velocity - TestVelocityDiff) * a, Color.red);
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

        DragVector *= Vector2.Dot(DragVector, -1 * VelocityMap) * Time.fixedDeltaTime * DampAcceleration;
        if (DragVector.sqrMagnitude > VelocityMap.sqrMagnitude)
        {
            DragVector = Vector2.zero;
            VelocityMap = ProcessedInputMap.normalized * VelocityMap.magnitude * Vector2.Dot(VelocityMap, ProcessedInputMap);
        }

        //apply additional velocity
        //ProcessedInputMap *= FrontAcceleration * Time.fixedDeltaTime;
        VelocityMap += ProcessedInputMap * FrontAcceleration * Time.fixedDeltaTime;
        VelocityMap += DragVector;


        if (VelocityMap.sqrMagnitude > MoveSpd * MoveSpd * 4)
        {
            VelocityMap = VelocityMap.normalized * MoveSpd * 2; //max spd clamp
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

    //NYI
    private IEnumerator DoDash()
    {
        yield return new WaitForFixedUpdate();
    }
}
