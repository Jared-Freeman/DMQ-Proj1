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
        
        private void OnTriggerEnter(Collider other)
        {
            //If other actors are within the radius when the shield is planted
            ActorStats otherStats;
            if (otherStats = other.gameObject.GetComponent<ActorStats>())
            {
                Actor_DamageMessage msg = new Actor_DamageMessage();
                msg._DamageInfo = damageMessagePreset;
                if (ctx.AttackData._Owner != null) msg._Caster = ctx.AttackData._Owner.gameObject;
                if (ctx.AttackData._InitialGameObject != null) msg._DamageSource = ctx.AttackData._InitialGameObject;
                if (ctx.AttackData._Team != null) msg._Team = ctx.AttackData._Team;
                otherStats.ApplyDamage(msg);

                //Also need to apply knockback
                Actor otherActor = other.gameObject.GetComponent<Actor>();
                if(otherActor)
                {
                    Team otherTeam = otherActor._Team;
                    Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                    if (rb && otherTeam != ctx.AttackData._Team)
                    {
                        rb.AddForce(-rb.transform.forward * knockbackForce); //Backwards force?
                    }
                }
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

