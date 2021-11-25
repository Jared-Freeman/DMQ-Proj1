using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TextBubble_CameraProxy : MonoBehaviour
{
    #region MEMBERS 
    public bool Flag_Debug;
    public Camera AttachedCamera;
    #endregion

    #region INIT
    private void Start()
    {
        AttachedCamera = GetComponent<Camera>();
        if (AttachedCamera == null) Destroy(this);
    }
    #endregion

    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Text_Bubble.RequestAlignToCameraAnglesEvent += HandleCameraAnglesRequest;
    }
    private void OnDisable()
    {
        Text_Bubble.RequestAlignToCameraAnglesEvent -= HandleCameraAnglesRequest;
    }
    #endregion

    #region EVENT HANDLERS
    void HandleCameraAnglesRequest(object dispatcher, MonobehaviourEventArgs MonoArgs)
    {
        if (Flag_Debug)
        {
            Debug.Log("HandleCameraAnglesRequest -- Event handled");

            if (dispatcher == null) Debug.LogError("obj wrong");
            if (MonoArgs == null) Debug.LogError("mono wrong");
            else if (AttachedCamera == null) Debug.LogError("go wrong"); //somehow this is getting called with the attached camera set to null???

        }

        MonoArgs.monobehaviour.gameObject.transform.rotation = gameObject.transform.rotation;
    }
    #endregion
}
