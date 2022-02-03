using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// NYI
    /// </summary>
    public class ET_Visualizer : MonoBehaviour
    {
        protected static int s_BranchDepthMax = 15; //hmmm i dunno how to impl this

        /// <summary>
        /// May have to write a rule for each type of effect node to get their refs correctly...
        /// </summary>
        Tree EffectTree;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var VolumeSize = 3; //hmmm

            Gizmos.DrawWireCube(transform.position, new Vector3(VolumeSize, VolumeSize, VolumeSize));
            //Gizmos.DrawIcon(transform.position + Vector3.forward * VolumeSize, "test");

            //Draw Cardinal icons
            GUIStyle G = new GUIStyle();
            G.fontSize = 15;
            float Denominator = 2.25f;

            int i = 0;
            string GUI_str;


            GUI_str = "X";
            UnityEditor.Handles.Label(transform.position + transform.forward * VolumeSize / Denominator, GUI_str, G);
            Gizmos.DrawRay(new Ray(transform.position, transform.forward * VolumeSize / Denominator));

            i++;
            GUI_str = "X";
            UnityEditor.Handles.Label(transform.position + transform.right * VolumeSize / Denominator, GUI_str, G);
            Gizmos.DrawRay(new Ray(transform.position, transform.right * VolumeSize / Denominator));

            i++;
            GUI_str = "X";
            UnityEditor.Handles.Label(transform.position + -transform.forward * VolumeSize / Denominator, GUI_str, G);
            Gizmos.DrawRay(new Ray(transform.position, -transform.forward * VolumeSize / Denominator));

            i++;
            GUI_str = "X";
            UnityEditor.Handles.Label(transform.position + -transform.right * VolumeSize / Denominator, GUI_str, G);
            Gizmos.DrawRay(new Ray(transform.position, -transform.right * VolumeSize / Denominator));
        }
#endif


    }
}