using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Maintains a list of all Team Scriptable Objects
/// </summary>
public class TeamManager : Singleton<TeamManager>
{
    #region Members

    #region Events
    public static event System.EventHandler<CSEventArgs.TeamEventArgs> OnTeamAdded;
    public static event System.EventHandler<CSEventArgs.TeamEventArgs> OnTeamRemoved;
    #endregion

    public bool FLAG_Debug = false;

    [Header("Hardcoded to load from Assets/Resources/Teams")]
    public List<Team> AllTeams = new List<Team>();

    #endregion


    #region Initialization

    protected override void Awake()
    {
        base.Awake();
        var T_List = Resources.LoadAll<Team>("Teams").ToList();

        foreach (Team t in T_List)
        {
            AllTeams.Add(t);

            if (FLAG_Debug) Debug.Log("Team Asset Loaded: " + t.TeamName);
        }

        ValidateTeamNames();

        //Initialized list is finalized. Invoke Team added event
        foreach (Team t in AllTeams)
        {
            OnTeamAdded?.Invoke(this, new CSEventArgs.TeamEventArgs(t));
        }
    }

    private void ValidateTeamNames()
    {
        //temp list
        var ToBeValidated = new List<Team>();
        foreach (Team t in AllTeams) ToBeValidated.Add(t);

        //re-add items, ignoring duplicate names
        AllTeams.Clear();

        foreach (Team t in ToBeValidated)
        {
            if (GetTeam(t.TeamName) == null)
            {
                AllTeams.Add(t);
            }
            else
            {
                Debug.LogError("Duplicate TeamName Detected: <" + t.TeamName + "> -- Please ensure no duplicate team names exist in the Resources/Teams folder. Ignoring this Team!");
            }
        }
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }
    #endregion


    #region Events
    #region Event Subscriptions
    void SubscribeToEvents()
    {
    }
    void UnsubscribeToEvents()
    {
    }
    #endregion

    #region Event Handlers
    #endregion
    #endregion


    #region Methods

    public bool RemoveTeam(Team t)
    {
        if (AllTeams.Remove(t))
        {
            OnTeamRemoved?.Invoke(this, new CSEventArgs.TeamEventArgs(t));
            return true;
        }
        return false;
    }
    public bool RemoveTeam(string Name)
    {
        var t = GetTeam(Name);
        if(t != null)
        {
            return RemoveTeam(t);
        }
        return false;
    }

    public Team GetTeam(string Name)
    {
        if (FLAG_Debug) Debug.Log("Searching for team: " + Name);

        Team tm = null;

        foreach(Team t in AllTeams)
        {
            if(t.TeamName == Name)
            {
                tm = t;
            }
        }

        return tm;
    }

    public void PrintAllTeamNames()
    {
        System.Text.StringBuilder SB = new System.Text.StringBuilder();
        foreach(Team T in AllTeams)
        {
            SB.Append(T.TeamName + ", ");
        }
        SB.Remove(SB.Length - 2, 2); //remove last ", "

        Debug.Log("All teams: " + SB.ToString());
    }

    #endregion
}
