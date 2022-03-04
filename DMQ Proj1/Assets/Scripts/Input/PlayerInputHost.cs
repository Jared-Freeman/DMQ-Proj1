using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace CSEventArgs
{
    public class PlayerInputEventArgs : System.EventArgs
    {
        public PlayerInput input;

        public PlayerInputEventArgs(PlayerInput p)
        {
            input = p;
        }
    }
}

/// <summary>
/// Container for current PlayerInput attached to this GameObject. 
/// Useful for other scripts to reference.
/// </summary>
public class PlayerInputHost : MonoBehaviour
{
    public event System.EventHandler<CSEventArgs.PlayerInputEventArgs> OnInputChanged;

    [SerializeField] private PlayerInput _curPlayerInput;

    public PlayerInput CurrentPlayerInput 
    { 
        get { return _curPlayerInput; }
        set
        {
            if(value != _curPlayerInput)
            {
                _curPlayerInput = value;

                OnInputChanged?.Invoke(this, new CSEventArgs.PlayerInputEventArgs(_curPlayerInput));
            }
        }    
    }
}
