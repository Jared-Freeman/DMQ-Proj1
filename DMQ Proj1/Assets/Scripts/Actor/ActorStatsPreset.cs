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
        public ActorStatsData()
        {
            //Defaults
            HP.Default.Max = 5;
            Energy.Default.Max = 5;
            MoveSpeed.Default.Max = 7.5f;

            //these would be annoying to set every time...
            HP.Modifier.Multiply = 1;
            Energy.Modifier.Multiply = 1;
            MoveSpeed.Modifier.Multiply = 1;
        }
        /// <summary>
        /// Param ctor useful for streamlining inspector defaults
        /// </summary>
        /// <param name="addDefaultValue">for modifier</param>
        /// <param name="multiplyDefaultValue">for modifier</param>
        public ActorStatsData(float addDefaultValue, float multiplyDefaultValue = 1f)
        {
            HP.Modifier.Add = addDefaultValue;
            Energy.Modifier.Add = addDefaultValue;
            MoveSpeed.Modifier.Add = addDefaultValue;

            HP.Modifier.Multiply = multiplyDefaultValue;
            Energy.Modifier.Multiply = multiplyDefaultValue;
            MoveSpeed.Modifier.Multiply = multiplyDefaultValue;
        }

        //might eventually want to perform a method call to alter these values (i.e. turning them into properties)
        //(i.e., changing max health reduces current health by (newmax - max))
        //TODO: Wrap in struct: {name, record} so this can be turned into a list??? Would be good to iterate over quickly
        public Utils.Stats.StatRecord HP;
        public Utils.Stats.StatRecord Energy;
        public Utils.Stats.StatRecord MoveSpeed;
    }

}