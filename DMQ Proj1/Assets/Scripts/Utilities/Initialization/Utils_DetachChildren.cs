using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Detaches all children of this component. Also contains static method to do the same anywhere in the program.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     - This component executes its code BEFORE standard execution time. 
    ///     Thus, you can infer that heritage won't be mutated in some race condition in other default-time scripts.
    /// </para>
    /// <para>
    ///     - Does NOT detach the host GameObject from any parentage.
    /// </para>
    /// <para>
    ///     - This Component is destroyed right after initialization!
    /// </para>
    /// </remarks>
    public class Utils_DetachChildren : MonoBehaviour
    {
        #region Members

        [Header("Detach all children recursively?")]
        public bool FlattenHierarchyCompletely = false;

        #endregion

        /// <summary>
        /// Attempts to detach all children from the <paramref name="targetTransform"/>.
        /// </summary>
        /// <remarks>
        /// The flag <paramref name="recursivelyDetachAllChildren"/> will cause the entire hierarchy to be flattened.
        /// </remarks>
        /// <param name="targetTransform">target to detach children from</param>
        /// <param name="recursivelyDetachAllChildren">Detach children for all objects in this hierarchy? (aka flatten hierarchy)</param>
        /// <returns></returns>
        public static bool DetachChildren(Transform targetTransform, bool recursivelyDetachAllChildren = false)
        {
            List<Transform> dTransforms = new List<Transform>();

            //get list of children. The heap fears me for my sins
            foreach (Transform t in targetTransform)
            {
                dTransforms.Add(t);
            }

            //behavior based on the recursive arg
            if (recursivelyDetachAllChildren)
            {
                targetTransform.parent = null;

                foreach (Transform t in dTransforms)
                {
                    DetachChildren(t, true);
                }
            }
            else
            {
                foreach (Transform t in dTransforms)
                {
                    t.parent = null;
                }
            }

            //why did i do this...
            return true;
        }

        void Awake()
        {
            DetachChildren(transform, FlattenHierarchyCompletely);
            Destroy(this);
        }


    }
}