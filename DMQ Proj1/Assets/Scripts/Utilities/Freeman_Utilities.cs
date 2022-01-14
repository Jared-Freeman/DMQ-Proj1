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
}
