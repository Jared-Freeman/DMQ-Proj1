using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils.Stats;

using ActorSystem.StatusEffect;

public class ActorStats : MonoBehaviour
{

    #region Members

    public bool FLAG_Debug = false;

    /// <summary>
    /// Reference to the asset containing this Actor's stat defaults.
    /// </summary>
    public ActorSystem.ActorStatsPreset Preset;

    public List<SE_StatusEffect_Base> InitialStatusEffects = new List<SE_StatusEffect_Base>();

    protected List<SE_StatusEffect_Instance> _ListStatusEffects = new List<SE_StatusEffect_Instance>();
    public IReadOnlyCollection<SE_StatusEffect_Instance> StatusEffects
    {
        get
        {
            return _ListStatusEffects?.AsReadOnly();
        }
    }

    Actor actor;
    System.Action schedule;

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

    public float m_timeSinceLastHit = 0.0f; //Can we deprecate this or discuss this functionality? The current implementation was blocking features like damage over time -Jared

    //protected Collider m_Collider; //why was this here??

    protected StatInstance HP;
    protected StatInstance Energy;
    protected StatInstance MoveSpeed;

    #region Properties

    //public float HpCurrent
    //{
    //    get { return (HP.Value + HP.Modifier.Add) * HP.Modifier.Multiply; }
    //    protected set 
    //    { 
    //        HP.Value = value;         
    //    }
    //}
    //public float EnergyCurrent
    //{
    //    get { return (Energy.Value + Energy.Modifier.Add) * Energy.Modifier.Multiply; }
    //    protected set { Energy.Value = value; }
    //}
    //public float MoveSpeedCurrent
    //{
    //    get { return (MoveSpeed.Value + MoveSpeed.Modifier.Add) * MoveSpeed.Modifier.Multiply; }
    //    protected set { MoveSpeed.Value = value; }
    //}

    public float HpCurrent
    {
        get { return HP.Value; }
        protected set { HP.Value = value; }
    }
    public float EnergyCurrent
    {
        get { return Energy.Value; }
        protected set { Energy.Value = value; }
    }
    public float MoveSpeedCurrent
    {
        get { return MoveSpeed.Value; }
        protected set { MoveSpeed.Value = value; }
    }

    public bool isInvulnerable { get; set; }

    #endregion

    #endregion

    #region Events

    public UnityEvent OnDeath, OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable, OnResetDamage;

    #endregion

    #region Initialization
    void Awake()
    {
        actor = GetComponent<Actor>();
        //m_Collider = GetComponent<Collider>();
    }

    void Start()
    {
        ResetDamage();

        foreach(var e in InitialStatusEffects)
        {
            AddStatusEffect(e.CreateInstance(gameObject));
        }
    }
    public void ResetDamage()
    {
        HpCurrent = Preset.Data.HP.Default.Max;
        EnergyCurrent = Preset.Data.Energy.Default.Max;
        isInvulnerable = false;
        m_timeSinceLastHit = 0.0f;
        OnResetDamage.Invoke();
    }

    #endregion

    /// <summary>
    /// Recalculates ActorStat current values based on ALL current status effects
    /// </summary>
    protected void RecalculateStatusMutations()
    {
        ResetStatusMutation();

        //recalculate per-status
        foreach (var m in _ListStatusEffects)
        {
            AddStatusEffect(m);
        }
    }

    /// <summary>
    /// Appends a stat data modifier into this ActorStat's modifier buffer
    /// </summary>
    /// <param name="append_data"></param>
    public void AddStatusEffect(SE_StatusEffect_Instance Effect)
    {
        _ListStatusEffects.Add(Effect);

        Effect.OnStatusEffectDestroy_Local += Effect_OnStatusEffectDestroy_Local;

        var append_data = Effect.Preset.Settings.StatsModifiers;

        HP.Modifier.Add += append_data.HP.Modifier.Add;
        Energy.Modifier.Add += append_data.Energy.Modifier.Add;
        MoveSpeed.Modifier.Add += append_data.MoveSpeed.Modifier.Add;

        HP.Modifier.Multiply *= append_data.HP.Modifier.Multiply;
        Energy.Modifier.Multiply *= append_data.Energy.Modifier.Multiply;
        MoveSpeed.Modifier.Multiply *= append_data.MoveSpeed.Modifier.Multiply;
    }

    private void Effect_OnStatusEffectDestroy_Local(object sender, CSEventArgs.StatusEffect_Actor_EventArgs e)
    {
        if(e._Actor == this)
        {
            RemoveStatusMutation(e._StatusEffect);
        }

        //not sure if this unsubscription is needed but does FEEL safer.
        e._StatusEffect.OnStatusEffectDestroy_Local -= Effect_OnStatusEffectDestroy_Local;
    }

    /// <summary>
    /// Removes a stat data modifier into this ActorStat's modifier buffer
    /// </summary>
    /// <param name="append_data"></param>
    protected void RemoveStatusMutation(SE_StatusEffect_Instance Effect)
    {
        _ListStatusEffects.Remove(Effect);

        var append_data = Effect.Preset.Settings.StatsModifiers;

        HP.Modifier.Add -= append_data.HP.Modifier.Add;
        Energy.Modifier.Add -= append_data.Energy.Modifier.Add;
        MoveSpeed.Modifier.Add -= append_data.MoveSpeed.Modifier.Add;

        HP.Modifier.Multiply /= append_data.HP.Modifier.Multiply;
        Energy.Modifier.Multiply /= append_data.Energy.Modifier.Multiply;
        MoveSpeed.Modifier.Multiply /= append_data.MoveSpeed.Modifier.Multiply;
    }


    /// <summary>
    /// Reset Status mutation to default values
    /// </summary>
    protected void ResetStatusMutation()
    {
        HP.Modifier.Add = Preset.Data.HP.Modifier.Add;
        HP.Modifier.Multiply = Preset.Data.HP.Modifier.Multiply;

        Energy.Modifier.Add = Preset.Data.Energy.Modifier.Add;
        Energy.Modifier.Multiply = Preset.Data.Energy.Modifier.Multiply;

        MoveSpeed.Modifier.Add = Preset.Data.MoveSpeed.Modifier.Add;
        MoveSpeed.Modifier.Multiply = Preset.Data.MoveSpeed.Modifier.Multiply;
    }

    //TODO: Make more robust
    public void ApplyDamage(Actor_DamageMessage DamageMessage)
    {
        if(FLAG_Debug) Debug.Log("Damage Taken");

        if (HpCurrent <= 0)
        {//ignore damage if already dead. TODO : may have to change that if we want to detect hit on death...
            return;
        }


        //if no team was sent with this message, just take the damage.
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

    #region Deprecated

    //DEPRECATED
    //public void CalculateStats()
    //{
    //    totalAtk = atk + buffAtk;
    //    totalDef = def + buffDef;
    //}

    /// <summary>
    /// DEPRECATED. Please use new Actor_DamageMessage arg impl!
    /// </summary>
    /// <param name="data"></param>
    public void ApplyDamage(DamageMessage data)
    {
        if (FLAG_Debug) Debug.Log("Damge Taken in deprecated function!");

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

    #endregion

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