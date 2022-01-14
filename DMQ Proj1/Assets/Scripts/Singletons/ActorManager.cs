using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Maintains a list of all Actors
/// </summary>
public class ActorManager : Singleton<ActorManager>
{
    //[Header("Tracks a list of all active Actors")]

    public bool FLAG_Debug = false;

    public List<Actor> ActorList = new List<Actor>();

    #region Initialization
    protected override void Awake()
    {
        base.Awake();

        //ActorList.Clear();
        //PrintActorList();

        ////add any actors, if they exist
        //List<Actor> DanglingActors = FindObjectsOfType<Actor>().ToList();
        //foreach(Actor a in DanglingActors)
        //{
        //    if (FLAG_Debug) Debug.Log("ADDED DANLGING ACTOR: " + a.name);
        //    ActorList.Add(a);
        //}
    }

    private void OnEnable()
    {
        EventSubscribe();
    }
    private void OnDisable()
    {
        EventUnsubscribe();
    }
    #endregion

    public void PrintActorList()
    {

        print("Count = " + ActorList.Count + ", Cap = " + ActorList.Capacity);
        foreach(Actor a in ActorList)
        {
            if (a == null) print("null found in actorlist!");
            print(a.name);
        }
    }

    #region Events

    #region Event Subscriptions

    void EventSubscribe()
    {
        Actor.OnActorCreated += AddActorToList;
        Actor.OnActorDestroyed += RemoveActorFromList;
    }
    void EventUnsubscribe()
    {
        Actor.OnActorCreated -= AddActorToList;
        Actor.OnActorDestroyed -= RemoveActorFromList;
    }

    #endregion

    #region Event Handlers

    private void AddActorToList(object caller, CSEventArgs.ActorEventArgs args)
    {
        if (FLAG_Debug) Debug.Log("ADDED <event> ACTOR: " + args._Actor.name);
        ActorList.Add(args._Actor);
    }
    private void RemoveActorFromList(object caller, CSEventArgs.ActorEventArgs args)
    {
        if (FLAG_Debug) Debug.Log("REMOVED <event> ACTOR: " + args._Actor.name);
        ActorList.Remove(args._Actor);
    }

    #endregion

    #endregion


}
