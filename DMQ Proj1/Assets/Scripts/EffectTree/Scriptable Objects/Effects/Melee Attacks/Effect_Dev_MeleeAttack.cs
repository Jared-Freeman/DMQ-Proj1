using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

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

    /// <summary>
    /// Spawns a trigger for a period of time, and applies effects to targets within that volume.
    /// </summary>
    [CreateAssetMenu(fileName = "DMA_", menuName = "Effect Tree/Dev Melee Attack", order = 1)]
    public class Effect_Dev_MeleeAttack : Effect_Base
    {
        public static event System.EventHandler<MeleeAttackEventArgs> OnMeleeAttack;

        public bool AttachAttackToCaster = true;
        /// <summary>
        /// Should the hitbox be rotated so that its forward (blue arrow) aligns to the effect context direction?
        /// </summary>
        public bool AdjustRotationToFaceContextDirection = true;

        /// <summary>
        /// Distance from spawn position that the prefab is spawned.
        /// </summary>
        public float Distance = 2f;


        public EffectContext.FacingOptions Direction = EffectContext.FacingOptions._TargetDirection;

        public TargetFilterOptions TargetFilters;

        /// <summary>
        /// Reference GameObject prefab containing a MeleeEvent class, and a Trigger collider.
        /// </summary>
        public GameObject meleeAttackEventObject;
        public List<Effect_Base> EffectList;

        public float EffectDuration = 0.75f; //Arbitrarily chosen number. Probably a better way to do it.

        /// <summary>
        /// Create a Trigger that represents this melee attack, and pass in the effect list to be used in that instantiated GameObject
        /// </summary>
        /// <param name="ctx">Effect Context</param>
        /// <returns></returns>
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

                Vector3 direction = ctx.RetrieveDirectionVector(Direction);
                if(direction.sqrMagnitude == 0) direction = ctx.GetAnyDirectionVector();


                Vector3 spawnPos = position + direction * Distance;


                GameObject w = Instantiate(meleeAttackEventObject,spawnPos,Quaternion.identity);

                if (w == null) return false;

                if (AdjustRotationToFaceContextDirection) w.transform.forward = direction;

                if (AttachAttackToCaster) w.transform.parent = ctx.AttackData._Owner.gameObject.transform;

                MeleeEvent m = w.GetComponent<MeleeEvent>();

                m.Preset = this;
                m.ctx = ctx;

                return true;
            }
            return false;
        }
    }
}

