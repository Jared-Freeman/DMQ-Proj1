using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class DEMO_ActorAISpawner : MonoBehaviour
{
    #region Members

    public SpawnerSettings Settings;
    public SpawnerStateInfo Info;

    public List<ActorAI> List_SpawnedActors = new List<ActorAI>();

    [System.Serializable]
    public struct SpawnerSettings
    {
        /// <summary>
        /// Maximum number of AI at once.
        /// </summary>
        public int MaxCount;

        /// <summary>
        /// The Actor to spawn
        /// </summary>
        public GameObject ActorPrefab;

        /// <summary>
        /// Period between spawns in seconds.
        /// </summary>
        public float SpawnInterval;
    }

    [System.Serializable]
    public struct SpawnerStateInfo
    {
        public float _LastSpawnedTimestamp;
    }

    #endregion

    #region Initialization

    void Awake()
    {
        if(Settings.ActorPrefab == null)
        {
            Debug.LogError("Actor prefab not set!");
        }
        if(Settings.ActorPrefab.GetComponent<ActorAI>() == null)
        {
            Debug.LogError("ActorPrefab Gameobject does not contain an ActorAI component!");
        }
    }

    void Start()
    {
        Actor.OnActorDestroyed += Actor_OnActorDestroyed;

        StartCoroutine(I_ContinueSpawn());
    }

    private void Actor_OnActorDestroyed(object sender, CSEventArgs.ActorEventArgs e)
    {
        var act = e._Actor as ActorAI;

        if(act != null && List_SpawnedActors.Contains(act))
        {
            List_SpawnedActors.Remove(act);
        }

    }
    #endregion

    protected IEnumerator I_ContinueSpawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(Settings.SpawnInterval);

            Spawn();
        }
    }

    protected bool Spawn()
    {
        if(List_SpawnedActors.Count < Settings.MaxCount)
        {
            var go = Instantiate(Settings.ActorPrefab);


            var agent = go.GetComponent<NavMeshAgent>();
            if (Utils.AI.RandomPointInCircle(transform.position, 1.5f, 12, out Vector3 result))
            {
                go.transform.position = result;
                agent.Warp(result);
            }
            else
            {
                go.transform.position = transform.position;
                agent.Warp(transform.position);
            }

            var ai = go.GetComponent<ActorAI>();
            if(ai != null)
                List_SpawnedActors.Add(ai);

            Info._LastSpawnedTimestamp = Time.time;
        }

        return true;
    }
}
