using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a dash trail PFX if the <see cref="PlayerMovementV3"/> component exceeds its movement speed max
/// </summary>
[RequireComponent(typeof(PlayerMovementV3))]
public class PlayerMovementV3_DashTrail : MonoBehaviour
{
    private static float _RoutineWaitForDestroySeconds = 2;

    public PlayerMovementV3_DashTrailPreset Preset;
    /// <summary>
    /// Transform that the the dash trail follows
    /// </summary>
    public Transform DashTrailPositionReference;

    /// <summary>
    /// Manages and destroys these as needed
    /// </summary>
    protected List<ParticleSystem> List_ParticleSystems { get; set; }  = new List<ParticleSystem>();
    protected ParticleSystem CurrentParticleTrail;
    protected PlayerMovementV3 AttachedMovementSystem;


    void Awake()
    {
        if(Preset == null)
        {
            Debug.LogError("No preset found. Destroying self");
            Destroy(this);
        }
        if(Preset.ParticleSystemPrefab == null)
        {
            Debug.LogError("Preset needs a valid, non-null prefab. Destroying self");
            Destroy(this);
        }
        if(DashTrailPositionReference == null)
        {
            Debug.LogError("DashTrailPositionReference is null. Destroying.");
            Destroy(this);
        }

        AttachedMovementSystem = GetComponent<PlayerMovementV3>();
        if(AttachedMovementSystem == null)
        {
            Debug.LogError("No PlayerMovementV3 found. Destroying self");
            Destroy(this);
        }
    }

    void Start()
    {
        StartCoroutine(CleanupSystems());
    }

    void OnEnable()
    {
        PlayerMovementV3.OnBeginExceedingCurrentMaxMovementSpeed += PlayerMovementV3_OnBeginExceedingCurrentMaxMovementSpeed;
        PlayerMovementV3.OnFinishExceedingCurrentMaxMovementSpeed += PlayerMovementV3_OnFinishExceedingCurrentMaxMovementSpeed;
    }
    void OnDisable()
    {
        PlayerMovementV3.OnBeginExceedingCurrentMaxMovementSpeed -= PlayerMovementV3_OnBeginExceedingCurrentMaxMovementSpeed;
        PlayerMovementV3.OnFinishExceedingCurrentMaxMovementSpeed -= PlayerMovementV3_OnFinishExceedingCurrentMaxMovementSpeed;
    }

    void Update()
    {
        //update position
        if(CurrentParticleTrail != null && DashTrailPositionReference != null)
        {
            CurrentParticleTrail.gameObject.transform.position = DashTrailPositionReference.transform.position;
        }
    }

    private void PlayerMovementV3_OnFinishExceedingCurrentMaxMovementSpeed(object sender, PlayerMovementV3EventArgs e)
    {
        if(e.Mover == AttachedMovementSystem)
        {
            CurrentParticleTrail.Stop();
        }
    }

    private void PlayerMovementV3_OnBeginExceedingCurrentMaxMovementSpeed(object sender, PlayerMovementV3EventArgs e)
    {
        if (e.Mover == AttachedMovementSystem)
        {
            var go = Instantiate(Preset.ParticleSystemPrefab);

            CurrentParticleTrail = go.GetComponent<ParticleSystem>();

            if (CurrentParticleTrail == null) Destroy(CurrentParticleTrail);

            else
            {
                List_ParticleSystems.Add(CurrentParticleTrail);

                go.transform.position = DashTrailPositionReference.transform.position;

                CurrentParticleTrail.Play();
            }
        }
    }

    private IEnumerator CleanupSystems()
    {
        List<ParticleSystem> destroyList = new List<ParticleSystem>();

        while(true)
        {
            //mark for removal
            foreach(var p in List_ParticleSystems)
            {
                if (p.isStopped)
                {
                    destroyList.Add(p);
                }
            }
            //remove objects
            foreach(var p in destroyList)
            {
                List_ParticleSystems.Remove(p);
                Destroy(p.gameObject);
            }
            destroyList.Clear();

            yield return new WaitForSeconds(_RoutineWaitForDestroySeconds);
        }
    }
}
