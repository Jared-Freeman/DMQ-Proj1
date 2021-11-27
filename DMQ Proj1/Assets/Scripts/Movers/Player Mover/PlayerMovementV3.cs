using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementV3 : MonoBehaviour
{
    #region Members
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

    // Start is called before the first frame update
    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody>();
        if (HorizontalMovementAngleHost == null)
        {
            HorizontalMovementAngleHost = gameObject;
        }

        InitInput();
    }

    // Update is called once per frame
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


    #region Input Gatherers
    private void InitInput()
    {
        controls = new PlayerControls();
        Input.onActionTriggered += HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == controls.Player.Movement.name)
        {
            InputMap = ctx.ReadValue<Vector2>();
        }
        if (ctx.action.name == controls.Player.Jump.name)
        {
            if (ctx.performed)
            {
                //JumpInput();
            }
        }
    }

    #endregion

    #region Input Handlers
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
        //Debug.Log("JumpInput");

        StopCoroutine(JumpStart());
        StartCoroutine(JumpStart());

        //float JumpSpeed = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
        //RB.AddForce(JumpSpeed * Vector3.up, ForceMode.VelocityChange);

        //StartCoroutine(JumpNormal(1));
        //RB.AddForce(HorizontalMovementAngleHost.transform.forward * 5 * RB.mass, ForceMode.Impulse);
    }

    private void CrouchInputStart()
    {
        Debug.Log("CrouchInputStart");

    }

    private void CrouchInputEnd()
    {
        Debug.Log("CrouchInputEnd");

    }

    private void SprintMovementStart()
    {
        Debug.Log("SprintMovementStart");

    }

    private void SprintMovementEnd()
    {
        Debug.Log("SprintMovementEnd");

    }
    #endregion

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
}
