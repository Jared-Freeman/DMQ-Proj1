using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Movement_System_1 : MonoBehaviour
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

    [Range(0,1.0f)]
    public float MovementDamp = .9f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody>();
        if(HorizontalMovementAngleHost == null)
        {
            HorizontalMovementAngleHost = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(new Vector3(RB.velocity.x, 0, RB.velocity.z).sqrMagnitude > MoveSpd * MoveSpd * 4)
        {
            Debug.Log("Exceeding"); 
        }

        UpdateMovementStates();

        HandleInput();
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


    private void HandleInput()
    {

        if (Input.GetButtonDown("Jump"))
        {
            JumpInput();
        }

        if (Input.GetButtonDown("Crouch"))
        {
            CrouchInputStart();
        }
        if (Input.GetButtonUp("Crouch"))
        {
            CrouchInputEnd();
        }
    }


    #region Input Handlers
    //in fixed dt
    private void OldHorizontalMovementInput()
    {
        float CurrentVelocity = RB.velocity.magnitude;

        //Debug.Log("HorizontalMovementInput");
        InputStrafe = Input.GetAxisRaw("Horizontal");
        InputFront = Input.GetAxisRaw("Vertical");

        ForceVector_Front = HorizontalMovementAngleHost.transform.forward;
        ForceVector_Strafe = HorizontalMovementAngleHost.transform.right;

        //float CurrentAccleration = CurrentVelocity / Time.deltaTime;

        //float ClampedFrontAcceleration = 

        float FrontForce = FrontAcceleration * RB.mass;
        float StrafeForce = DampAcceleration * RB.mass;

        ForceVector_Front *= InputFront * FrontForce * Time.fixedDeltaTime;
        ForceVector_Strafe *= InputStrafe * StrafeForce * Time.fixedDeltaTime;

        Vector3 ForceVectorFinal = ForceVector_Front + ForceVector_Strafe;
        /*
        //clamp
        if(ForceVectorFinal.sqrMagnitude > Mathf.Max(MoveSpd * MoveSpd, MoveSpdStrafe * MoveSpdStrafe))
        {
            ForceVectorFinal = ForceVectorFinal.normalized * Mathf.Max(MoveSpd, MoveSpdStrafe);
        }
        */
        if(RB.velocity.sqrMagnitude < Mathf.Max(MoveSpd * MoveSpd, MoveSpdStrafe * MoveSpdStrafe) || (RB.velocity.sqrMagnitude > (ForceVectorFinal + RB.velocity).sqrMagnitude))
            RB.AddForce(ForceVectorFinal, ForceMode.Force);
    }
    //in fixed dt
    private void OldHorizontalMovementInput2()
    {
        InputStrafe = Input.GetAxisRaw("Horizontal");
        InputFront = Input.GetAxisRaw("Vertical");

        Vector3 ConstructionVector1 = Vector3.zero;// = HorizontalMovementAngleHost.transform.forward * InputFront;
        Vector3 ConstructionVector2 = Vector3.zero;// = HorizontalMovementAngleHost.transform.right * InputStrafe;

        //ConstructionVector1 = HorizontalMovementAngleHost.transform.forward * InputFront;
        //ConstructionVector2 = HorizontalMovementAngleHost.transform.right * InputStrafe;


        ConstructionVector2 = HorizontalMovementAngleHost.transform.right * InputStrafe;
        ConstructionVector1 = HorizontalMovementAngleHost.transform.forward * InputFront;

        InputMap.x = ConstructionVector1.x + ConstructionVector2.x;
        InputMap.y = ConstructionVector1.z + ConstructionVector2.z;


        if (InputMap.sqrMagnitude > 1)
        {
            InputMap.Normalize();
        }

        DragVector = Vector3.zero;
        //kill all stuff not in the dir we wanna go
        if (InputFront != 0 || InputStrafe != 0)
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


        /*
        //clamp
        if(Vector2.Angle(VelocityMap, InputMap) == 0)
        {
            Debug.Log("no");
            //DragVector = Vector2.zero;
        }
        else if (Vector2.Angle(VelocityMap + DragVector, InputMap) == 0)
        {
            Debug.Log("angle eq");
            // do nothing
        }
        //overshoot?
        else if(Vector2.SignedAngle(VelocityMap, InputMap)/Vector2.Angle(VelocityMap, InputMap) != Vector2.SignedAngle((VelocityMap + DragVector), InputMap) / Vector2.Angle((VelocityMap + DragVector), InputMap))
        {
            Debug.Log("Trig");
            //Keep only parallel component
            //DragVector = Vector2.zero;
            //VelocityMap = InputMap.normalized * VelocityMap.magnitude * Mathf.Cos(Vector2.SignedAngle(VelocityMap, InputMap));
        }*/


        /*
        InputMap *= FrontAcceleration * Time.fixedDeltaTime; //velocity to add
        
        if(InputFront == 0 && InputStrafe == 0)
        {
            DragVector = VelocityMap.normalized;
        }
        else
        {
            DragVector = Vector2.Perpendicular(InputMap).normalized;
        }
        DragVector *= -1 * Vector2.Dot(VelocityMap, DragVector);
        //DragVector *= -1 * Vector2.Dot(InputMap, DragVector);
        DragVector *= FrontAcceleration * .5f * Time.fixedDeltaTime; //magic numbers atm
        */



        /*
        //decay some old velocity
        DragVector = VelocityMap.normalized * -1;
        DragVector *= FrontAcceleration * 1.5f * Time.fixedDeltaTime; //magic numbers atm

        if(InputMap.sqrMagnitude > 0)
        {
            //drag should only be perpendicular component to InputMap vector.
            DragVector *= Vector2.Dot(DragVector.normalized, Vector2.Perpendicular(InputMap).normalized);
        }
        //guarantee we don't overshoot 0
        if (DragVector.sqrMagnitude > VelocityMap.sqrMagnitude)
        {
            VelocityMap = Vector2.zero;
        }
        else
        {
            VelocityMap += DragVector;
        }
        */

        //apply additional velocity
        VelocityMap += InputMap;
        VelocityMap += DragVector;


        if (VelocityMap.sqrMagnitude > MoveSpd * MoveSpd * 4)
        {
            VelocityMap = VelocityMap.normalized * MoveSpd * 2; //max spd clamp
        }

        Debug.DrawRay(gameObject.transform.position, new Vector3(DragVector.x, 0, DragVector.y), Color.red);
        Debug.DrawRay(gameObject.transform.position, new Vector3(InputMap.x, 0, InputMap.y), Color.blue);
        Debug.DrawRay(gameObject.transform.position, new Vector3(VelocityMap.x, 0, VelocityMap.y), Color.green);

        Vector3 VelocityMap3 = new Vector3(VelocityMap.x, RB.velocity.y, VelocityMap.y);

        //if ((RB.velocity + VelocityMap3).sqrMagnitude > MoveSpd * MoveSpd)

        RB.AddForce(VelocityMap3 - RB.velocity, ForceMode.VelocityChange);

        //RB.velocity = new Vector3((VelocityMap3 - RB.velocity).x, RB.velocity.y, (VelocityMap3 - RB.velocity).z);

        //Vector3 Displacement = new Vector3(VelocityMap.x, 0, VelocityMap.y) * Time.fixedDeltaTime;

        //RB.MovePosition( RB.position + Displacement );
    }
    
    //in fixed dt
    private void HorizontalMovementInput()
    {
        Vector3 RetainedVelocity = new Vector3(0, RB.velocity.y, 0);

        InputStrafe = Input.GetAxisRaw("Horizontal");
        InputFront = Input.GetAxisRaw("Vertical");

        Vector3 ConstructionVector1 = Vector3.zero;
        Vector3 ConstructionVector2 = Vector3.zero;
        ConstructionVector2 = HorizontalMovementAngleHost.transform.right * InputStrafe;
        ConstructionVector1 = HorizontalMovementAngleHost.transform.forward * InputFront;

        InputMap.x = ConstructionVector1.x + ConstructionVector2.x;
        InputMap.y = ConstructionVector1.z + ConstructionVector2.z;


        if (InputMap.sqrMagnitude > 1)
        {
            InputMap.Normalize();
        }

        DragVector = Vector3.zero;
        //kill all stuff not in the dir we wanna go
        if (InputFront != 0 || InputStrafe != 0)
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
        InputMap *= FrontAcceleration * Time.fixedDeltaTime;
        VelocityMap += InputMap;
        VelocityMap += DragVector;


        if (VelocityMap.sqrMagnitude > MoveSpd * MoveSpd * 4)
        {
            VelocityMap = VelocityMap.normalized * MoveSpd * 2; //max spd clamp
        }

        Debug.DrawRay(gameObject.transform.position, new Vector3(DragVector.x, 0, DragVector.y), Color.red);
        Debug.DrawRay(gameObject.transform.position, new Vector3(InputMap.x, 0, InputMap.y), Color.blue);
        Debug.DrawRay(gameObject.transform.position, new Vector3(VelocityMap.x, 0, VelocityMap.y), Color.green);

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
