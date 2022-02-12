using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using ClassSystem;

/// <summary>
/// Container for Player Data for use in-session. Info can be extrapolated and saved afterward
/// </summary>
[System.Serializable]
public class PlayerData_Session
{
    #region Ctor

    public PlayerData_Session() { }

    public PlayerData_Session(string savePath)
    {
        SaveDataPath = savePath;
    }
    #endregion

    #region Members

    public readonly string SaveDataPath;


    public PlayerData_StateInfo Info;

    #region Helpers

    /// <summary>
    /// Current chosen character info including Class
    /// </summary>
    [System.Serializable]
    public struct PlayerData_StateInfo
    {
        public PlayerInput _Input;
        public CharacterClass _CurrentClassPreset;
    }

    #endregion

    #endregion
    //Data during runtime gets stored here:
    //character level 
    //preferences
    //etc


}
