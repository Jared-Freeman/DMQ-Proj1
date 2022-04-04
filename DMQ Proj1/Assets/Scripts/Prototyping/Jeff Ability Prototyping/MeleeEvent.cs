using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    public class MeleeEvent : MonoBehaviour
    {
        public Effect_Dev_MeleeAttack Preset;

        public EffectContext ctx { get; set; }
        private List<GameObject> List_TargetsHit = new List<GameObject>();

        void Start()
        {
            StartCoroutine(DestroyAfterSeconds(Preset.EffectDuration));
        }

        /// <summary>
        /// Dispatch effect tree to targets this trigger has hit.
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerEnter(Collider col)
        {
            //Currently melees only register on actors. Might want to change this to hit objects.
            var a = col.gameObject.GetComponent<Actor>();
            if (
                a != null 
                && !List_TargetsHit.Contains(col.gameObject) 
                && Preset.TargetFilters.TargetIsAllowed(ctx.AttackData._Team, a)
                ) 
            {
                List_TargetsHit.Add(col.gameObject);

                EffectContext newContext = new EffectContext(ctx);

                newContext.AttackData._TargetGameObject = col.gameObject; //We found an enemy actor so set it as the target.

                foreach (Effect_Base effect in Preset.EffectList)
                    effect.Invoke(ref newContext);
            }
            else if(a == null && !List_TargetsHit.Contains(col.gameObject))
            {
                List_TargetsHit.Add(col.gameObject);

                EffectContext newContext = new EffectContext(ctx);
                newContext.AttackData._TargetGameObject = col.gameObject; //We found an enemy actor so set it as the target.

                foreach (Effect_Base effect in Preset.EffectList)
                    effect.Invoke(ref newContext);
            }
        }

        protected IEnumerator DestroyAfterSeconds(float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}

