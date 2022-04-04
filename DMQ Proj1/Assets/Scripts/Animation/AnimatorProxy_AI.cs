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

    protected override void EventSubscribe()
    {
        //OnVelocityUpdate
        AIMover.OnVelocityUpdate += AIMover_OnVelocityUpdate;
        //OnAttack
        //AILogic_Shambler.OnAbilityCast += AILogic_Shambler_OnAbilityCast;
        AILogic_Shambler.OnAttackChargeBegin += AILogic_Shambler_OnAttackChargeBegin; //new handler since swing animation works well for "charging" style attack delay -Jared
        AILogic_Shambler.OnAttackChargeCancel += AILogic_Shambler_OnAttackChargeCancel;
        //On Hit
        ActorStats.OnDamageTaken += ActorStats_OnDamageTaken;
        //On Death
        Actor.OnActorDestroyed += Actor_OnActorDestroyed;
    }

    private void AILogic_Shambler_OnAttackChargeCancel(object sender, AILogic_ShamblerEventArgs e)
    {
        if (e.actor == actor)
        {
            animator.ResetTrigger("Ability");
            // is this needed here?
            //animator.SetInteger("AbilityNum", e.abilityIndex); 
        }
    }

    private void AILogic_Shambler_OnAttackChargeBegin(object sender, AILogic_ShamblerEventArgs e)
    {
        if (e.actor == actor)
        {
            animator.SetTrigger("Ability");
            animator.SetInteger("AbilityNum", e.abilityIndex);
        }
    }

    private void AIMover_OnVelocityUpdate(object sender, AIMoverEventArgs e)
    {
        if(e.actor == actor)
        {
            //float velocity = e.velocity;

            //this is a convoluted method of avoiding the expensive magnitude property accessor (it uses sqrt)
            float percentageOfMaxSpeed = e.velocity.magnitude / actor.Stats.MoveSpeedCurrent;

            animator.SetFloat("Speed", percentageOfMaxSpeed);

            //if (velocity > 0.1f)
            //{
            //    animator.SetFloat("Speed", velocity);
            //}
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
