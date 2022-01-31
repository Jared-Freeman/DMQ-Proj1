using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem
{
    //This class holds the reigns of any AI. Its goal is to take the ActorAI data structure and drive it during game simulation.
    [RequireComponent(typeof(ActorAI))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class ActorAI_Logic : MonoBehaviour
    {
        #region members

        public enum TargetPriority { None, Proximity }

        //For Logic subroutines (minimize cost-per-frame)
        protected readonly static float _RoutineSleepDuration = .125f; //8 times / sec

        //flags
        public bool FLAG_Debug = false;

        public AILogicOptions BaseOptions = new AILogicOptions();


        public ActorAI AttachedActor { get; protected set; }
        public NavMeshAgent NavAgent { get; protected set; }
        public Animator Animator { get; protected set; }
        #endregion

        #region Helper data

        //inspector helper
        [System.Serializable]
        public class AILogicOptions
        {
            [Header("Aggro")]
            public float AggroRadius = 20;


            [Header("Moving")]
            public float MovementSpeed = 2f; //TODO: Currently not hooked up to navmesh agent
            public float StopSlideDistance = .5f;


            [Header("Turning")]
            [Tooltip("Max angle the agent can move toward without needing to stop and turn")]
            public float MaxFacingAngle = 250f;
            [Tooltip("Deg/sec")]
            public float TurningRate = 360f;
        }


        #endregion

        protected virtual void Awake()
        {
            AttachedActor = GetComponent<ActorAI>();
            NavAgent = GetComponent<NavMeshAgent>();
            if (NavAgent == null) Debug.LogError("Navmesh Agent not discovered!");
        }

        protected virtual void Start()
        {
        }

        public void UpdateLogic()
        {

        }
    }


}