﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorStats : MonoBehaviour
{

    #region members
    
    [Header("Default Values")]
    public float HpMax = 0;
    public float EnergyMax = 0;

    [Header("Current Values")]
    public float HpCurrent;
    public float EnergyCurrent;

    [Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
    public float invulnerabiltyTime;

    public int atk = 0;
    public int def = 0;

    //later stuff
    int buffAtk = 0;
    int buffDef = 0;
    public int totalAtk = 0;
    public int totalDef = 0;

    public float m_timeSinceLastHit = 0.0f;
    protected Collider m_Collider;

    System.Action schedule;
    #endregion

    public bool isInvulnerable { get; set; }

    public UnityEvent OnDeath, OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable, OnResetDamage;
    Actor actor;

    void Start()
    {
        ResetDamage();
        m_Collider = GetComponent<Collider>();
        CalculateStats();
        actor = GetComponent<Actor>();
    }
    public void ResetDamage()
    {
        HpCurrent = HpMax;
        EnergyCurrent = EnergyMax;
        isInvulnerable = false;
        m_timeSinceLastHit = 0.0f;
        OnResetDamage.Invoke();
    }
    public void CalculateStats()
    {
        totalAtk = atk + buffAtk;
        totalDef = def + buffDef;
    }

    public void ApplyDamage(DamageMessage data)
    {
        if (HpCurrent <= 0)
        {//ignore damage if already dead. TODO : may have to change that if we want to detect hit on death...
            return;
        }

        if (isInvulnerable)
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

        HpCurrent -= Mathf.Max(data.amount - totalDef, 0);

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
}
[System.Serializable]
public struct DamageMessage
{
    public MonoBehaviour damager;
    public int amount;
    public Vector3 direction;
    public Vector3 damageSource;

    public bool stopCamera;
}