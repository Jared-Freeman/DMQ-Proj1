using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preset used with <see cref="DamageNumberEventHandler"/>
/// </summary>
[CreateAssetMenu(fileName = "DamageNumberPreset_", menuName = "ScriptableObjects/Damage Number Preset", order = 1)]
public class DamageNumberEventHandlerPreset : ScriptableObject
{
    public Color ColorHealing = Color.green;
    public Color ColorDamage = Color.red;

    public float TextDurationMin = 1f;
    public float TextDurationMax = 3f;


}
