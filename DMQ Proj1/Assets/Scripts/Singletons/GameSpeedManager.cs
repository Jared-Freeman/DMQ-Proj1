using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wraps all UnityEngine Time mutations.
/// </summary>
/// <remarks>
///  Enables game speed management to be on a priority-basis i.e. GameState pause is more important than a time-slowing script.
/// </remarks>
public class GameSpeedManager : Singleton<GameSpeedManager>
{
    #region Members

    [SerializeField] private int DefaultTimePriority = 128;

    private float DefaultTimeScale { get; set; }
    public MonoBehaviour CurrentTokenHolder { get; private set; } = null;

    public List<TimePriority> List_TimePriorities = new List<TimePriority>();

    #region Helpers

    [System.Serializable]
    public class TimePriority
    {
        public MonoBehaviour Instance;
        [Header("Lower values == higher priority")]
        public int Priority;
    }

    #endregion

    #endregion

    #region Initialization

    void Start()
    {
        DefaultTimeScale = Time.timeScale;
    }

    #endregion

    #region Internal Utilities

    /// <summary>
    /// Gets record from list, if one exists.
    /// </summary>
    /// <param name="m"></param>
    /// <returns>the TimePriority record for <paramref name="m"/>, or null if none exists.</returns>
    private TimePriority GetPriorityRecord(MonoBehaviour m)
    {
        foreach(var r in List_TimePriorities)
        {
            if (r.Instance == m) return r;
        }

        return null;
    }

    /// <summary>
    /// removes dangling refs from RecordList
    /// </summary>
    private void ValidateRecordList()
    {
        List<TimePriority> removeList = new List<TimePriority>();
        foreach(var r in List_TimePriorities)
        {
            if (r.Instance == null) removeList.Add(r);
        }

        foreach(var r in removeList)
        {
            List_TimePriorities.Remove(r);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Attempts to acquire the time control token
    /// </summary>
    /// <param name="invokingComponent"></param>
    /// <returns>True, if the token was acquired</returns>
    /// <remarks>
    /// Acquiring the token will reset the timescale to default. 
    /// Call the ModifyTimeScale method on <see cref="GameSpeedManager"/> after this method to control timescale.
    /// </remarks>
    public bool AcquireTimeControlToken(MonoBehaviour invokingComponent)
    {
        var invokingR = GetPriorityRecord(invokingComponent);
        int newPriority = DefaultTimePriority;

        var curR = GetPriorityRecord(CurrentTokenHolder);
        int curPriority = DefaultTimePriority;

        //assign custom priorities if they exist
        if (invokingR != null)
        {
            newPriority = invokingR.Priority;
        }
        if (curR != null)
        {
            curPriority = curR.Priority;
        }

        //if new priority is greater than current, token is acquired
        if (newPriority > curPriority)
        {
            RelinquishTimeControlToken(CurrentTokenHolder);
            CurrentTokenHolder = invokingComponent;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Cleans up timescale after a component relinquishes its control.
    /// </summary>
    /// <param name="invokingComponent">Monobehavior caller of this method.</param>
    /// <remarks>
    /// Control may be given up voluntarily, or forced by the <see cref="GameSpeedManager"/>
    /// </remarks>
    public void RelinquishTimeControlToken(MonoBehaviour invokingComponent)
    {
        if (invokingComponent == CurrentTokenHolder)
        {
            Time.timeScale = DefaultTimeScale;
            CurrentTokenHolder = null;
        }
    }

    /// <summary>
    /// Attempts to modify the time scale. 
    /// </summary>
    /// <param name="invokingComponent">caller of this method.</param>
    /// <param name="newTimeScale"></param>
    /// <returns>True, if time scale was modified.</returns>
    /// <remarks>
    /// <paramref name="invokingComponent"/> must be the current token holder to succeed.
    /// </remarks>
    public bool ModifyTimeScale(MonoBehaviour invokingComponent, float newTimeScale)
    {
        if(invokingComponent == CurrentTokenHolder)
        {
            Time.timeScale = newTimeScale;
            return true;
        }
        return false;
    }

    #endregion
}
