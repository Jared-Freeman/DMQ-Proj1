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
    [System.Serializable]
    public class CooldownTracker
    {
        public float Cooldown = 1f;
        //[Min(1)] public int MaxCharges = 1;

        private float LastUsedTime;

        public CooldownTracker()
        {
        }
        public CooldownTracker(float CooldownTime): base()
        {
            Cooldown = CooldownTime;
        }

        //Must be called to start running the cooldown tracker
        public void InitializeCooldown()
        {
            LastUsedTime = Time.time;
        }

        //Returns true if Cooldown was used.
        public bool ConsumeCooldown()
        {
            if(CanUseCooldown())
            {
                LastUsedTime = Time.time;
                return true;
            }
            return false;
        }

        //Return true if Cooldown CAN be used
        public bool CanUseCooldown()
        {
            if(Time.time - LastUsedTime >= Cooldown) return true;
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
