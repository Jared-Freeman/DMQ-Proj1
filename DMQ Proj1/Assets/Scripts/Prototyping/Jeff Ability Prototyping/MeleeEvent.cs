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

        /// <summary>
        /// Dispatch effect tree to targets this trigger has hit.
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.GetComponent<Actor>()) //Currently melees only register on actors. Might want to change this to hit objects.
            {
                Actor otherActor = col.gameObject.GetComponent<Actor>();

                ctx.AttackData._TargetGameObject = col.gameObject; //We found an enemy actor so set it as the target.

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

