using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class Utils_SpawnPrefabAfterSeconds : MonoBehaviour
    {
        public float Seconds = 0f;

        public bool UseUnscaledTime = true;

        public GameObject Prefab;

        // Start is called before the first frame update
        void Start()
        {
            if (Prefab == null) Destroy(this);

            if(UseUnscaledTime)
            {
                StartCoroutine(I_ContinueDelayUnscaled(Seconds));
            }
            else
            {
                StartCoroutine(I_ContinueDelay(Seconds));
            }
        }


        protected IEnumerator I_ContinueDelayUnscaled(float duration)
        {
            float startTime = Time.unscaledTime;
            while (Mathf.Abs(Time.unscaledTime - startTime) < duration)
            {
                yield return null;
            }

            var g = Instantiate(Prefab);

            g.transform.position = transform.position;

            Destroy(this);
        }


        protected IEnumerator I_ContinueDelay(float duration)
        {
            float startTime = Time.time;
            while (Mathf.Abs(Time.time - startTime) < duration)
            {
                yield return null;
            }

            var g = Instantiate(Prefab);

            g.transform.position = transform.position;

            Destroy(this);
        }
    }


}