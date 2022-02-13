using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

/// <summary>
/// Assistant to UnityInputPackage's PlayerInputManager. Singleton allows input mgr to be accessed from anywhere. 
/// Also in charge of filing PlayerInput GameObjects into the correct position in the scene hierarchy (currently in DontDestroyOnLoad 2-12-2022)
/// </summary>
public class PlayerManager_Proxy : Singleton<PlayerManager_Proxy>
{
    [SerializeField] private PlayerInputManager _InputManager; //inspector 
    public PlayerInputManager InputManager { get => _InputManager; private set => _InputManager = value; }

    protected override void Awake()
    {
        base.Awake();

        if (InputManager == null)
        {
            Debug.LogError("No Input Manager Specified! Destroying");
            Destroy(this);
        }

        InputManager.onPlayerJoined += InputManager_onPlayerJoined;
    }


    //void OnDestroy()
    //{
    //    InputManager.onPlayerJoined -= InputManager_onPlayerJoined;
    //}

    private void InputManager_onPlayerJoined(PlayerInput obj)
    {
        DontDestroyOnLoad(obj.gameObject);

        obj.gameObject.transform.parent = gameObject.transform;
    }
}
