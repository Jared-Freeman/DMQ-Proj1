using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorStats))]
public class Actor : MonoBehaviour
{
    #region EVENTS
    public static event System.EventHandler<MonobehaviourEventArgs> Event_ActorCreate;
    public static event System.EventHandler<MonobehaviourEventArgs> Event_ActorDestroy;
    #endregion
    #region members

    private static int IdGenerator = 0;
    
    readonly public int ActorID = IdGenerator++;

    public bool Flag_ActorDebug = false;


    public ActorStats Stats;
    public List<ActorAction> Actions;
    public List<ActorStatusEffect> StatusEffects;


    #endregion

    protected void Start()
    {
        if (Flag_ActorDebug) Debug.Log("Base Actor Start()");
        if (Flag_ActorDebug) Debug.Log("Base Actor ID: " + ActorID);

        Stats = GetComponent<ActorStats>();

        Event_ActorCreate.Invoke(this, new MonobehaviourEventArgs(this));
    }

    

    private void OnDestroy()
    {
        Event_ActorDestroy.Invoke(this, new MonobehaviourEventArgs(this));
    }

    protected void Update()
    {
        
    }
}
