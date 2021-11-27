using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// DESC:

// This camera rig orbits around the target gameobject.

// Camera parameters (these rules may sometimes be broken or overridden if everything is designed well):
// - yaw sensitivity
// - flag invert pitch
// - flag invert yaw (who does this??????)
// - pitch sensitivity
// - angle low
// - angle high (typically vector3(0,1,0) direction is max high angle)
// - desired camera distance (to target)
//   - interpolated either through lerp, moveto w speed/accel parms, bounded by max distance differential, or perhaps frame position buffer averaging
// - 
public class Orbital_Camera_Rig : MonoBehaviour
{
    #region members

    //flags
    [Header("**FLAGS**")]

    public bool FLAG_invert_pitch = false;
    public bool FLAG_invert_yaw = false;
    public bool FLAG_use_initial_camera_distance = false; //sets default camera distance to the distance magnitude between this gameobject and the target

    [Space(10)]
    [Header("**INPUT**")]
    public PlayerInput Input;
    private PlayerControls Controls;

    //settings
    [Space(10)]
    [Header("**SETTINGS**")]

    [Range(-89.999f, 89.999f)]
    public float angle_min = 0; //bounded to (-90,90), must be <= angle_max
    [Range(-89.999f, 89.999f)]
    public float angle_max = 90; //bounded to (-90,90)

    public float input_horizontal_sens = 5; //multiplier
    public float input_vertical_sens = 5;   //multiplier

    public float cam_dist_default;

    public GameObject target;


    //desired states
    [SerializeField]
    private float cam_dist_desired; //NOTE: This is a SCALAR not a vector!
    [SerializeField]
    private float cam_yaw_desired;
    [SerializeField]
    private float cam_pitch_desired;


    //state variables
    [SerializeField]
    private float inp_x;
    [SerializeField]
    private float inp_y;

    Vector2 InputMap = Vector2.zero;

    private float cam_dist_current;
    private float cam_yaw_current; //euler angle, bounded: [0,360)
    private float cam_pitch_current; //RELATIVE euler angle bounded: [angle_min, angle_max]

    #endregion

    #region initialization
    private void Start()
    {
        if(target == null)
        {
            Debug.LogError("ERROR! Orbital Camera Rig does not have a target! Destroying script.");
            Destroy(this);
        }

        if (FLAG_use_initial_camera_distance) cam_dist_desired = (gameObject.transform.position - target.transform.position).magnitude;
        else cam_dist_desired = cam_dist_default;


        InitInput();
    }

    private void InitInput()
    {
        Controls = new PlayerControls();
        Input.onActionTriggered += HandleInput;
    }

    #endregion

    #region event handlers

    private void HandleInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == Controls.Player.Movement.name)
        {
            InputMap = ctx.ReadValue<Vector2>();
        }
    }
    #endregion

    private void Update()
    {
        UpdateInputVars();
        UpdateStateVars();
        UpdateCameraPosition();
    }

    void UpdateInputVars()
    {
        float delta_time_scaled = 100f * Time.deltaTime;

        inp_x = InputMap.x * input_horizontal_sens * delta_time_scaled;
        if (FLAG_invert_yaw) inp_x *= -1;
        inp_y = InputMap.y * input_vertical_sens * delta_time_scaled;
        if (FLAG_invert_pitch) inp_y *= -1;

    }

    void UpdateStateVars()
    {
        cam_yaw_desired = (cam_yaw_desired + inp_x) % 360;
        cam_pitch_desired = Mathf.Clamp(cam_pitch_desired + inp_y, angle_min, angle_max);
    }

    void UpdateCameraPosition()
    {
        Vector3 offset = new Vector3(cam_dist_desired * Mathf.Cos(Mathf.Deg2Rad * cam_pitch_desired), cam_dist_desired * Mathf.Sin(Mathf.Deg2Rad * cam_pitch_desired), 0);

        //TODO: interpolate experiments!
        gameObject.transform.position = target.transform.position + offset;
        gameObject.transform.RotateAround(target.transform.position, Vector3.up, cam_yaw_desired);
        transform.LookAt(target.transform, Vector3.up);
    }
}
