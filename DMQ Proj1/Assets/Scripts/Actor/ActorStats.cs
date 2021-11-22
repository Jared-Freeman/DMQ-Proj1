using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : MonoBehaviour
{

    #region members
    
    [Header("Default Values")]
    public float HpMax = 0;
    public float EnergyMax = 0;

    [Header("Current Values")]
    public float HpCurrent;
    public float EnergyCurrent;

    #endregion

    void Start()
    {
        HpCurrent = HpMax;
        EnergyCurrent = EnergyMax;
    }

}
