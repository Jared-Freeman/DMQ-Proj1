using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorStats : MonoBehaviour
{

    #region members

    public bool FLAG_Debug = false;

    public ActorSystem.ActorStatsPreset Preset;


    //probably want to increase protection on current (state) variables
    [Header("Current Values")]
    public float HpCurrent;
    public float EnergyCurrent;

    // DEPRECATED

    ////how do we impl this to not mess with stuff like damage over time (DoT)?
    ////Should we make invuln into a comm channel sort of thing? 
    ////(?) enum InvulnerabilityHandling { Normal, IgnoreInvulnerabilityFrames, WaitUntilInvulnerabilityFramesOver }
    //[Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
    //public float invulnerabiltyTime;

    //public int atk = 0;
    //public int def = 0;

    ////later stuff
    //int buffAtk = 0;
    //int buffDef = 0;
    //public int totalAtk = 0;
    //public int totalDef = 0;

    public float m_timeSinceLastHit = 0.0f;
    protected Collider m_Collider;

    System.Action schedule;
    #endregion

    public bool isInvulnerable { get; set; }

    public UnityEvent OnDeath, OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable, OnResetDamage;



    //refs
    Actor actor;

    void Start()
    {
        ResetDamage();
        m_Collider = GetComponent<Collider>();
        actor = GetComponent<Actor>();
    }
    public void ResetDamage()
    {
        HpCurrent = Preset.Options.HpMax;
        EnergyCurrent = Preset.Options.EnergyMax;
        isInvulnerable = false;
        m_timeSinceLastHit = 0.0f;
        OnResetDamage.Invoke();
    }

    //DEPRECATED
    //public void CalculateStats()
    //{
    //    totalAtk = atk + buffAtk;
    //    totalDef = def + buffDef;
    //}

    public void ApplyDamage(DamageMessage data)
    {
        Debug.Log("false Taken");

        if (HpCurrent <= 0)
        {//ignore damage if already dead. TODO : may have to change that if we want to detect hit on death...
            return;
        }
        
        if (isInvulnerable && !data.FLAG_IgnoreInvulnerability)
        {
            OnHitWhileInvulnerable.Invoke();
            return;
        }

        Vector3 forward = transform.forward;

        //we project the direction to damager to the plane formed by the direction of damage
        Vector3 positionToDamager = data.damageSource - transform.position;
        positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);

        isInvulnerable = true;
        m_timeSinceLastHit = 1.0f;

        HpCurrent -= Mathf.Max(data.amount, 0);

        if (HpCurrent <= 0)
        {
            schedule += OnDeath.Invoke; //This avoid race condition when objects kill each other.
            actor.ActorDead();
        }
        else
        {
            OnReceiveDamage.Invoke();
        }


    }

    //TODO: Make more robust
    public void ApplyDamage(Actor_DamageMessage DamageMessage)
    {
        if(FLAG_Debug) Debug.Log("Damage Taken");

        if (HpCurrent <= 0)
        {//ignore damage if already dead. TODO : may have to change that if we want to detect hit on death...
            return;
        }

        if(DamageMessage._Team == null)
        {
            if (FLAG_Debug) Debug.Log("No team found on message packet");

            HpCurrent -= Mathf.Max(DamageMessage._DamageInfo.DamageAmount, 0);
            OnReceiveDamage.Invoke();
        }
        //target filtering
        else if(DamageMessage._DamageInfo.TargetFilters.TargetIsAllowed(DamageMessage._Team, actor))
        {
            if (FLAG_Debug) Debug.Log("Target filters validated message packet. Damage taken.");

            HpCurrent -= Mathf.Max(DamageMessage._DamageInfo.DamageAmount, 0);
            OnReceiveDamage.Invoke();
        }

        if (HpCurrent <= 0)
        {
            schedule += OnDeath.Invoke; //This avoid race condition when objects kill each other.
            actor.ActorDead();
        }
    }
}
[System.Serializable]
public struct DamageMessage
{
    public MonoBehaviour damager;
    public float amount;
    public Vector3 direction;
    public Vector3 damageSource;

    public bool stopCamera;

    public bool FLAG_IgnoreInvulnerability; //Useful for consistent DOT damage. Default (because struct) == FALSE
}