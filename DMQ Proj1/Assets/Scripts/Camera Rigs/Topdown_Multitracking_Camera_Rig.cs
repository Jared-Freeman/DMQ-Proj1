using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class Topdown_Multitracking_Camera_Rig : MonoBehaviour
{

    #region members

    //flags
    [Header("__FLAGS__")]
    [Tooltip("For console output, draw rays...")]
    public bool FLAGDebug = false; //For console output, draw rays...


    
    //settings
    [Header("__SETTINGS__")]

    [Range(-89.999f, 89.999f)]
    public float AngleMin = 0; //bounded to (-90,90), must be <= angle_max

    [Range(-89.999f, 89.999f)]
    public float AngleMax = 89.999f; //bounded to (-90,90)

    //NYI
    public float MovementAcceleration = 1f;
    public float MovementDeceleration = 1f;
    public float MovementSpeedMax = 1f;

    [Range(0f, 1f)]
    public float CamOffsetScale = 1f;

    [Min(0f)]
    public float CameraDistanceDefault = 1f;

    [Min(.1f)]
    public float CameraDistanceMax = 1f;

    [Min(0f)]
    public float CameraDistanceMin = 1f;

    [Tooltip("X and Y should be zeroed. The length and width represent proportions of screen size and must be bounded between [0,1]")]
    public Rect DeadzoneDimensions = Rect.zero; //Centered at center of screen

    public enum MovementStyle {DebugMovement, LerpSimple, LogVelocity};
    public MovementStyle MovementType = MovementStyle.LogVelocity;

    //Movement Style specific properties
    [Header("*** Movement Style Properties ***")]
    public float LogVelocity_Acceleration = 14f; //we express acceleration using this and a standard log function
    [Min(.000001f)]
    public float LogVelocity_CameraSoftRadius = 8f;

    //External refs
    [Header("__EXTERNAL OBJECTS__")]
    public List<GameObject> TargetList;
    public PlayerInputManager PlayerInputMgr;
    private Camera AttachedCamera;



    //Debug stuff. Note these data items are actually used in the script
    [Header("__DEBUG TELEMETRY__")]

    //desired states
    [SerializeField]
    private float CamDistanceDesired; //NOTE: This is a SCALAR not a vector!
    [SerializeField]
    private Vector3 CamTargetPositionDesired = Vector3.zero;
    private Vector3 CamPositionDesired = Vector3.zero;

    //state variables
    [SerializeField]
    public float CamDistanceCurrent { get; private set; }
    [SerializeField]
    public float CamAdditionalOffsetCurrent { get; private set; }



    #endregion

    #region initialization
    private void Awake()
    {
        AttachedCamera = GetComponent<Camera>();
        if (TargetList == null) TargetList = new List<GameObject>();
    }

    #region Events
    private void OnEnable()
    {
        PlayerInputMgr.onPlayerJoined += PlayerInputMgr_onPlayerJoined;
        PlayerInputMgr.onPlayerLeft += PlayerInputMgr_onPlayerLeft;

        DEBUG_PlayerSpawner.OnPlayerAgentsInstantiated += DEBUG_PlayerSpawner_OnPlayerAgentsInstantiated;

        Actor.OnActorDestroyed += Actor_OnActorDestroyed;
    }

    private void Actor_OnActorDestroyed(object sender, CSEventArgs.ActorEventArgs e)
    {
        if (TargetList.Contains(e._Actor.gameObject)) TargetList.Remove(e._Actor.gameObject);
    }

    private void DEBUG_PlayerSpawner_OnPlayerAgentsInstantiated(object sender, CSEventArgs.GameObjectListEventArgs e)
    {
        foreach(var g in e.gameObjects)
        {
            TargetList.Add(g);
        }
    }

    private void OnDisable()
    {
        PlayerInputMgr.onPlayerJoined -= PlayerInputMgr_onPlayerJoined;
        PlayerInputMgr.onPlayerLeft -= PlayerInputMgr_onPlayerLeft;

        DEBUG_PlayerSpawner.OnPlayerAgentsInstantiated -= DEBUG_PlayerSpawner_OnPlayerAgentsInstantiated;

        Actor.OnActorDestroyed -= Actor_OnActorDestroyed;
    }

    //functionality removed.
    private void PlayerInputMgr_onPlayerJoined(PlayerInput obj)
    {
        //TargetList.Add(obj.gameObject);
    }
    private void PlayerInputMgr_onPlayerLeft(PlayerInput obj)
    {
        //TargetList.Remove(obj.gameObject);
    }
    #endregion

    #endregion

    #region Update

    private void Update()
    {
        if(TargetList.Count > 0)
        {
            //These methods must be executed in this order
            UpdateTargetDesiredPosition();
            UpdateCamDistanceDesired();
            UpdateCamPositionDesired();

            if (!CameraTargetIsInDeadzone())
            {
                InterpolateToDesiredPosition();
            }
        }

        if (FLAGDebug) Debug.DrawRay(transform.position, transform.forward * 50, Color.blue);
    }

    #region Update Vars

    private void UpdateTargetDesiredPosition()
    {
        //Idea: Get x,z min's,max's and determine midpoint of generated rectangle
        float MaxX = Mathf.NegativeInfinity, MinX = Mathf.Infinity;
        float MaxZ = Mathf.NegativeInfinity, MinZ = Mathf.Infinity;
        float MaxY = Mathf.NegativeInfinity, MinY = Mathf.Infinity;
        foreach (GameObject GO in TargetList)
        {
            if (FLAGDebug)
            {
                Debug.Log(gameObject.name + ": targetlist chk");
                Debug.DrawRay(GO.transform.position, Vector3.up * 7f, Color.cyan);
            }

            if (GO.transform.position.x < MinX) MinX = GO.transform.position.x;
            if (GO.transform.position.x > MaxX) MaxX = GO.transform.position.x;

            if (GO.transform.position.z < MinZ) MinZ = GO.transform.position.z;
            if (GO.transform.position.z > MaxZ) MaxZ = GO.transform.position.z;

            if (GO.transform.position.y > MaxY) MaxY = GO.transform.position.y;
            if (GO.transform.position.y < MinY) MinY = GO.transform.position.y;
        }

        CamTargetPositionDesired = new Vector3((MaxX + MinX) / 2, (MaxY + MinY) / 2, (MaxZ + MinZ) / 2);

        if (FLAGDebug)
        {
            Debug.DrawRay(CamTargetPositionDesired, Vector3.up * 2f, Color.green);
            Debug.DrawRay(CamTargetPositionDesired, Vector3.forward * 2f, Color.blue);
            Debug.DrawRay(CamTargetPositionDesired, Vector3.right * 2f, Color.red);
        }
        //CamPositionDesired = new Vector3(4,0,4);
    }

    private void UpdateCamDistanceDesired()
    {
        CamDistanceDesired = CameraDistanceDefault;
        CamPositionDesired = CamTargetPositionDesired + (transform.forward * -1f * CamDistanceDesired);
    }

    private void UpdateCamPositionDesired()
    {
        //TODO: Add-in offset component in screen-space computations (or after computation?)
        CamAdditionalOffsetCurrent = 0f; //Can re-add offset when it's fully parameterized       
        CamAdditionalOffsetCurrent = ComputeAdditionalOffset();
        CamPositionDesired += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * -1f * CamAdditionalOffsetCurrent * CamOffsetScale;
    }

    //TODO: Cache most of these computations and recalculate them only when we change the state variables
    //TODO: Parameterize the strength of additional offset (based on parallel linear component distance between closest and furthest tracked obj)
    private float ComputeAdditionalOffset()
    {
        float h, sigma1, sigma2, sigma3, theta; // delta1;
        h = transform.position.y - CamTargetPositionDesired.y; if (h < 0) return 0;
        sigma1 = Vector3.Angle(transform.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.up));
        sigma2 = 90 - sigma1;
        theta = GetComponent<Camera>().GetGateFittedFieldOfView();
        sigma3 = (theta / 2) - sigma2; //can be negative
        //delta1 = 180 - (180 - sigma1) - theta / 2;

        float a, b, c;
        a = CamDistanceDesired * Mathf.Cos(Mathf.Deg2Rad * sigma1);
        b = h * Mathf.Tan(Mathf.Deg2Rad * (sigma2 + theta / 2));
        c = b - a;

        float aa = 0, ab = 0;
        aa = CamDistanceDesired * Mathf.Sin(Mathf.Deg2Rad * sigma2);
        if (sigma3 > 0)
        {
            ab = h * Mathf.Tan(Mathf.Deg2Rad * sigma3);
        }

        //if (FLAGDebug) Debug.Log("ANGLES: " + " " + theta + " " +/* delta1 +*/ " " + sigma1 + " " + sigma2 + " " + sigma3);
        //if (FLAGDebug) Debug.Log("MEASUREMENTS: " + h + " " + a + " " + b + " " + c + " " + aa + " " + ab);

        return (c - aa - ab) / 2;
    }
    #endregion

    #endregion

    #region Movement
    private void InterpolateToDesiredPosition()
    {
        switch (MovementType)
        {
            case MovementStyle.DebugMovement:
                DebugMovement();
                break;
            case MovementStyle.LogVelocity:
                LogVelocityMovement();
                break;
            case MovementStyle.LerpSimple:
                LerpSimpleMovement();
                break;
            default:
                break;
        };
    }

    void DebugMovement()
    {
        transform.position = CamPositionDesired;
    }

    void LerpSimpleMovement()
    {
        //Larger deadzones will make a Lerp method look ugly. Eventually want to implement a "thruster" approach for interp
        //transform.position = Vector3.Lerp(transform.position, CamPositionDesired, Mathf.Clamp(1.45f * Time.deltaTime, 0, 1)); //TODO: Make this actually smart

        //Eventually fix this or whatever
        float LeashDistance = 6f;
        float T_param = (transform.position - CamPositionDesired).magnitude / LeashDistance;
        //Debug.Log(T_param);
        T_param = Freeman_Utilities.MapValueFromRangeToRange(T_param, 0, LeashDistance, 0, 1);
        //if(FLAGDebug) Debug.Log(1-T_param);

        transform.position = Vector3.Lerp(transform.position, CamPositionDesired, 1 - T_param); //TODO: Make this actually smart

    }

    void LogVelocityMovement()
    {
        float delta = (transform.position - CamPositionDesired).magnitude / LogVelocity_CameraSoftRadius;

        float CurrentMoveSpeed = Mathf.Log(delta + 1, 2) * LogVelocity_Acceleration; //CurrentMoveSpeed: [0, +inf)

        if (FLAGDebug) Debug.Log(CurrentMoveSpeed);

        float DisplacementMagnitude = CurrentMoveSpeed * Time.deltaTime;
        Vector3 Distance = (CamPositionDesired - transform.position);
        float DisplacementToleranceScale = 1.001f;
        if (Distance.sqrMagnitude > DisplacementMagnitude * DisplacementMagnitude * DisplacementToleranceScale)
        {
            transform.position = transform.position + Distance.normalized * DisplacementMagnitude;
        }
        else
        {
            transform.position = CamPositionDesired;
        }

    }
    #endregion

    private bool CameraTargetIsInDeadzone()
    {
        //Exit if deadzone doesnt exist
        if (DeadzoneDimensions.height <= 0 || DeadzoneDimensions.width <= 0) return false;
        

        Vector3 ScreenPosition = AttachedCamera.WorldToScreenPoint(CamTargetPositionDesired);
        
        float H_half = .5f * Screen.height * Mathf.Clamp(DeadzoneDimensions.height, 0, 1);

        //check x dims
        if ((Screen.height/2 - H_half <= ScreenPosition.y) && (ScreenPosition.y <= Screen.height/2 + H_half))
        {
            float W_half = .5f * Screen.width * Mathf.Clamp(DeadzoneDimensions.width, 0, 1);

            //check y dims
            if ((Screen.width/2 - W_half <= ScreenPosition.x) && (ScreenPosition.x <= Screen.width/2 + W_half))
            {
                if (FLAGDebug) Debug.Log("In Deadzone! " + H_half + " " + W_half);
                return true;
            }
        }       

        return false;
    }

    /// <summary>
    /// Instantly teleports the camera to its desired target.
    /// </summary>
    public void WarpToCurrentDesiredLocation()
    {
        transform.position = CamPositionDesired;
    }
}
