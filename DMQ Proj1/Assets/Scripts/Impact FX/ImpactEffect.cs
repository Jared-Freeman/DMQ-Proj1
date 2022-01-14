using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImpactFX
{
    public abstract class ImpactEffect : ScriptableObject
    {
        public bool FLAG_Debug = false;

        [System.Serializable]
        public struct FXOptions
        {
            public float Duration;
        }

        public FXOptions Options;

        // DESIGN PATTERN: attach to OptionalParent if != null
        public virtual void SpawnImpactEffect(GameObject OptionalParent, Vector3 Position, Vector3 DirectionForward = default, Vector3 DirectionNormal = default)
        {

        }
    }
}