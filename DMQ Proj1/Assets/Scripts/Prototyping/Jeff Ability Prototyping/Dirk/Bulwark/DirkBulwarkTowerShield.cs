using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EffectTree
{
    public class DirkBulwarkTowerShield : MonoBehaviour
    {
        public int shieldHealth = 10;
        public float damageAmount = 50f;
        public int damagePerProjectile = 1;
        public float reflectProjectileProbability = 0.25f; //25% chance of reflecting projectiles that hit the shield.
        public float knockbackForce = 10000;
        public AP2_DamageMessagePreset damageMessagePreset;
        public EffectContext ctx;
        float colliderExpireTimer = 0;
        bool triggerUp = true;
        SphereCollider shieldCollider;
        public List<Effect_Base> EffectList;
        // Start is called before the first frame update
        void Start()
        {
            shieldCollider = GetComponent<SphereCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (triggerUp)
            {
                if (colliderExpireTimer < 1.0f)
                    colliderExpireTimer += Time.deltaTime;
                else
                {
                    if (shieldCollider.enabled)
                        shieldCollider.enabled = false;
                    triggerUp = false;
                }
            }
        }
        
        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.GetComponent<Actor>()) //Currently melees only register on actors. Might want to change this to hit objects.
            {
                Team friendlyTeam = ctx.AttackData._Team;
                Actor otherActor = col.gameObject.GetComponent<Actor>();
                Debug.Log("FriendlyTeam: " + friendlyTeam);
                if (otherActor._Team != friendlyTeam)
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
        private void OnCollisionEnter(Collision col)
        {
            Rigidbody rb = col.rigidbody;
            if (rb)
            {
                /*Detect if the object is an enemy projectile. Don't know how to do this a better way.
                 * Doing it this way will allow projectiles from teams other than 1(players) to trigger the effect.
                 This is assuming that the AI will tag their projectiles to their team's layer
                Check for GenericProjectile component?
                 */

                if (rb.gameObject.layer == 8 || rb.gameObject.layer == 11 || rb.gameObject.layer == 13 || rb.gameObject.layer == 15)
                {
                    float randNum = Random.Range(0, 1.0f);
                    if (randNum > reflectProjectileProbability)
                    {
                        Destroy(col.gameObject);
                        shieldHealth -= 1;
                        if (shieldHealth <= 0)
                            Destroy(this.gameObject);
                    }
                }
            }
        }
    }
 }

