using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSEventArgs
{
    public class PlayerInputManager_Args : System.EventArgs
    {
        public PlayerInputManager Manager;

        public PlayerInputManager_Args(PlayerInputManager mgr)
        {
            Manager = mgr;
        }
    }
}

namespace Input_DMQ
{
    /// <summary>
    /// Helper to relay an attached PlayerInputManager to other script(s)
    /// </summary>
    public class DMQ_PlayerInputManager_Relay : MonoBehaviour
    {
        public static event System.EventHandler<CSEventArgs.PlayerInputManager_Args> onPlayerInputManagerCreate;
        public static event System.EventHandler<CSEventArgs.PlayerInputManager_Args> onPlayerInputManagerDestroy;

        private void Start()
        {
            PlayerInputManager mgr = GetComponent<PlayerInputManager>();

            if (mgr != null)
            {
                onPlayerInputManagerCreate?.Invoke(this, new CSEventArgs.PlayerInputManager_Args(mgr));
            }
        }

        private void OnDestroy()
        {
            PlayerInputManager mgr = GetComponent<PlayerInputManager>();

            if (mgr != null)
            {
                onPlayerInputManagerDestroy?.Invoke(this, new CSEventArgs.PlayerInputManager_Args(mgr));
            }
        }
    }
}