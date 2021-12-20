using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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

    Inventory inventory;
    


    [Range(.1f, 5000f)]
    [Tooltip("m / sec^2")]
    public float FrontAcceleration = 1;
    [Range(.1f, 5000f)]
    [Tooltip("m / sec^2")]
    public float DampAcceleration = 1;
    [Tooltip("Meters / sec")]
    public float MoveSpd = 1;
    [Header("Currently Obsolete")]
    [Tooltip("Meters / sec")]
    public float MoveSpdStrafe = 1;
    [Tooltip("1.5x true value seems to be ideal rn")]
    public float JumpHeight = 1;
    [Tooltip("Scaled by Mass")]
    public float AddedDownwardForce = 1;

    [Space(10)]


    [Header("Input")]
    public PlayerInput Input;
    private PlayerControls controls;


    [Space(10)]

    public GameObject HorizontalMovementAngleHost;


    [Space(30)]

    [SerializeField]
    private Rigidbody RB;

    [Space(10)]

    [SerializeField]
    float InputStrafe = 0;
    [SerializeField]
    float InputFront = 0;

    [Space(10)]

    [SerializeField]
    Vector2 VelocityMap = Vector2.zero; //affects horizontal movement velocity. Controlled via input
    [SerializeField]
    Vector2 InputMap = Vector2.zero;
    [SerializeField]
    Vector2 DragVector = Vector2.zero;

    [Space(10)]

    [SerializeField]
    Vector3 ForceVector_Front = Vector3.zero;
    [SerializeField]
    Vector3 ForceVector_Strafe = Vector3.zero;

    [Range(0, 1.0f)]
    public float MovementDamp = .9f;
    #endregion

    #region EVENTS

    //TODO: figure out what event arg type(s) to use and why
    public class PlayerMovementEvent : UnityEvent<PlayerMovementEventArgs> { };

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
        if(inventory == null) inventory = GetComponent<Inventory>();
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

    void Update()
    {
        if (new Vector3(RB.velocity.x, 0, RB.velocity.z).sqrMagnitude > MoveSpd * MoveSpd * 4)
        {
            Debug.Log("Exceeding");
        }

        UpdateMovementStates();

    }

    private void FixedUpdate()
    {
        //if (new Vector3(RB.velocity.x, 0, RB.velocity.z).sqrMagnitude < MoveSpd * MoveSpd * 4)
        HorizontalMovementInput();

        //RB.AddForce(Vector3.down * AddedDownwardForce * RB.mass * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void UpdateMovementStates()
    {
        //grounded

        //exceeding influencible speed

        //[maybe] crouched
        //[maybe] sprint
    }

    #region Input Handlers
    // This function is called in Awake(), and creates controls 
    // + registers all the events that may occur due to player input
    private void InitInput()
    {
        controls = new PlayerControls();

        //GAMEPAD EVENTS REGISTER //////////////////////////////////////
        //register reading movement values from input
        controls.Gamepad.Movement.performed += ctx =>           InputMap = ctx.ReadValue<Vector2>();
        controls.Gamepad.Movement.canceled += ctx =>            InputMap = Vector2.zero;
        controls.Gamepad.Attack.performed += ctx =>             AttackEvent();
        controls.Gamepad.SpecialAction.performed += ctx =>      SpecialActionEvent();
        controls.Gamepad.Wepon1Equip.performed += ctx =>        ChangeWeaponEvent();
        controls.Gamepad.Wepon2Equip.performed += ctx =>        ChangeWeaponEvent();

        //MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
        //register reading movement values from input
        controls.MouseAndKeyboard.Movement.performed += ctx =>          InputMap = ctx.ReadValue<Vector2>();
        controls.MouseAndKeyboard.Movement.canceled += ctx =>           InputMap = Vector2.zero;
        controls.MouseAndKeyboard.Attack.performed += ctx =>            AttackEvent();
        controls.MouseAndKeyboard.SpecialAction.performed += ctx =>     SpecialActionEvent();
        controls.MouseAndKeyboard.Wepon1Equip.performed += ctx =>       ChangeWeaponEvent();
        controls.MouseAndKeyboard.Wepon2Equip.performed += ctx =>       ChangeWeaponEvent();

    }
    //in fixed dt
    private void HorizontalMovementInput()
    {
        Vector3 RetainedVelocity = new Vector3(0, RB.velocity.y, 0);

        //Vector3 ConstructionVector1 = Vector3.zero;
        //Vector3 ConstructionVector2 = Vector3.zero;
        //ConstructionVector2 = HorizontalMovementAngleHost.transform.right * InputStrafe;
        //ConstructionVector1 = HorizontalMovementAngleHost.transform.forward * InputFront;

        //InputMap.x = ConstructionVector1.x + ConstructionVector2.x;
        //InputMap.y = ConstructionVector1.z + ConstructionVector2.z;

        //transform.Translate(new Vector3(InputMap.x, 0, InputMap.y) * MoveSpd * Time.deltaTime, Space.World);
        //return;



        //TODO: investigate replacing InputMap with a newly instantiated vector2. would it mess with this script in another method?
        if (InputMap.sqrMagnitude > 1)
        {
            InputMap.Normalize();
        }

        DragVector = Vector3.zero;
        //kill all stuff not in the dir we wanna go
        if (InputMap.x != 0 || InputMap.y != 0)
        {
            DragVector = Vector2.Perpendicular(InputMap);
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
            VelocityMap = InputMap.normalized * VelocityMap.magnitude * Vector2.Dot(VelocityMap, InputMap);
        }

        //apply additional velocity
        //InputMap *= FrontAcceleration * Time.fixedDeltaTime;
        VelocityMap += InputMap * FrontAcceleration * Time.fixedDeltaTime;
        VelocityMap += DragVector;


        if (VelocityMap.sqrMagnitude > MoveSpd * MoveSpd * 4)
        {
            VelocityMap = VelocityMap.normalized * MoveSpd * 2; //max spd clamp
        }

        Debug.DrawRay(gameObject.transform.position, new Vector3(DragVector.x, 0, DragVector.y), Color.magenta);
        Debug.DrawRay(gameObject.transform.position, new Vector3(InputMap.x * 1.5f, .6f, InputMap.y * 1.5f), Color.green);
        Debug.DrawRay(gameObject.transform.position, new Vector3(VelocityMap.x, 0, VelocityMap.y), Color.cyan);

        Vector3 VelocityMap3 = new Vector3(VelocityMap.x, 0, VelocityMap.y);
        VelocityMap3 += RetainedVelocity;

        //if ((RB.velocity + VelocityMap3).sqrMagnitude > MoveSpd * MoveSpd)

        RB.AddForce(VelocityMap3 - RB.velocity, ForceMode.VelocityChange);

        //RB.velocity = new Vector3((VelocityMap3 - RB.velocity).x, RB.velocity.y, (VelocityMap3 - RB.velocity).z);

        //Vector3 Displacement = new Vector3(VelocityMap.x, 0, VelocityMap.y) * Time.fixedDeltaTime;

        //RB.MovePosition( RB.position + Displacement );
    }

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
}
