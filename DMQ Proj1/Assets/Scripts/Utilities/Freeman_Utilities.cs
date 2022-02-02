using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Jared Freeman
//Desc: This class implements some helpful utility methods for use in Unity game design

public static class Freeman_Utilities
{
    public static float MapValueFromRangeToRange(float val_a, float range_a_start, float range_a_end, float range_b_start, float range_b_end)
    {
        float normal = Mathf.InverseLerp(range_a_start, range_a_end, val_a);
        return Mathf.Lerp(range_b_start, range_b_end, normal);
    }
}

namespace Utils
{
    namespace Stats
    {
        /// <summary>
        /// A Stat Record composed of a Default, an optional Modifier, and the current stat
        /// </summary>
        [System.Serializable]
        public struct StatRecord
        {
            /// <summary>
            /// The defaults for this stat
            /// </summary>
            public StatDefaultRecord Default;
            /// <summary>
            /// The modifiers for this stat
            /// </summary>
            public StatModifierRecord Modifier;

            //public float Current
            //{
            //    get { return Current; }
            //    set
            //    {
            //        Current = Mathf.Clamp(value, Default.Min, Default.Max);
            //    }
            //}
        }


        /// <summary>
        /// A record for a stat, its multipliers, and 
        /// </summary>
        [System.Serializable]
        public struct StatDefaultRecord
        {
            public StatDefaultRecord(float max, float min = Mathf.NegativeInfinity)
            {
                Max = max;
                Min = min;
            }

            public float Max;
            public float Min;

        }

        /// <summary>
        /// A struct representing a temporary MUTATION to a StatRecord
        /// </summary>
        [System.Serializable]
        public struct StatModifierRecord
        {
            public float Multiply;
            public float Add;
        }
    }


    /// <summary>
    /// Maintains a generic _Cooldown based in (scaled) Time.
    /// </summary>
    [System.Serializable]
    public class CooldownTracker
    {
        #region Members

        private static float s_MIN_COOLDOWN = .0001f;
        private static float s_MAX_COOLDOWN_RATE = 1 / s_MIN_COOLDOWN;

        //can probably replace with new property completely
        [Tooltip("Measured in seconds")]
        [Min(0.0001f)]
        [SerializeField] protected float _Cooldown = 1f;
        protected float _CooldownRate = 1f;
        //[Min(1)] public int MaxCharges = 1; //NYI

        private float LastUsedTime;

        #endregion

        #region Properties

        //havent tested changing these during runtime
        public float CooldownRate 
        { 
            get { return _CooldownRate; }
            set
            {
                if(_CooldownRate <= s_MAX_COOLDOWN_RATE)
                {
                    _Cooldown = 1 / value;
                    _CooldownRate = value;
                }
            }
        }        
        public float Cooldown
        {
            get { return _Cooldown; }
            set
            {
                if (_CooldownRate >= s_MIN_COOLDOWN)
                {
                    _Cooldown = value;
                    _CooldownRate = 1 / value;
                }
            }
        }

        public bool CooldownAvailable
        { get
            {
                return CanUseCooldown();
            } 
        }

        #endregion

        public CooldownTracker()
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Other _Cooldown to retrieve values from.</param>
        public CooldownTracker(CooldownTracker other) : base()
        {
            if(other != null)
                Cooldown = other.Cooldown;
        }
        public CooldownTracker(float CooldownTime): base()
        {
            Cooldown = CooldownTime;
        }

        //Must be called to start running the _Cooldown tracker
        public void InitializeCooldown()
        {
            LastUsedTime = Time.time;
        }

        /// <summary>
        /// Places this tracker on cooldown, if it's available
        /// </summary>
        /// <returns>True if cooldown was consumed</returns>
        public bool ConsumeCooldown()
        {
            if(CanUseCooldown())
            {
                LastUsedTime = Time.time;
                return true;
            }
            return false;
        }

        //Return true if _Cooldown CAN be used
        public bool CanUseCooldown()
        {
            if(Time.time - LastUsedTime >= _Cooldown) return true;
            return false;
        }
    }

    public static class AI
    {
        /// <summary>
        /// Finds a random point in the specified, 2.5D radius (where y is specified by center argument)
        /// </summary>
        /// <param name="center"> Center of circular search </param>
        /// <param name="MinRange"> Inner radius of circular search </param>
        /// <param name="MaxRange"> Outer radius of circular search </param>
        /// <param name="result"> Resulting point </param>
        /// <param name="CheckCount"> Number of checks before considered unsuccessful </param>
        /// <returns></returns>
        public static bool RandomPointInCircle(Vector3 center, float MinRange, float MaxRange, out Vector3 result, int CheckCount = 30)
        {
            if (MinRange < 0 || MaxRange < 0)
            {
                result = Vector3.zero;
                return false;
            }

            MinRange = Mathf.Min(MinRange, MaxRange);
            float MinSquared = MinRange * MinRange;
            float RandomRadius;
            float RandomAngle;
            Vector2 CircleSample;

            for (int i = 0; i < CheckCount; i++)
            {
                RandomRadius = Random.Range(MinRange, MaxRange);
                RandomAngle = Random.Range(0, 360);

                CircleSample.x = RandomRadius * Mathf.Cos(RandomAngle);
                CircleSample.y = RandomRadius * Mathf.Sin(RandomAngle);

                Vector3 randomPoint = center + new Vector3(CircleSample.x, center.y, CircleSample.y);
                UnityEngine.AI.NavMeshHit hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = Vector3.zero;
            return false;
        }
    }
}
