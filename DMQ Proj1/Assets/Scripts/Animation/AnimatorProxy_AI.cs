using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem.AI;
public class AnimatorProxy_AI : AnimatorProxy
{
    void Start()
    {
        EventSubscribe();
    }
    void Update()
    {
        Debug.Log("Hello, tick?");
    }

    protected override void EventSubscribe()
    {
        //OnVelocityUpdate
        AIMover.OnVelocityUpdate += AIMover_OnVelocityUpdate;
        //OnAttack
        AILogic_Shambler.OnAbilityCast += AILogic_Shambler_OnAbilityCast;
        //On Hit
        ActorStats.OnDamageTaken += ActorStats_OnDamageTaken;
        //On Death
        Actor.OnActorDestroyed += Actor_OnActorDestroyed;
    }


    private void AIMover_OnVelocityUpdate(object sender, AIMoverEventArgs e)
    {
        if(e.actor == actor)
        {
            float velocity = e.velocity;
            if(velocity > 0.1f)
            {
                animator.SetFloat("Speed", velocity);
            }
        }
    }
    private void AILogic_Shambler_OnAbilityCast(object sender, AILogic_ShamblerEventArgs e)
    {
        if(e.actor == actor)
        {
            animator.SetTrigger("Ability");
            animator.SetInteger("AbilityNum", e.abilityIndex);
        }
    }

    private void ActorStats_OnDamageTaken(object sender, ActorSystem.EventArgs.ActorDamageTakenEventArgs e)
    {
       if(e._Actor == actor)
        {
            animator.SetTrigger("Hurt");
        }
    }

    private void Actor_OnActorDestroyed(object sender, CSEventArgs.ActorEventArgs e)
    {
        if(e._Actor == actor)
        {
            animator.SetTrigger("Death");
        }
    }

    protected override void EventUnsubscribe()
    {
        //OnVelocityUpdate
        AIMover.OnVelocityUpdate -= AIMover_OnVelocityUpdate;
        //OnAttack
        AILogic_Shambler.OnAbilityCast -= AILogic_Shambler_OnAbilityCast;
        //On Hit
        ActorStats.OnDamageTaken -= ActorStats_OnDamageTaken;
        //On Death
        Actor.OnActorDestroyed -= Actor_OnActorDestroyed;
    }
}
