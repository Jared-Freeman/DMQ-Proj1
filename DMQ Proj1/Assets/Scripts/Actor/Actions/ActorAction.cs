using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorActionType { DealDamage, Action2 }; //placeholder actions. Gotta figure out how we're handling defining the set of all actions still


public enum ActorDamageType { Internal, Kinetic, Fire, Frost, Acid }; //placeholder damage types. Possibly a good system to delineate resistance/weakness if we wanna go down that route
//A container class for a standardized description of damage
public class ActorDamage
{
    public ActorDamageType DamageType = ActorDamageType.Internal;
    public ActorAction Action;
    public float DamageAmount = 0;

    ActorDamage() { }
    ActorDamage(ActorAction CreatorAction, float DamageToDeal = 0, ActorDamageType TypeOfDamage = ActorDamageType.Internal)
    {
        Action = CreatorAction;
        DamageType = TypeOfDamage;
        DamageAmount = DamageToDeal;
    }
};


//This class implements an INSTANCE of an action to be executed, and contains the relevant state data needed to carry out that action
public class ActorAction : MonoBehaviour
{
    #region Members
    public Actor OwningActor;
    #endregion

    #region Init
    protected void Start()
    {
        
    }
    #endregion

    #region Static Methods
    public static ActorAction AddActionToActorActions(Actor CreatorActor)
    {
        ActorAction Act = new ActorAction();

        Act.OwningActor = CreatorActor;
        CreatorActor.Actions.Add(Act); //Do we do this here??? Would that be overstepping class responsibilities???

        return Act;
    }
    #endregion
}
