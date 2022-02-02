using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorSystem
{
    [CreateAssetMenu(fileName = "Stats_", menuName = "Actor/Actor Stats Preset", order = 1)]
    public class ActorStatsPreset : ScriptableObject
    {
        [Header("Ignore modifier fields")]
        public ActorStatsData Data;
    }

    /// <summary>
    /// Container for the Actor Stats Preset
    /// </summary>
    [System.Serializable]
    public class ActorStatsData
    {
        //might eventually want to perform a method call to alter these values (i.e. turning them into properties)
        //(i.e., changing max health reduces current health by (newmax - max))
        public Utils.Stats.StatRecord HP;
        public Utils.Stats.StatRecord Energy;
    }

}