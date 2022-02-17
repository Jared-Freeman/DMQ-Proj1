using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class DEBUG_NavmeshRebaker : MonoBehaviour
{
    NavMeshSurface s;

    void Awake()
    {
        s = GetComponent<NavMeshSurface>();

        if (s == null) Destroy(this);
    }

    void Start()
    {
        StartCoroutine(Bake());
    }

    public IEnumerator Bake()
    {
        yield return new WaitForEndOfFrame();

        s.BuildNavMesh();
    }
}
