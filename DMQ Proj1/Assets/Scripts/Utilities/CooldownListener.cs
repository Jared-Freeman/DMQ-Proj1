using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Updates a display based on an attached Cooldown
    /// </summary>
    public class CooldownListener
    {
        public CooldownTracker Cooldown { get; set; }

        /// <summary>
        /// Time remaining before cooldown is available. 0 -> cooldown is ready to use
        /// </summary>
        public float TimeRemaining
        {
            get
            {
                return Cooldown.TimeRemaining;
            }
        }

        /// <summary>
        /// TimeRemaining remapped to [0,1]
        /// </summary>
        public float PercentRemaining
        {
            get
            {
                return Freeman_Utilities.MapValueFromRangeToRange(
                    Cooldown.TimeRemaining,
                    0, Cooldown.Cooldown,
                    0, 1
                    );
            }
        }
    }
}