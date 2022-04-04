using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class ActorEventArgs : System.EventArgs
    {
        public Actor _Actor;
        
        public ActorEventArgs(Actor a)
        {
            _Actor = a;
        }
    }
}

[RequireComponent(typeof(ActorStats))]
public class Actor : MonoBehaviour
{
    #region EVENTS

    public static event System.EventHandler<CSEventArgs.ActorEventArgs> OnActorCreated;
    public static event System.EventHandler<CSEventArgs.ActorEventArgs> OnActorDestroyed;
    public static event System.EventHandler<CSEventArgs.ActorEventArgs> OnActorDead;

    #endregion

    #region members

    private static int IdGenerator = 0;
    
    readonly public int ActorID = IdGenerator++;

    public bool Flag_ActorDebug = false;

    public Team _Team;
    public ActorStats Stats { get; private set; }

    //DEPRECATED. DO NOT USE
    public List<ActorAction> Actions { get; private set; }
    //public List<ActorStatusEffect> StatusEffects; //deprecated

    #endregion
    protected virtual void Awake() { }

    protected virtual void Start()
    {
        if (Flag_ActorDebug) Debug.Log("Base Actor Start()");
        if (Flag_ActorDebug) Debug.Log("Base Actor ID: " + ActorID);

        Stats = GetComponent<ActorStats>();
        if (Stats == null) Debug.LogError("No ActorStats found!");

        OnActorCreated?.Invoke(this, new CSEventArgs.ActorEventArgs(this));
    }


    protected virtual void OnDestroy()
    {
        OnActorDestroyed?.Invoke(this, new CSEventArgs.ActorEventArgs(this));

        // This doesnt work! OnDestroy() is invoked IMMEDIATELY BEFORE the game object is destroyed as essentially an event handler. 
        // You may want to invoke DestroyAfterSeconds in ActorDead() instead - Jared
        //StartCoroutine(DestroyAfterSeconds(2.0f)); //2 seconds for now?
    }

    protected void Update()
    {
        if (Stats.m_timeSinceLastHit > 0)
            Stats.m_timeSinceLastHit -= Time.deltaTime;
        else
        {
            Stats.isInvulnerable = false;
        }
    }
    public virtual void ActorDead()
    {
        // Actor has run out of HP. Probably want to have an action for handling this later.
        if(Flag_ActorDebug) Debug.Log(gameObject.name + " is dead");

        //TODO: Consider how to handle this!

        //Dispatch an event to AI animator proxy and destroy gameobject after a few seconds
        //OnDestroy();

        OnActorDead?.Invoke(this, new CSEventArgs.ActorEventArgs(this));
        Destroy(gameObject); // could delay using coroutine here if we wanted.

        //gameObject.SetActive(false);
    }
    protected IEnumerator DestroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    //Wasn't sure where else to put this but I figure every actor will need this function. 
    // This functionality is added in ActorStats.cs ! ~Jared
    /// <summary>
    /// DEPRECATED!!!!! Please dont use!
    /// </summary>
    /// <param name="DamageTaken"></param>
    public void TakeDamage(ActorDamage DamageTaken)
    {
        ////Take damage
        //Stats.HpCurrent -= DamageTaken.DamageAmount;
        ////Check for status effects depending on type of damage

        ////Check if HP has run out
        //if(Stats.HpCurrent <= 0f)
        //{
        //    ActorDead();
        //}
            
    }

    public EffectTree.EffectContext GetDefaultEffectContext()
    {
        Utils.AttackContext ac = new Utils.AttackContext()
        {
            _InitialDirection = transform.forward,
            _InitialGameObject = gameObject,
            _InitialPosition = transform.position,
            _TargetDirection = transform.forward,
            _TargetGameObject = transform.gameObject,
            _TargetPosition = transform.position,
            _Owner = this,
            _Team = _Team
        };

        EffectTree.EffectContext ctx = new EffectTree.EffectContext(ac);

        return ctx;
    }
}
