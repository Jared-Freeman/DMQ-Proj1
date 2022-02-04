using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.StatusEffect.UI
{
    /// <summary>
    /// Component to help determine screen space position to render a status effect icon
    /// </summary>
    public class SE_UI_Proxy : MonoBehaviour
    {
        public Transform RenderTransform;

        // Start is called before the first frame update
        void Start()
        {
            if (RenderTransform == null) RenderTransform = gameObject.transform;
        }

        /// <summary>
        /// Get screen space position relative to <paramref name="cam"/>
        /// </summary>
        /// <returns>Vector3 representing x/y screen-space coordinates of the tracked RenderPosition. Z is distance from camera in world units</returns>
        public Vector3 GetCameraSpacePosition(Camera cam)
        {
            var cPos3 = cam.WorldToScreenPoint(RenderTransform.position);

            return cPos3;
        }
    }
}