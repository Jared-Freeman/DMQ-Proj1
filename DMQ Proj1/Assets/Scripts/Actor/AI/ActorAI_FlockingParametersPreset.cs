using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorSystem.AI.Flocking
{
    /// <summary>
    /// Container for Flocking Parameters for use with <see cref="ActorAI_Logic"/>. The algorithm influences AI desired movement position.
    /// </summary>
    /// <remarks>
    /// Flocking is an AI formation behavior algorithm popularized by the Boids simulation originally developed by Craig Reynolds
    /// <para>Some possibly useful reading:</para>
    /// <para>https://www.oreilly.com/library/view/ai-for-game/0596005555/ch04.html</para>
    /// <para>https://cs.stanford.edu/people/eroberts/courses/soco/projects/2008-09/modeling-natural-systems/boids.html</para>
    /// </remarks>
    [CreateAssetMenu(fileName = "AI_FlockPreset_", menuName = "Actor/AI Flocking Preset", order = 1)]
    public class ActorAI_FlockingParametersPreset : ScriptableObject
    {
        /// <summary>
        /// Any Actors using this preset will display Flocking debug Gizmos and debug log output.
        /// </summary>
        public bool FLAG_Debug = false;

        public ActorAI_FlockingParametersPreset_Options Options = new ActorAI_FlockingParametersPreset_Options();

        #region Helpers

        [System.Serializable]
        public class ActorAI_FlockingParametersPreset_Options
        {
            [Range(0,1)]
            public float OverallFlockingStrength = 1;

            // The 3 flocking contributors. These weights will be normalized during computation.
            public ActorAI_FlockingParametersPreset_FlockingParameter Avoidance = new ActorAI_FlockingParametersPreset_FlockingParameter();
            public ActorAI_FlockingParametersPreset_FlockingParameter Cohesion = new ActorAI_FlockingParametersPreset_FlockingParameter();
            public ActorAI_FlockingParametersPreset_FlockingParameter Alignment = new ActorAI_FlockingParametersPreset_FlockingParameter();
        }

        [System.Serializable]
        public class ActorAI_FlockingParametersPreset_FlockingParameter
        {
            /// <summary>
            /// Valid actors in this radius will contribute to this Actor's flock computations.
            /// </summary>
            public float Radius = 0f;
            /// <summary>
            /// The Distance that this weight will begin to taper off from "1."
            /// </summary>
            public float MaxStrength = 3f;
            /// <summary>
            /// Steepness will make the strength taper more/less harshly.
            /// </summary>
            /// <remarks>
            ///     <para>
            ///     Low values look like a square wave (e.g. 0 to maxStrength instantly). High values look like a horizontal line at y=maxStrength.
            ///     </para>
            ///     <para>
            ///     Likely requires some playing with to get right, as it is influenced by other Flocking settings.
            ///     </para>
            /// </remarks>
            public float CurveSteepness = .25f;
        }

        #endregion
    }
}