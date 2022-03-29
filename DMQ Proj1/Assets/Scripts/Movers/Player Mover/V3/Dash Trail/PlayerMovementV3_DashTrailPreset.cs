using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preset for <see cref="PlayerMovementV3_DashTrail"/>
/// </summary>
[CreateAssetMenu(fileName = "PMV3_DashTrail_", menuName = "Actor/Player/PlayerMovementV3 Dash Trail", order = 2)]
public class PlayerMovementV3_DashTrailPreset : ScriptableObject
{
    public GameObject ParticleSystemPrefab;
}
