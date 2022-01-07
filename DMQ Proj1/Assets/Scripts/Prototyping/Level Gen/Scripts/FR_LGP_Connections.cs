using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FR_LGP_Connections : MonoBehaviour
{
    public static bool FLAG_DrawGizmos = false;

    public float VolumeSize = 1f;
    public bool [] Connections = new bool[4]; //NESW cardinal dir

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(FLAG_DrawGizmos)
        {
            //Gizmos.DrawWireCube(transform.position, new Vector3(VolumeSize, VolumeSize, VolumeSize));
            //Gizmos.DrawIcon(transform.position + Vector3.forward * VolumeSize, "test");

            //Draw Cardinal icons
            GUIStyle G = new GUIStyle();
            G.fontSize = 15;
            float Denominator = 2.25f;

            int i = 0;

            G.normal.textColor = Connections[i] ? Color.green : Color.red;
            string GUI_str = Connections[i] ? "N" : "X";
            UnityEditor.Handles.Label(transform.position + transform.forward * VolumeSize / Denominator, GUI_str, G);
            if (Connections[i]) Gizmos.DrawRay(new Ray(transform.position, transform.forward * VolumeSize / Denominator));

            i++;
            G.normal.textColor = Connections[i] ? Color.green : Color.red;
            GUI_str = Connections[i] ? "E" : "X";
            UnityEditor.Handles.Label(transform.position + transform.right * VolumeSize / Denominator, GUI_str, G);
            if (Connections[i]) Gizmos.DrawRay(new Ray(transform.position, transform.right * VolumeSize / Denominator));

            i++;
            G.normal.textColor = Connections[i] ? Color.green : Color.red;
            GUI_str = Connections[i] ? "S" : "X";
            UnityEditor.Handles.Label(transform.position + -transform.forward * VolumeSize / Denominator, GUI_str, G);
            if (Connections[i]) Gizmos.DrawRay(new Ray(transform.position, -transform.forward * VolumeSize / Denominator));

            i++;
            G.normal.textColor = Connections[3] ? Color.green : Color.red;
            GUI_str = Connections[i] ? "W" : "X";
            UnityEditor.Handles.Label(transform.position + -transform.right * VolumeSize / Denominator, GUI_str, G);
            if (Connections[i]) Gizmos.DrawRay(new Ray(transform.position, -transform.right * VolumeSize / Denominator));
        }


    }
#endif

}
