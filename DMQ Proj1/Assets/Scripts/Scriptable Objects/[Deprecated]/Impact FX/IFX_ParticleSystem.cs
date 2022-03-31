using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImpactFX
{
    //[CreateAssetMenu(fileName = "IFX", menuName = "ScriptableObjects/Impact FX/Particle System", order = 1)]
    public class IFX_ParticleSystem : ImpactEffect
    {
        [System.Serializable]
        public struct ExtraOptions
        {
            public GameObject _PrefabWithParticleSystem;
        }

        public ExtraOptions ParticleOptions;

        public override void SpawnImpactEffect(GameObject OptionalParent, Vector3 Position, Vector3 DirectionForward = default, Vector3 DirectionNormal = default)
        {
            base.SpawnImpactEffect(OptionalParent, Position, DirectionForward, DirectionNormal);

            if (FLAG_Debug) Debug.Log("IFX Particle System Spawn(...)");

            if (ParticleOptions._PrefabWithParticleSystem == null)
            {
                Debug.LogError("IFX_Light called but Instance is not fully defined! Check the Light Options in the inspector!");
            }

            else
            {
                GameObject GO = Instantiate(ParticleOptions._PrefabWithParticleSystem);
                var h = GO.AddComponent<IFX_ParticleSystem_Helper>();
                h.IFX = this;

                if (OptionalParent != null)
                {
                    GO.transform.parent = OptionalParent.transform;
                    GO.transform.localPosition = Position;

                }
                else
                {
                    GO.transform.position = Position;
                }

                if (DirectionForward != default)
                {
                    GO.transform.forward = DirectionForward;
                }
            }
        }


        public class IFX_ParticleSystem_Helper : MonoBehaviour
        {
            private static float _RoutineWaitForDestroySeconds = 2f;

            private ParticleSystem _P;
            public IFX_ParticleSystem IFX;

            private float DefaultIntensity, DefaultRange;

            private void Awake()
            {
                _P = GetComponent<ParticleSystem>();
                if (_P == null) Destroy(gameObject);

                //var temp = _P.main;

                //temp.duration = IFX.Options.Duration;
                //temp.loop = false;
            }

            private void Start()
            {
                StartCoroutine(Action());
            }

            private IEnumerator Action()
            {
                _P.Play();

                yield return new WaitForSeconds(IFX.Options.Duration);

                _P.Stop();

                while(_P.particleCount > 0)
                {
                    yield return new WaitForSeconds(_RoutineWaitForDestroySeconds);
                }

                Destroy(gameObject);
            }
        }
    }
}