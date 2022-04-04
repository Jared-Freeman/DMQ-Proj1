﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem.AI;
public class AnimatorProxy_AI : AnimatorProxy
{
    protected ActorAI_Logic AttachedAILogic;

    protected override void Awake()
    {
        base.Awake();

        AttachedAILogic = GetComponent<ActorAI_Logic>();
        if (!Utils.Testing.ReferenceIsValid(AttachedAILogic)) Destroy(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void EventSubscribe()
    {
        base.EventSubscribe();

        //OnVelocityUpdate
        AIMover.OnVelocityUpdate += AIMover_OnVelocityUpdate;
        //OnAttack
        //AILogic_Shambler.OnAbilityCast += AILogic_Shambler_OnAbilityCast;
        AttachedAILogic.OnAttackChargeBegin += AttachedAILogic_OnAttackChargeBegin;
        AttachedAILogic.OnAttackChargeCancel += AttachedAILogic_OnAttackChargeCancel;
        //On Hit
        ActorStats.OnDamageTaken += ActorStats_OnDamageTaken;
        //On Death
        Actor.OnActorDestroyed += Actor_OnActorDestroyed;
    }

    private void AttachedAILogic_OnAttackChargeCancel(object sender, ActorSystem.AI.EventArgs.ActorAI_Logic_EventArgs e)
    {
        if (e._Actor == actor)
        {
            // is this needed here?
            animator.SetInteger("AbilityNum", e._AbilityIndex);

            animator.ResetTrigger("Ability");
        }
    }

    private void AttachedAILogic_OnAttackChargeBegin(object sender, ActorSystem.AI.EventArgs.ActorAI_Logic_EventArgs e)
    {
        if (e._Actor == actor)
        {
            animator.SetTrigger("Ability");
            animator.SetInteger("AbilityNum", e._AbilityIndex);
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
        base.EventUnsubscribe();

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
