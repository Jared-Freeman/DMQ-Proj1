using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem;

/// <summary>
/// 
/// Designed to contain any and all information needed to apply damage to ANY target. 
/// 
/// </summary>

// Expand this class as needed, but try not to add variables unless ABSOLUTELY NECESSARY since this is designed to be a lightweight packet of data
// Extending ScriptableObject allows most of the data to be kept in a catalogue (library of preloaded values)
[CreateAssetMenu(fileName = "DamageMessage", menuName = "ScriptableObjects/Damage Message", order = 1)]
public class AP2_DamageMessagePreset : ScriptableObject
{
    public float DamageAmount;

    public TargetFilterOptions TargetFilters;

    public Actor_DamageMessage CreateMessage(GameObject Caster, Team team, Vector3 CasterOffset, GameObject DamageSource, Vector3 DamageSourceOffset)
    {
        Actor_DamageMessage m = new Actor_DamageMessage();

        m._Caster = Caster;
        m._CasterOffset = CasterOffset;
        m._DamageSource = DamageSource;
        m._DamageSourceOffset = DamageSourceOffset;
        m._Team = team;

        m._DamageInfo = this;

        return m;
    }
}

/// <summary>
/// Container holding info on a specific damage instance
/// </summary>
public class Actor_DamageMessage
{
    
    /// <summary>
    /// Reference to the damage preset that created this instance. Gives access to damage amount, target filters, etc.
    /// </summary>
    public AP2_DamageMessagePreset _DamageInfo;

    // Info related to this SPECIFIC message
    public GameObject _Caster; //i.e. object that cast the fireball
    public Vector3 _CasterOffset = Vector3.zero; //optional offset from origin
    public GameObject _DamageSource; //i.e. the fireball
    public Vector3 _DamageSourceOffset = Vector3.zero; //optional offset from origin

    public Team _Team;
}

/// <summary>
/// Specifies who will receive this message. For use with Team Scriptable Object
/// </summary>
[System.Serializable]
public class TargetFilterOptions
{
    [Header("Checkbox == Damage message sent to this team (relative to owner of the message)")]
    public bool Enemy = true;
    public bool Ally = false;
    public bool Neutral = false;
    public bool Self = false;

    /// <summary>
    /// Returns true if Target is acceptable according to this team's perspective given the target filters
    /// </summary>
    /// <param name="InvokingTeam">Team this TargetFilter is being used relative to</param>
    /// <param name="Target">Target from the InvokingTeam's perspective</param>
    /// <returns></returns>
    public bool TargetIsAllowed(Team InvokingTeam, Actor Target)
    {
        if(Self && InvokingTeam.IsSelf(Target._Team))
        {
            return true;
        }
        else if (Ally && InvokingTeam.IsAlly(Target._Team))
        {
            return true;
        }
        else if (Neutral && InvokingTeam.IsNeutral(Target._Team))
        {
            return true;
        }
        else if (Enemy && InvokingTeam.IsEnemy(Target._Team))
        {
            return true;
        }

        return false;
    }
}