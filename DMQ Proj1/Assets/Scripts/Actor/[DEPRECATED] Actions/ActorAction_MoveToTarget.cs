using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem
{
    public class ActorAction_MoveToTarget : MonoBehaviour
    {
        //All this does for now is feed the NavMesh Agent the Target's position.
        #region Members
        ActorAI_Logic_Melee Logic;
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            Logic = GetComponent<ActorAI_Logic_Melee>();
        }

        #region Methods
        public void OnActionStart()
        {
            //Logic.NavAgent.SetDestination(Logic.TargetActor.transform.position);
        }
        public void ActionUpdate()
        {
            Logic.NavAgent.SetDestination(Logic.TargetActor.transform.position);
        }
        public void OnActionEnd()
        {

        }
        #endregion
    }


}