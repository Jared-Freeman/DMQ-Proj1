using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem
{
    public class ActorAI_Logic_Melee : ActorAI_Logic
    {
        #region members
        public Actor TargetActor;
        public float CurrentAttackCooldown;
        public float AttackCooldown = 1.0f;
        public float AggroDistance = 20;
        public bool isAggro;
        ActorAction_Attack MeleeAttack;
        ActorAction_MoveToTarget MoveTo;
        #endregion

        #region methods
        // Start is called before the first frame update
        new protected void Start()
        {
            base.Start();
            Animator = GetComponent<Animator>();
            MeleeAttack = GetComponent<ActorAction_Attack>();
            MoveTo = GetComponent<ActorAction_MoveToTarget>();
            CurrentAttackCooldown = 0f;
            isAggro = false;
            //MoveTo.OnActionStart();
        }

        // Update is called once per frame
        void Update()
        {
            //Replace with event based interrupt later
            if ((TargetActor.transform.position - gameObject.transform.position).sqrMagnitude < (AggroDistance * AggroDistance) && !isAggro)
            {
                MoveTo.OnActionStart();
                isAggro = true;
            }
            float hVelocity = NavAgent.velocity.x;
            float vVelocity = NavAgent.velocity.y;
            var speed = Mathf.Max(Mathf.Abs(hVelocity), Mathf.Abs(vVelocity));
            Animator.SetFloat("speedv", speed);
            UpdateLogic();
        }

        new void UpdateLogic()
        {
            if (isAggro)
            {
                if (NavAgent.remainingDistance <= NavAgent.stoppingDistance)
                {
                    if (!NavAgent.hasPath || NavAgent.velocity.sqrMagnitude == 0f)
                    {
                        //Destination reached
                        if (CurrentAttackCooldown >= AttackCooldown)
                        {
                            //Attack
                            Debug.Log("Attacking");
                            Animator.SetTrigger("Attack1h1");
                            MeleeAttack.BeginAttack(true);
                            CurrentAttackCooldown = 0f;
                        }
                    }
                }
                MoveTo.ActionUpdate();
                if (CurrentAttackCooldown < AttackCooldown)
                    CurrentAttackCooldown += Time.deltaTime;
            }
        }
        #endregion

    }


}