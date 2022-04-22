using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Destroy this object when the reference GameObject is destroyed
    /// </summary>
    public class Utils_DestroyOnReferenceDestroyed : MonoBehaviour
    {
        private static float s_RoutineInterval = .125f;

        public GameObject ReferenceObject;

        // Start is called before the first frame update
        void Awake()
        {
            if (!Utils.Testing.ReferenceIsValid(ReferenceObject)) Destroy(this);
        }

        void Start()
        {
            StartCoroutine(I_CheckForDeletion());
        }

        public IEnumerator I_CheckForDeletion()
        {
            while (true)
            {
                if (ReferenceObject == null) Destroy(gameObject);
                yield return new WaitForSeconds(s_RoutineInterval);
            }
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}