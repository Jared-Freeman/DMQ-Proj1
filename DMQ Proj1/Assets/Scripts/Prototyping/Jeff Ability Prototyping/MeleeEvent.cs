using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    public class MeleeEvent : MonoBehaviour
    {
        
        public EffectContext ctx;
        public float eventDuration = 0; //This value is set by the effect that instantiates this event.
        public List<Effect_Base> EffectList;
        private List<GameObject> targetsHit;
        void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.GetComponent<Actor>()) //Currently melees only register on actors. Might want to change this to hit objects.
            {
                Team friendlyTeam = ctx.AttackData._Team;
                Actor otherActor = col.gameObject.GetComponent<Actor>();

                if(otherActor._Team != friendlyTeam) //Not sure if I need to do this. Do effects have their own team filtering?
                {
                   // if(!targetsHit.Contains(col.gameObject)) //Make sure we don't hit the same object twice with one melee event
                    //{
                        ctx.AttackData._TargetGameObject = col.gameObject; //We found an enemy actor so set it as the target.
                        foreach (Effect_Base effect in EffectList)
                            effect.Invoke(ref ctx);
                   // }
                    //targetsHit.Add(col.gameObject);
                }
            }
        }
        void Update()
        {
            if(eventDuration != 0)
            {
                if(eventDuration > 0)
                    eventDuration -= Time.deltaTime;
                if(eventDuration < 0)
                    Destroy(gameObject);
            }
        }
    }
}

