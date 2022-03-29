using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImpactFX
{
    //[CreateAssetMenu(fileName = "IFX", menuName = "ScriptableObjects/Impact FX/Light", order = 1)]
    public class IFX_Light : ImpactEffect
    {
        [System.Serializable]
        public struct ExtraOptions
        {
            public AnimationCurve _IntensityCurve;
            public AnimationCurve _RangeCurve;

            public GameObject _PrefabWithLightComponent;
        }

        public ExtraOptions LightOptions;

        public override void SpawnImpactEffect(GameObject OptionalParent, Vector3 Position, Vector3 DirectionForward = default, Vector3 DirectionNormal = default)
        {
            base.SpawnImpactEffect(OptionalParent, Position, DirectionForward, DirectionNormal);

            if (FLAG_Debug) Debug.Log("IFX Light Spawn(...)");

            if(LightOptions._IntensityCurve == null || LightOptions._RangeCurve == null || LightOptions._PrefabWithLightComponent == null)
            {
                Debug.LogError("IFX_Light called but Instance is not fully defined! Check the Light Options in the inspector!");
            }

            else
            {
                GameObject GO = Instantiate(LightOptions._PrefabWithLightComponent);
                var h = GO.AddComponent<IFX_Light_Helper>();
                h.IFX = this;

                if(OptionalParent != null)
                {
                    GO.transform.parent = OptionalParent.transform;
                    GO.transform.localPosition = Position;
                }
                else
                {
                    GO.transform.position = Position;
                }
            }
        }
    }

    public class IFX_Light_Helper : MonoBehaviour
    {
        private Light AttachedLight;
        public IFX_Light IFX;

        private float DefaultIntensity, DefaultRange;

        private void Awake()
        {
            AttachedLight = GetComponent<Light>();
            if (AttachedLight == null) Destroy(gameObject);

            DefaultIntensity = AttachedLight.intensity;
            DefaultRange = AttachedLight.range;
        }

        private void Start()
        {
            StartCoroutine(Action());
        }

        private IEnumerator Action()
        {
            float start = Time.time;
            float eval;
            while (Mathf.Abs(start - Time.time) < Mathf.Abs(IFX.Options.Duration))
            {
                eval = Mathf.Abs(start - Time.time);

                AttachedLight.intensity = IFX.LightOptions._IntensityCurve.Evaluate(eval) * DefaultIntensity;
                AttachedLight.range = IFX.LightOptions._RangeCurve.Evaluate(eval) * DefaultRange;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}