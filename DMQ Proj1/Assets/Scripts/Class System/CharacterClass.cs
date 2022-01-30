using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preset for a Character Class
/// </summary>
public class CharacterClass : ScriptableObject
{
    /// <summary>
    /// Container for the rules this class follows, such as their set of useable stuff (perhaps), base stats, etc.
    /// </summary>
    [System.Serializable]
    public struct ClassInfo
    {

    }

    public string ClassName;
    [Multiline]
    public string DisplayInfo;
}
