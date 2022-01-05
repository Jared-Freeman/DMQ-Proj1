using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

//For dynamically enabling/disabling lights based on distance.
//Not applicable for extremely distant lights, or lights that must always be on
public class FR_DynamicLightingManager : MonoBehaviour
{
    #region Members

    //flags
    public bool FLAG_Debug = false;

    //settings
    public readonly int MaxLights;
    public Opt Options;

    public enum LightMGRUpdateSettings { DistanceTraveled, Polling }
    [System.Serializable]
    public struct Opt
    {
        public LightMGRUpdateSettings UpdateStyle;
        [Min(0f)]
        public float Radius;
        [Min(1)]
        public int MaxLightsEnabled;
        public Opt_Dist DistanceSettings;
        public Opt_Poll PollingSettings;
    }
    #region Helper structs
    [System.Serializable]
    public struct Opt_Dist
    {
        [Min(.1f)]
        public float Distance; //in unity units (m)
        [Min(.1f)]
        public float DistancePollingInterval;
    }
    [System.Serializable]
    public struct Opt_Poll
    {
        [Min(.125f)]
        public float PollingInterval; //in seconds
    }
    #endregion

    //refs
    public static List<Light> LightsInScene;
    private List<Light> CurrentEnabledLights;

    public Transform TrackerPosition; //TODO: Consider safest ways to hold this info (ie what if object is destroyed???)

    #endregion

    #region Event Subscriptions

    private void OnEnable()
    {
        FR_TrackedLight.OnLightCreated += AddTrackedLight;
        FR_TrackedLight.OnLightDestroyed += RemoveTrackedLight;
    }
    private void OnDisable()
    {
        FR_TrackedLight.OnLightCreated -= AddTrackedLight;
        FR_TrackedLight.OnLightDestroyed -= RemoveTrackedLight;

        //Probably a horrible idea to disable this, please don't!
    }

    #endregion

    #region Event Handlers

    void AddTrackedLight(object o, CSEventArgs.LightArgs a)
    {
        LightsInScene.Add(a.light);
        a.light.enabled = false; //turn off initially

        if (FLAG_Debug) Debug.Log("Light Added: " + a.light.name);
    }
    void RemoveTrackedLight(object o, CSEventArgs.LightArgs a)
    {
        LightsInScene.Remove(a.light);

        if (FLAG_Debug) Debug.Log("Light Removed: " + a.light.name);
    }

    #endregion

    #region Initialization

    private void Awake()
    {
        if (LightsInScene != null)
        {
            LightsInScene.Clear();
        }
        else
        {
            LightsInScene = new List<Light>();
        }
        CurrentEnabledLights = new List<Light>();

        if (TrackerPosition == null)
        {
            Debug.LogError("No Tracked Object specified! Destroying Manager");
            Destroy(this);
        }
    }

    private void Start()
    {
        switch(Options.UpdateStyle)
        {
            case LightMGRUpdateSettings.DistanceTraveled:
                StartCoroutine(Routine_DistanceTraveled());
                break;

            case LightMGRUpdateSettings.Polling:
                StartCoroutine(Routine_Polling());
                break;

            default:
                StartCoroutine(Routine_Polling());
                break;
        }
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if(FLAG_Debug) Gizmos.DrawWireSphere(TrackerPosition.position, Options.Radius);
        #endif
    }

#endregion

    private void OnDestroy()
    {
        StopAllCoroutines();
        LightsInScene.Clear();
    }

    public IEnumerator Routine_Polling()
    {
        while(true)
        {
            UpdateEnabledLights();

            yield return new WaitForSeconds(Options.PollingSettings.PollingInterval);
        }
    }
    

    public IEnumerator Routine_DistanceTraveled()
    {
        UpdateEnabledLights();

        Vector3 LastUpdatedPosition = TrackerPosition.position;
        Vector3 CurrentPosition;
        while (true)
        {
            CurrentPosition = TrackerPosition.position;


            if ((LastUpdatedPosition - CurrentPosition).sqrMagnitude > Options.DistanceSettings.Distance * Options.DistanceSettings.Distance)
            {
                LastUpdatedPosition = CurrentPosition;
                UpdateEnabledLights();
            }

            yield return new WaitForSeconds(Options.DistanceSettings.DistancePollingInterval);
        }
    }

    //TODO: Async with time budget
    //relatively expensive... Maybe can be optimized somehow through better traversal or sort method
    void UpdateEnabledLights()
    {

        //Disable Last-enabled Lights (check to see if they still exist)
        foreach(Light L in CurrentEnabledLights)
        {
            if(L != null)
                L.enabled = false;
        }
        CurrentEnabledLights.Clear();

        var UnprioLights = new List<Light>();

        //Order Lights in scene by nearest to TrackerPosition position
        var LightsByNearest = LightsInScene
          .OrderBy(t => (TrackerPosition.position - t.transform.position).sqrMagnitude);

        //Remove all lights outside of Radius
        foreach (Light L in LightsByNearest)
        {
            //if (FLAG_Debug) Debug.Log((TrackerPosition.position - L.transform.position).sqrMagnitude);
            if ((TrackerPosition.position - L.transform.position).sqrMagnitude < Options.Radius * Options.Radius)
            {
                UnprioLights.Add(L);
            }
            else
            {
                //break;
            }
        }

        //Order by render priority
        var PrioLights = UnprioLights.OrderBy(t => (t.gameObject.GetComponent<FR_TrackedLight>().RenderPriority));

        //Render all lights within our budget. Add to list
        int RemainingLights = Options.MaxLightsEnabled;
        foreach (Light L in PrioLights)
        {
            if (FLAG_Debug)
            {
                Debug.Log(L.gameObject.GetComponent<FR_TrackedLight>().RenderPriority);
            }

            if(RemainingLights > 0)
            {
                L.enabled = true;

                CurrentEnabledLights.Add(L);

                RemainingLights--;
            }
            else
            {
                break;
            }
        }
    }
}
