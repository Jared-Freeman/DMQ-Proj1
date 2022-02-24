﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    public class MeleeAttackEventArgs : System.EventArgs
    {
        public MeleeAttackEventArgs(Actor a)
        {
            actor = a;
        }
        public Actor actor;
    }
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Dev Melee Attack", order = 1)]
    public class Effect_Dev_MeleeAttack : Effect_Base
    {
        public static event System.EventHandler<MeleeAttackEventArgs> OnMeleeAttack;
        public GameObject meleeAttackEventObject;
        public List<Effect_Base> EffectList;
        public float distance = 2; //Arbitrarily chosen, seems ok.
        public float duration = 0.5f; //Arbitrarily chosen number. Probably a better way to do it.
        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                Actor attackingActor = ctx.AttackData._Owner.gameObject.GetComponent<Actor>();
                if(attackingActor)
                {
                    OnMeleeAttack.Invoke(this, new MeleeAttackEventArgs(attackingActor));
                }
                //Instantiate a melee event object
                GameObject g = ctx.AttackData._Owner.gameObject;
                Vector3 position = g.transform.position;
                Vector3 direction = g.transform.forward;
                Vector3 spawnPos = position + direction * distance;
                GameObject w = Instantiate(meleeAttackEventObject,spawnPos,Quaternion.identity);
                MeleeEvent m = w.GetComponent<MeleeEvent>();
                m.Init(EffectList, duration,ctx);
                return true;
            }
            return false;
        }
    }
}

