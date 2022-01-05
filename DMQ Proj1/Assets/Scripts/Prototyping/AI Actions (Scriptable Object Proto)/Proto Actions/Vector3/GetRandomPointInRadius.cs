using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetRandomPointInRadius", menuName = "ScriptableObjects/AI_Proto_Action/GetRandomPointInRadius", order = 1)] //May want to remove this from menu
public class GetRandomPointInRadius : AI_Proto_Action
{
    [System.Serializable]
    public struct O
    {
        public float PatrolRadius;
    }

    public O Options;

    public override void Invoke(AI_Proto_Logic Logic)
    {
        base.Invoke(Logic);

        GameObject Parent = AttachedLogic.gameObject;

        float dist = Random.Range(0, Options.PatrolRadius);
        float yawAngle = Random.Range(0, 360);
        yawAngle *= Mathf.Deg2Rad;

        Vector3 Point = Parent.transform.position + new Vector3(Mathf.Cos(yawAngle), 0, Mathf.Sin(yawAngle)) * dist;
    }
}
