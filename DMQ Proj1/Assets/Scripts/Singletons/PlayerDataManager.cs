using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that maintains a list of all loaded player session data. Implements Loading and Saving player session data.
/// </summary>
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    //Static members
    private static string s_ProfileSubfolderPath = "/PlayerProfiles";
    private static string s_ProfileSaveFileExtension = ".psave"; //player profile save "psave" idk lol


    #region Members

    //Save files list. Should these be shared publicly?
    private List<string> _ProfileSavePathList;

    //Data
    private List<PlayerData_Session> _Data;
    //Public facing
    public IReadOnlyCollection<PlayerData_Session> ActivePlayerProfiles { get { return _Data.AsReadOnly(); } }



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

        VerifyProfileDirectoryIntegrity(); //Guarantees <PlayerProfilePath> exists

        Test_GenerateSave();

        InitProfileFileList();
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

}