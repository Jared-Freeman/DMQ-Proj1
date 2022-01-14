using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Designed to contain any and all information needed to apply damage to ANY target. 
/// 
/// Expand this class as needed, but try not to add variables unless ABSOLUTELY NECESSARY since this is designed to be a lightweight packet of data
/// 
/// Extending ScriptableObject allows most of the data to be kept in a catalogue (library of preloaded values)
/// 
/// </summary>
[CreateAssetMenu(fileName = "DamageMessage", menuName = "ScriptableObjects/Damage Message", order = 1)]
public class AP2_DamageMessage : ScriptableObject
{
    public GameObject Caster; //i.e. object that cast the fireball
    public Vector3 CasterOffset = Vector3.zero; //optional offset from origin

    public GameObject DamageSource; //i.e. the fireball
    public Vector3 DamageSourceOffset = Vector3.zero; //optional offset from origin

    public float DamageAmount;
}
