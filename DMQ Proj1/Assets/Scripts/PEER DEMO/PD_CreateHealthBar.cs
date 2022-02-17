using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PD_CreateHealthBar : MonoBehaviour
{
    public GameObject HealthbarPrefab;

    void Start()
    {
        if(HealthbarPrefab != null)
        {
            var go = Instantiate(HealthbarPrefab);

            Mover_MatchTransformPosition mover = go.GetComponentInChildren<Mover_MatchTransformPosition>();

            mover.ReferenceTransform = transform;

            var relay = go.GetComponentInChildren<PD_ActorStatsHealthBarRelay>();
            var stats = GetComponent<ActorStats>();

            relay._Stats = stats;
        }
    }

}
