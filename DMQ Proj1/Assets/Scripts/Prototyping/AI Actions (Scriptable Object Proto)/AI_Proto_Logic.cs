using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notes:
// Use tasks to allow for interrupting and queueing without issue
// Behavior invokes a method, the method invokes a scriptable object. SO's are extended via args
public class AI_Proto_Logic : MonoBehaviour
{
    public enum State { None, Patrol, AttackPoint }
    private State CurrentState = State.Patrol;

    [SerializeField] Vector3 patrolPoint = Vector3.zero;

    //For coupling Methods in the Logic with Actions specified by SO's. Decoupling actions and logic allows us to easily reuse code for both
    public Action_List Methods;

    [System.Serializable]
    public struct Action_List
    {
        public int test;
        public AI_Proto_Action MoveToPosition;
    }

    private void Start()
    {
        UpdateState(); //TODO: Change to a service/routine
    }

    void Update()
    {
    }

    void UpdateState()
    {
        switch (CurrentState)
        {
            case State.None:
                break;
            case State.Patrol:
                Patrol();
                break;
        };
    }

    void Patrol()
    {
        //Get new point

        //move to point
        StartCoroutine(MoveToPatrolPoint(2f, GetRandomPoint()));
    }

    public IEnumerator MoveToPatrolPoint(float speed, Vector3 Point)
    {
        while((transform.position - Point).sqrMagnitude > ((Point - transform.position).normalized * speed * Time.deltaTime).sqrMagnitude)
        {
            transform.position += (Point - transform.position).normalized * speed * Time.deltaTime;
            yield return null;
        }
        transform.position = Point;
        Debug.Log("Exit Routine");

        UpdateState();
    }

    Vector3 GetRandomPoint()
    {
        float PatrolRadius = 3f;

        GameObject Parent = gameObject;

        float dist = Random.Range(0, PatrolRadius);
        float yawAngle = Random.Range(0, 360);
        yawAngle *= Mathf.Deg2Rad;

        Vector3 Point = Parent.transform.position + new Vector3(Mathf.Cos(yawAngle), 0, Mathf.Sin(yawAngle)) * dist;


        patrolPoint = Point;

        return Point;
    }

}
