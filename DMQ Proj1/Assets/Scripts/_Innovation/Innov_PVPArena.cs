using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ClassSystem;
using UnityEngine.InputSystem;

public class Innov_PVPArena : MonoBehaviour
{
    private static int _CurrentClassSpawnIndex = 0;
    public bool ForceUseSpawnQueue = true;
    public List<GameObject> ClassSpawnQueue = new List<GameObject>();
    public List<Transform> SpawnPositions = new List<Transform>();

    public int sceneIndex = -1;
    public GameObject GameOverText;
    public TMPro.TextMeshPro goTextMesh; 

    public AbilitySystem.AS_Ability_Base ActorLoseAbility;
    private AbilitySystem.AS_Ability_Instance_Base _ActorLoseInst;

    /// <summary>
    /// Fires when the gameobjects that represent the players are created and initialized. New batches may be fired at any time
    /// </summary>
    public static event System.EventHandler<CSEventArgs.GameObjectListEventArgs> OnPlayerAgentsInstantiated;

    void OnEnable()
    {
        PlayerDataManager.OnPlayerActivated += PlayerDataManager_OnPlayerActivated;
    }
    void OnDisable()
    {
        PlayerDataManager.OnPlayerActivated -= PlayerDataManager_OnPlayerActivated;
    }

    private void PlayerDataManager_OnPlayerActivated(object sender, PlayerDataManager.PlayerDataSessionEventArgs e)
    {
        var g = InitGameObjectByDataSession(e.Data);

        List<GameObject> Glist = new List<GameObject>();
        Glist.Add(g);

        OnPlayerAgentsInstantiated?.Invoke(this, new CSEventArgs.GameObjectListEventArgs(Glist));
    }

    void Start()
    {
        List<GameObject> instances = new List<GameObject>();

        foreach (var r in Singleton<PlayerDataManager>.Instance.ActivatedPlayerSessions)
        {
            GameObject g = InitGameObjectByDataSession(r);

            if (g != null) instances.Add(g);
        }

        OnPlayerAgentsInstantiated?.Invoke(this, new CSEventArgs.GameObjectListEventArgs(instances));

        Actor.OnActorDead += Actor_OnActorDead;

        if(ActorLoseAbility != null)
        {
            _ActorLoseInst = ActorLoseAbility.GetInstance(gameObject);
        }    
    }
    void OnDestroy()
    {
        Actor.OnActorDead -= Actor_OnActorDead;
    }

    private void Actor_OnActorDead(object sender, CSEventArgs.ActorEventArgs e)
    {
        if(_ActorLoseInst != null && _ActorLoseInst.CanCastAbility)
        {
            EffectTree.EffectContext c = new EffectTree.EffectContext()
            {
                AttackData = new Utils.AttackContext()
                {
                    _InitialDirection = e._Actor.transform.forward,
                    _InitialGameObject = e._Actor.gameObject,
                    _InitialPosition = e._Actor.transform.position,

                    _TargetDirection = transform.forward,
                    _TargetGameObject = gameObject,
                    _TargetPosition = transform.position
                }
            };
            _ActorLoseInst.ExecuteAbility(ref c);
        }
        StartCoroutine(I_DelayCheckForWin(e));
    }

    protected IEnumerator I_DelayCheckForWin(CSEventArgs.ActorEventArgs e)
    {
        yield return new WaitForSeconds(2f);

        if(!CheckForTeam(e._Actor._Team))
        {
            if(Singleton<ActorManager>.Instance.ActorList.Count > 0)
            {
                goTextMesh.text = Singleton<ActorManager>.Instance.ActorList[0]._Team.TeamName + " wins!";
            }
            else
            {
                goTextMesh.text = Singleton<ActorManager>.Instance.ActorList[0]._Team.TeamName + " Draw!";
            }
            GameOverText.SetActive(true);
            yield return new WaitForSeconds(3f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

    }

    /// <summary>
    /// returns true if any of the specified team exist
    /// </summary>
    /// <returns></returns>
    bool CheckForTeam(Team team)
    {
        foreach (var a in Singleton<ActorManager>.Instance.ActorList)
        {
            if (a._Team == team) return true;
        }
        return false;   
    }

    GameObject InitGameObjectByDataSession(PlayerData_Session r)
    {
        GameObject g;

        if (ForceUseSpawnQueue || r.Info._CurrentClassPreset == null)
        {
            g = Instantiate(ClassSpawnQueue[_CurrentClassSpawnIndex]);

            if (g != null)
            {
                InitPlayerActor(g, r.Info._Input);
            }

            _CurrentClassSpawnIndex += 1;
            _CurrentClassSpawnIndex %= ClassSpawnQueue.Count;
        }
        else
        {
            g = r.Info._CurrentClassPreset?.InstantiatePlayerActor();

            if (g != null)
            {
                InitPlayerActor(g, r.Info._Input);
            }
        }

        return g;
    }
    void InitPlayerActor(GameObject g, PlayerInput p)
    {
        g.transform.position = SpawnPositions[_CurrentClassSpawnIndex].transform.position;

        var h = g.GetComponent<PlayerInputHost>();
        h.CurrentPlayerInput = p;
    }

}
