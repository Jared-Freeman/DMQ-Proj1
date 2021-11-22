using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : MonoBehaviour
{

    #region members
    
    public float HpMax = 0;
    public float EnergyMax = 0;

    
    public float HpCurrent;
    public float EnergyCurrent;

    #endregion

    void Start()
    {
        HpCurrent = HpMax;
        EnergyCurrent = EnergyMax;
    }

}
