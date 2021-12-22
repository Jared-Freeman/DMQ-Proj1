using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Topdown_Multitracking_Camera_Rig : MonoBehaviour
{

    #region members

    //flags
    [Space(10)]
    [Header("**FLAGS**")]

    //NYI
    public bool FLAGDebug = false; //For console output, draw rays...


    //settings
    [Space(10)]
    [Header("**SETTINGS**")]

    [Range(-89.999f, 89.999f)]
    public float AngleMin = 0; //bounded to (-90,90), must be <= angle_max
    [Range(-89.999f, 89.999f)]
    public float AngleMax = 89.999f; //bounded to (-90,90)

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

    public List<GameObject> TargetList;
        
    [Space(10)]
    [Header("**SERIALIZED FIELDS**")]

    //desired states
    [SerializeField]
    private float CamDistanceDesired; //NOTE: This is a SCALAR not a vector!
    [SerializeField]
    private Vector3 CamTargetPositionDesired = Vector3.zero; //NOTE: This is a SCALAR not a vector!
    private Vector3 CamPositionDesired = Vector3.zero; //NOTE: This is a SCALAR not a vector!

    //state variables
    [SerializeField]
    private float CamDistanceCurrent;
    [SerializeField]
    private float CamAdditionalOffsetCurrent;

    private Camera AttachedCamera;


    #endregion

    #region initialization
    private void Start()
    {
        AttachedCamera = GetComponent<Camera>();
    }
    #endregion

    private void Update()
    {
        UpdateTargetDesiredPosition();
        CamDistanceDesired = CameraDistanceDefault;
        CamPositionDesired = CamTargetPositionDesired + (transform.forward * -1f * CamDistanceDesired);

        if (!CameraTargetIsInDeadzone())
        {
            InterpolateToDesiredPosition();
        }



        if (FLAGDebug) Debug.DrawRay(transform.position, transform.forward * 50, Color.blue);
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
        sigma3 = (theta/2) - sigma2; //can be negative
        //delta1 = 180 - (180 - sigma1) - theta / 2;

        float a, b, c;
        a = CamDistanceDesired * Mathf.Cos(Mathf.Deg2Rad * sigma1);
        b = h * Mathf.Tan(Mathf.Deg2Rad * (sigma2 + theta/2));
        c = b - a;

        float aa = 0, ab = 0;
        aa = CamDistanceDesired * Mathf.Sin(Mathf.Deg2Rad * sigma2);
        if(sigma3 > 0)
        {
            ab = h * Mathf.Tan(Mathf.Deg2Rad * sigma3);
        }

        //if (FLAGDebug) Debug.Log("ANGLES: " + " " + theta + " " +/* delta1 +*/ " " + sigma1 + " " + sigma2 + " " + sigma3);
        //if (FLAGDebug) Debug.Log("MEASUREMENTS: " + h + " " + a + " " + b + " " + c + " " + aa + " " + ab);

        return (c - aa - ab) / 2;
    }

    private void UpdateTargetDesiredPosition()
    {
        //Idea: Get x,z min's,max's and determine midpoint of generated rectangle
        float MaxX = Mathf.NegativeInfinity, MinX = Mathf.Infinity;
        float MaxZ = Mathf.NegativeInfinity, MinZ = Mathf.Infinity;
        float MaxY = Mathf.NegativeInfinity, MinY = Mathf.Infinity;
        foreach (GameObject GO in TargetList)
        {
            if (FLAGDebug) Debug.Log(gameObject.name + ": targetlist chk");
            if (FLAGDebug) Debug.DrawRay(GO.transform.position, Vector3.up * 7f, Color.cyan);

            if (GO.transform.position.x < MinX) MinX = GO.transform.position.x;
            if (GO.transform.position.x > MaxX) MaxX = GO.transform.position.x;

            if (GO.transform.position.z < MinZ) MinZ = GO.transform.position.z;
            if (GO.transform.position.z > MaxZ) MaxZ = GO.transform.position.z;

            if (GO.transform.position.y > MaxY) MaxY = GO.transform.position.y;
            if (GO.transform.position.y < MinY) MinY = GO.transform.position.y;
        }

        CamTargetPositionDesired = new Vector3((MaxX + MinX) / 2, (MaxY + MinY) / 2, (MaxZ + MinZ) / 2);

        if (FLAGDebug) Debug.DrawRay(CamTargetPositionDesired, Vector3.up * 2f, Color.green);
        if (FLAGDebug) Debug.DrawRay(CamTargetPositionDesired, Vector3.forward * 2f, Color.blue);
        if (FLAGDebug) Debug.DrawRay(CamTargetPositionDesired, Vector3.right * 2f, Color.red);
        //CamPositionDesired = new Vector3(4,0,4);
    }

    private void InterpolateToDesiredPosition()
    {
        /* TODO: Add-in offset component in screen-space computations (or after computation?)
        */
        CamAdditionalOffsetCurrent = 0f; //Can re-add offset when it's fully parameterized       
        CamAdditionalOffsetCurrent = ComputeAdditionalOffset();
        CamPositionDesired += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * -1f * CamAdditionalOffsetCurrent * CamOffsetScale;

        //Larger deadzones will make a Lerp method look ugly. Eventually want to implement a "thruster" approach for interp
        //transform.position = Vector3.Lerp(transform.position, CamPositionDesired, Mathf.Clamp(1.45f * Time.deltaTime, 0, 1)); //TODO: Make this actually smart


        //Eventually fix this or whatever
        float LeashDistance = 6f;
        float T_param = (transform.position - CamPositionDesired).magnitude / LeashDistance;
        //Debug.Log(T_param);
        T_param = Freeman_Utilities.MapValueFromRangeToRange(T_param, 0, LeashDistance, 0, 1);
        if(FLAGDebug) Debug.Log(1-T_param);

        transform.position = Vector3.Lerp(transform.position, CamPositionDesired, 1-T_param); //TODO: Make this actually smart
    }

    private bool CameraTargetIsInDeadzone()
    {
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
}
