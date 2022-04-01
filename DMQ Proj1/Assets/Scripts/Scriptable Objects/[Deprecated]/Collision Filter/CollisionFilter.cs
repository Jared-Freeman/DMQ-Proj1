using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// Data structure for specifying checks on collisions
/// </summary>
//[CreateAssetMenu(fileName = "CF_", menuName = "ScriptableObjects/Collsion Filter", order = 1)]
public class CollisionFilter : ScriptableObject
{
    public TargetFilterOptions TargetFilters = new TargetFilterOptions();
    public CF_Filters AdditionalFilters;

    [System.Serializable]
    public struct CF_Filters
    {

    }

    public bool TriggerAllowed(Collider collider, CollisionFilterContext ctx)
    {
        if(TargetFilterChecks(collider.gameObject.GetComponent<Actor>(), ctx, out bool Result))
        {
            return Result;
        }

        //TODO: CF_Filter checks (whenever new Filters get added anyway...)

        return true;
    }

    public bool CollisionAllowed(Collision collision, CollisionFilterContext ctx)
    {
        if (TargetFilterChecks(collision.gameObject.GetComponent<Actor>(), ctx, out bool Result))
        {
            Debug.Log("RESULT: " + Result.ToString());
            return Result;
        }

        //TODO: CF_Filter checks (whenever new Filters get added anyway...)

        return true;
    }

    /// <summary>
    /// Returns true if a Target Filter check was successfully processed
    /// </summary>
    /// <param name="a"></param>
    /// <param name="ctx"></param>
    /// <param name="Result"></param>
    /// <returns></returns>
    protected bool TargetFilterChecks(Actor a, CollisionFilterContext ctx, out bool Result)
    {
        if (a != null)
        {
            if (ctx._Team != null)
            {
                Result = TargetFilters.TargetIsAllowed(ctx._Team, a);
                return true;
            }
            else if (ctx._Actor != null)
            {
                Result = TargetFilters.TargetIsAllowed(ctx._Actor._Team, a);
                return true;
            }
        }
        Result = false;
        return false;
    }
}

/// <summary>
/// Message struct for CollisionFilter 
/// </summary>
public struct CollisionFilterContext
{
    /// <summary>
    /// Creates a CollisionFilterContext for a CollisionFilter to use for interpreting a specific event
    /// </summary>
    /// <param name="gameobject"></param>
    /// <param name="actor"></param>
    /// <param name="team"></param>
    public CollisionFilterContext(GameObject gameobject, Actor actor, Team team)
    {
        _Team = team;
        _Actor = actor;
        _gameObject = gameobject;
    }

    public Team _Team;
    public Actor _Actor;
    public GameObject _gameObject;
}
