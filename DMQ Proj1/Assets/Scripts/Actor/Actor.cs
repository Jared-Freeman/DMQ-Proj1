using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorStats))]
public class Actor : MonoBehaviour
{

    #region members

    private static int IdGenerator = 0;
    
    readonly public int ActorID = IdGenerator++;

    public bool Flag_ActorDebug = false;


    public ActorStats Stats;
    public List<ActorAction> Actions;
    

    #endregion
    
    protected void Start()
    {
        if (Flag_ActorDebug) Debug.Log("Base Actor Start()");
        if (Flag_ActorDebug) Debug.Log("Base Actor ID: " + ActorID);

        Stats = GetComponent<ActorStats>();
    }

}
