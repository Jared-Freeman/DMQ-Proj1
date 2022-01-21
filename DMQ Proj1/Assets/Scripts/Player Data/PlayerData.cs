using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for Player Data for use in-session. Info can be extrapolated and saved afterward
/// </summary>
[System.Serializable]
public class PlayerData_Session
{
    public PlayerData_Session(string savePath)
    {
        SaveDataPath = savePath;
    }

    public readonly string SaveDataPath;

    //Data during runtime gets stored here:
    //character level 
    //preferences
    //etc
}
