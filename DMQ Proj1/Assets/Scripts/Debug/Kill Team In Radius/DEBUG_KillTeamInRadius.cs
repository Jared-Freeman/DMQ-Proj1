using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_KillTeamInRadius : MonoBehaviour
{
    public float Radius = 300f;

    public Team DesiredTeam;

    // Start is called before the first frame update
    void Start()
    {
        if (DesiredTeam != null)
            StartCoroutine(WaitThenKillTeam());

        else Destroy(this);
    }

    public IEnumerator WaitThenKillTeam()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        foreach (var a in ActorManager.Instance.ActorList)
        {
            if( a._Team == DesiredTeam && (a.transform.position - transform.position).sqrMagnitude < Mathf.Pow(Radius,2))
            {
                Debug.LogError("radial dead act");

                Destroy(a.gameObject);
            }
        }

        Destroy(this);  
    }
}
