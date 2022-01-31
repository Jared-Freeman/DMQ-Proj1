using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class ActorEventArgs : System.EventArgs
    {
        public ActorSystem.Actor _Actor;
        
        public ActorEventArgs(ActorSystem.Actor a)
        {
            _Actor = a;
        }
    }
}

namespace ActorSystem
{
    [RequireComponent(typeof(ActorStats))]
    public class Actor : MonoBehaviour
    {
        #region EVENTS

        public static event System.EventHandler<CSEventArgs.ActorEventArgs> OnActorCreated;
        public static event System.EventHandler<CSEventArgs.ActorEventArgs> OnActorDestroyed;

        #endregion

        #region members

        private static int IdGenerator = 0;

        readonly public int ActorID = IdGenerator++;

        public bool Flag_ActorDebug = false;

        public Team _Team;
        public ActorStats Stats;
        public List<ActorAction> Actions;
        public List<ActorStatusEffect> StatusEffects;

        #endregion

        protected void Start()
        {
            if (Flag_ActorDebug) Debug.Log("Base Actor Start()");
            if (Flag_ActorDebug) Debug.Log("Base Actor ID: " + ActorID);

            Stats = GetComponent<ActorStats>();
            if (Stats == null) Debug.LogError("No ActorStats found!");

            OnActorCreated?.Invoke(this, new CSEventArgs.ActorEventArgs(this));
        }


        private void OnDestroy()
        {
            OnActorDestroyed?.Invoke(this, new CSEventArgs.ActorEventArgs(this));
        }

        protected void Update()
        {
            if (Stats.m_timeSinceLastHit > 0)
                Stats.m_timeSinceLastHit -= Time.deltaTime;
            else
            {
                Stats.isInvulnerable = false;
            }
        }
        public void ActorDead()
        {
            // Actor has run out of HP. Probably want to have an action for handling this later.
            if (Flag_ActorDebug) Debug.Log(gameObject.name + " is dead");
            gameObject.SetActive(false);
        }
        //Wasn't sure where else to put this but I figure every actor will need this function. 
        public void TakeDamage(ActorDamage DamageTaken)
        {
            //Take damage
            Stats.HpCurrent -= DamageTaken.DamageAmount;
            //Check for status effects depending on type of damage

            //Check if HP has run out
            if (Stats.HpCurrent <= 0f)
            {
                ActorDead();
            }

        }
    }


}
