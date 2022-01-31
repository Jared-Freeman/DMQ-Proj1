using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ClassSystem;

public class Debug_ClassSystemSelection : MonoBehaviour
{
    public PlayerInputManager _PlayerInputMGR;
    public List<GameObject> _PlayerList = new List<GameObject>();

    

    public enum ClassContainer { Interdictor, Bulwark, Arcanist, Enchanter }
    public ClassContainer ClassToSpawn;
    public ClassContainerOptions Options;

    [System.Serializable]
    public struct ClassContainerOptions
    {
        [Range(0, 4)]
        public int PlayersToSpawn;

        //misnomers now...
        public CharacterClass Prefab_Interdictor;
        public CharacterClass Prefab_Bulwark;
        public CharacterClass Prefab_Arcanist;
        public CharacterClass Prefab_Enchanter;
    }


    private void OnEnable()
    {
        _PlayerInputMGR.onPlayerJoined += _PlayerInputMGR_onPlayerJoined;

        //yes this is dumb
        for(int i=0; i<Options.PlayersToSpawn; i++)
        {
            switch(i)
            {
                case 0:
                    SpawnClassPrefab(ClassContainer.Arcanist);
                    break;
                case 1:
                    SpawnClassPrefab(ClassContainer.Enchanter);
                    break;
                case 2:
                    SpawnClassPrefab(ClassContainer.Interdictor);
                    break;
                case 3:
                    SpawnClassPrefab(ClassContainer.Bulwark);
                    break;
            }
        }
    }
    private void OnDisable()
    {
        _PlayerInputMGR.onPlayerJoined -= _PlayerInputMGR_onPlayerJoined;
    }


    private void SpawnClassPrefab(ClassContainer c)
    {
        switch(c)
        {
            case ClassContainer.Interdictor:
                DoInstantiation(Options.Prefab_Interdictor);
                break;
            case ClassContainer.Bulwark:
                DoInstantiation(Options.Prefab_Bulwark);
                break;
            case ClassContainer.Arcanist:
                DoInstantiation(Options.Prefab_Arcanist);
                break;
            case ClassContainer.Enchanter:
                DoInstantiation(Options.Prefab_Enchanter);
                break;
            default:
                Debug.LogError("Unrecognized class!");
                break;
        }
    }
    private void DoInstantiation(CharacterClass c)
    {
        GameObject g = c.InstantiatePlayerActor();
        g.transform.position = transform.position;
        g.transform.position += new Vector3(UnityEngine.Random.Range(-3, 3), 0, 0);
    }

    /// <summary>
    /// Reference code from official documentation: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Users.InputUser.html#events
    /// I think this is worth leveraging for doing runtime pairing
    /// </summary>
    private void UnpairedDeviceHandlingExample()
    {
        // Activate support for listening to device activity.
        ++UnityEngine.InputSystem.Users.InputUser.listenForUnpairedDeviceActivity;

        // When a button on an unpaired device is pressed, pair the device to a new
        // or existing user.
        UnityEngine.InputSystem.Users.InputUser.onUnpairedDeviceUsed +=
            (usedControl, ptr) =>
            {
                // Only react to button presses on unpaired devices.
                if (!(usedControl is UnityEngine.InputSystem.Controls.ButtonControl))
                    return;

                // Pair the device to a user.
                UnityEngine.InputSystem.Users.InputUser.PerformPairingWithDevice(usedControl.device);
            };
    }



    private void _PlayerInputMGR_onPlayerJoined(PlayerInput obj)
    {
        Debug.Log(ToString() + " reporting player joined: " + obj.ToString());
        _PlayerList.Add(obj.gameObject);
    }

}
