﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

/// <summary>
/// Singleton that maintains a list of all loaded player session data. Implements Loading and Saving player session data.
/// </summary>
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    //Static members
    private static string s_ProfileSubfolderPath = "/PlayerProfiles";
    private static string s_ProfileSaveFileExtension = ".psave"; //player profile save "psave" idk lol


    #region Members

    public int MaxActivatedPlayerSessions { get; private set; } = 4;

    //Save files list. Should these be shared publicly?
    private List<string> _ProfileSavePathList;

    /// <summary>
    /// All joined players
    /// </summary>
    [SerializeField] private List<PlayerData_Session> _Data;
    /// <summary>
    /// Subset of all joined players
    /// </summary>
    [SerializeField] private List<PlayerData_Session> _ActivatedPlayers;


    /// <summary>
    /// List of all players JOINED (BOTH active and inactive)
    /// </summary>
    public IReadOnlyCollection<PlayerData_Session> JoinedPlayerSessions { get { return _Data.AsReadOnly(); } }

    /// <summary>
    /// List of all players ACTIVATED. These are the player sessions to be used during gameplay
    /// </summary>
    public IReadOnlyCollection<PlayerData_Session> ActivatedPlayerSessions { get { return _ActivatedPlayers.AsReadOnly(); } }



    //Public facing
    public string PlayerProfilePath { get { return Application.persistentDataPath + s_ProfileSubfolderPath; } }
    public string PlayerProfileFileExtension { get { return s_ProfileSaveFileExtension; } }

    #endregion


    #region Test Methods

    /// <summary>
    /// Creates a debug, empty save file. Does not check if a file of same name exists
    /// </summary>
    private void Test_GenerateSave()
    {
        System.IO.FileStream fstream = System.IO.File.Create(Test_GetDebugOutString());

        // System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        // binary format + serialize data here...

        fstream.Close();
    }

    //test
    public string Test_GetDebugOutString()
    {
        string OutString = PlayerProfilePath + "/NewSave" + PlayerProfileFileExtension;

        Debug.Log(OutString);

        return OutString;
    }

    #endregion


    #region Initialization

    protected override void Awake()
    {
        base.Awake();

        Singleton<PlayerManager_Proxy>.Instance.InputManager.onPlayerJoined += InputManager_onPlayerJoined;
        Singleton<PlayerManager_Proxy>.Instance.InputManager.onPlayerLeft += InputManager_onPlayerLeft;

        VerifyProfileDirectoryIntegrity(); //Guarantees <PlayerProfilePath> exists

        //Test_GenerateSave();

        InitProfileFileList();
    }

    private void InputManager_onPlayerLeft(UnityEngine.InputSystem.PlayerInput obj)
    {
        var r = GetRecord(obj);

        if(r != null)
        {
            _Data.Remove(r);
        }
    }

    private void InputManager_onPlayerJoined(UnityEngine.InputSystem.PlayerInput obj)
    {
        PlayerData_Session r = new PlayerData_Session();

        r.Info._Input = obj;
        r.Info._CurrentClassPreset = null;

        _Data.Add(r);
    }



    /// <summary>
    /// Returns true if the directory already exists. Creates the directory and returns false if it does not.
    /// </summary>
    private bool VerifyProfileDirectoryIntegrity()
    {
        if(System.IO.Directory.Exists(PlayerProfilePath))
        {
            return true;
        }
        else
        {
            System.IO.Directory.CreateDirectory(PlayerProfilePath);
            return false;
        }
    }

    private void InitProfileFileList()
    {
        _ProfileSavePathList = new List<string>();

        string[] PlayerFiles = System.IO.Directory.GetFiles(PlayerProfilePath);
        foreach (string file in PlayerFiles)
        {
            if (System.IO.Path.GetExtension(file) == PlayerProfileFileExtension)
            {
                _ProfileSavePathList.Add(file);
                Debug.Log(file + " accepted and added to list");
            }
            else
                Debug.Log(file + " extension not accepted.");
        }
    }

    #endregion



    #region Player Data File IO Methods

    public void LoadPlayerData(string path) //what info goes in args? file? somethin else?
    {
        throw new System.NotImplementedException();
        //open file using path

        //deserialize

        //close file
    }


    public void SavePlayerData(string path)
    {
        throw new System.NotImplementedException();
        //System.IO.FileStream fstream = System.IO.File.Create(path);

        // System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        // binary format + serialize data here...

        //fstream.Close();
    }


    public void SavePlayerData(PlayerData_Session player_session)
    {
        SavePlayerData(player_session.SaveDataPath);
    }

    #endregion

    private PlayerData_Session GetRecord(PlayerInput p)
    {
        foreach(var r in _Data)
        {
            if (r.Info._Input == p) return r;
        }
        return null;
    }

    /// <summary>
    /// Activates specified player. Activated players are the inputs to be used during gameplay.
    /// </summary>
    /// <param name="p">player to activate</param>
    /// <returns>True if player was successfully activated.</returns>
    public bool ActivatePlayer(PlayerInput p)
    {
        var r = GetRecord(p);
        if (r != null)
        {
            _ActivatedPlayers.Add(r);

            return true;
        }
        return false;
    }
}