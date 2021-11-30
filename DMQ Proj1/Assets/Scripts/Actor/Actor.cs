using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorStats))]
public class Actor : MonoBehaviour
{
    #region EVENTS
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

    }

    protected void Update()
    {
        
    }

    //Wasn't sure where else to put this but I figure every actor will need this function. 
    public void TakeDamage(ActorDamage DamageTaken)
    {
        //Take damage
        Stats.HpCurrent -= DamageTaken.DamageAmount;
        //Check for status effects depending on type of damage

        //Check if HP has run out
        if(Stats.HpCurrent <= 0f)
        {
            // Actor has run out of HP. Probably want to have an action for handling this later.
            Debug.Log(gameObject.name + " is dead");
            gameObject.SetActive(false);
        }
            
    }
}
