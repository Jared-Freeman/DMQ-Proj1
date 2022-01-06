using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //for ToList()

//Manages enabling/disabling dynamic lights with a sister class FR_DynamicLightingManager.cs
//Only tracks lights added prior to instantiation. Runtime light adding is currently NOT SUPPORTED

[RequireComponent(typeof(Light))]
public class FR_TrackedLight : MonoBehaviour
{
    #region Events
    public static event System.EventHandler<CSEventArgs.LightArgs> OnLightCreated;
    public static event System.EventHandler<CSEventArgs.LightArgs> OnLightDestroyed;
    #endregion

    public byte RenderPriority = 64; //Higher is more important
    private List<Light> TrackedLights;

    private void Awake()
    {
        TrackedLights = GetComponents<Light>().ToList();
        if (TrackedLights.Count < 1)
        {
            Debug.LogError("No Lights detected on GameObject! Destroying MonoBehaviour");
            Destroy(this);
        }
    }

    private void Start()
    {
        foreach(Light L in TrackedLights)
        {
            OnLightCreated?.Invoke(this, new CSEventArgs.LightArgs(L));
        }
    }

    private void OnDestroy()
    {
        foreach (Light L in TrackedLights)
        {
            OnLightDestroyed?.Invoke(this, new CSEventArgs.LightArgs(L));
        }
    }

    public IEnumerator DestroyAfterSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(gameObject);
    }
}
