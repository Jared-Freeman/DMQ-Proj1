using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public GameObject YawHost;
    
    [Range(.001f, 3f)]
    public float MouseSensitivity = 1;

    [Range(-90f,0f)]
    public float LowerLookBoundary = -90f;
    [Range(0f,90f)]
    public float UpperLookBoundary = 90f;

    //[SerializeField]
    //private float xRotation = 0f;

    [SerializeField]
    private float Inp_x = 0f;
    [SerializeField]
    private float Inp_y = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (YawHost == null) YawHost = gameObject;
        MouseLookBegin();
    }

    void MouseLookBegin()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void MouseLookEnd()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
        Inp_x = Input.GetAxis("Mouse X") * MouseSensitivity;
        Inp_y = Input.GetAxis("Mouse Y") * MouseSensitivity;

        transform.RotateAround(transform.position, transform.right, -Inp_y);
        YawHost.transform.RotateAround(transform.position, Vector3.up, Inp_x);

        /*
        Quaternion q = new Quaternion();
        q = Quaternion.Euler(new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, -90, 90), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));

        transform.rotation = q;
        /*
        ////////

        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;// * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;// * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, LowerLookBoundary, UpperLookBoundary);

        //transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.RotateAround(transform.position, transform.right, xRotation);
        transform.RotateAround(transform.position, Vector3.up, mouseX);
        */

        //transform.localEulerAngles = new Vector3(-Inp_y, Inp_x, 0) + transform.localEulerAngles;
    }
}
