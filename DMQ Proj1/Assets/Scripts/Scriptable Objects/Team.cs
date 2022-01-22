using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class TeamEventArgs : System.EventArgs
    {
        public Team _Team;
        public TeamEventArgs(Team t)
        {
            _Team = t;
        }
    }
}


//TODO: This is NOT correct... Find a good method to generate a list of all Team SO's
[CreateAssetMenu(fileName = "Team", menuName = "ScriptableObjects/Team", order = 1)]
public class Team : ScriptableObject
{

    private static int m_teamIDGen = 0;

    public int TeamID { get; } = m_teamIDGen++;
    public string TeamName;

    //We assume all Teams are Enemy by default, so we don't track that info
    public List<Team> AllyList;
    public List<Team> NeutralList;

    public ScriptOptions Options;

    #region Helpers
    [System.Serializable]
    public struct ScriptOptions
    {
        public int NoCollideLayer;
        public int Layer;
    }
    #endregion

    //---------- USEFUL METHODS ---------------------

    //Determining alliance
    public bool IsEnemy(Team other)
    {
        if (IsSelf(other)) return false;
        if (IsAlly(other)) return false;
        if (IsNeutral(other)) return false; //maybe just return !isNeutral
        return true;
    }
    //Not to be confused with Self
    public bool IsAlly(Team other)
    {
        if (IsSelf(other)) return false;
        foreach (Team T in AllyList)
        {
            if (T == other) return true;
        }
        return false;
    }
    public bool IsNeutral(Team other)
    {
        if (IsSelf(other)) return false;
        foreach (Team T in NeutralList)
        {
            if (T == other) return true;
        }
        return false;
    }
    public bool IsSelf(Team other)
    {
        if (other == this) return true;
        return false;
    }

}
