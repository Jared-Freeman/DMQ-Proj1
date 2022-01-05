using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; //!!
using UnityEngine;

//Notes:
// Use tasks to allow for interrupting and queueing without issue
// Behavior invokes a method, the method invokes a scriptable object. SO's are extended via args
public class AI_Proto_Logic : MonoBehaviour
{
    public enum State { None, Patrol, AttackPoint }
    private State CurrentState = State.None;

    //For coupling Methods in the Logic with Actions specified by SO's. Decoupling actions and logic allows us to easily reuse code for both
    public Action_List Methods;

    [System.Serializable]
    public struct Action_List
    {
        public AI_Proto_Action MoveToPosition;
        public AI_Proto_Action Method1Action;
        public AI_Proto_Action Method2Action;
    }

    void Update()
    {
        UpdateState(); //TODO: Change to a service routine
    }

    void UpdateState()
    {
        switch (CurrentState)
        {
            case State.None:
                break;
            case State.Patrol:
                break;
        };
    }

}
