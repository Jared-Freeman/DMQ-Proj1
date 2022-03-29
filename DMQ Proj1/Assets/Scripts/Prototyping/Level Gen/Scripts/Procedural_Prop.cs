using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Procedural_Prop : MonoBehaviour
{
    public static bool FLAG_DrawGizmos = true;
    public float VolumeSize;

    [Range(-180f, 0f)]
    public float MinYaw = 0f;
    [Range (0f, 180f)]
    public float MaxYaw = 0f;

    public LGP_PROP_COLLECTION preset;

    /// <summary>
    /// Represents the Maximum distance from the origin that a procedural prop can be moved randomly
    /// </summary>
    [Min(0)]
    public float RandomDistance = 0f;

    void Awake()
    {
        RandomDistance = Mathf.Clamp(RandomDistance, 0, VolumeSize);
        Vector3 offsetVector = Vector3.zero;
        offsetVector.x += Random.Range(-RandomDistance, RandomDistance);
        offsetVector.z += Random.Range(-RandomDistance, RandomDistance);

        float randomRotation = Random.Range(MinYaw, MaxYaw);

        if (preset == null)
        {
            Debug.LogError("Cant fine me props");
            Destroy(this);
        }

        int index = Random.Range(0, preset.List_Props.Count);
        if (preset.List_Props[index] != null)
        {
            var go = Instantiate(preset.List_Props[index]);
            go.transform.position  = transform.position;
            go.transform.position += go.transform.forward * offsetVector.z;
            go.transform.position += go.transform.right * offsetVector.x;
            go.transform.rotation = transform.rotation;
            go.transform.Rotate(Vector3.up, randomRotation);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + new Vector3(0, VolumeSize / 2, 0), new Vector3(VolumeSize, VolumeSize, VolumeSize));

        //Draw RandomDistance extents icons
        GUIStyle G = new GUIStyle();
        G.fontSize = 15;

        G.normal.textColor = Color.green;
        string GUI_str = "X";
        UnityEditor.Handles.Label(transform.position + transform.forward * RandomDistance, GUI_str, G);
        UnityEditor.Handles.Label(transform.position + -transform.forward * RandomDistance, GUI_str, G);
        UnityEditor.Handles.Label(transform.position + transform.right * RandomDistance, GUI_str, G);
        UnityEditor.Handles.Label(transform.position + -transform.right * RandomDistance, GUI_str, G);
    }
#endif
}
