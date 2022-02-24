using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    public class MeleeEvent : MonoBehaviour
    {
        
        public EffectContext ctx;
        public Effect_Dev_MeleeAttack preset;
        public float eventDuration = 0; //This value is set by the effect that instantiates this event.
        public List<Effect_Base> EffectList;
        private List<GameObject> targetsHit;
        public void Init(List<Effect_Base> list,float duration, EffectContext ctx )
        {
            eventDuration = duration;
            EffectList = list;
            this.ctx = ctx;
        }
        void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.GetComponent<Actor>()) //Currently melees only register on actors. Might want to change this to hit objects.
            {
                Team friendlyTeam = ctx.AttackData._Team;
                Actor otherActor = col.gameObject.GetComponent<Actor>();
                Debug.Log("FriendlyTeam: " + friendlyTeam);
                if(otherActor._Team != friendlyTeam)
                {
                    ctx.AttackData._TargetGameObject = col.gameObject; //We found an enemy actor so set it as the target.
                    foreach (Effect_Base effect in EffectList)
                        effect.Invoke(ref ctx);
                }
            }
            else //We hit a cube or something
            {
                ctx.AttackData._TargetGameObject = col.gameObject;
                foreach (Effect_Base effect in EffectList)
                    effect.Invoke(ref ctx);
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

