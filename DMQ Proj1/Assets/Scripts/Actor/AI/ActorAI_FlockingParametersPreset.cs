using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorSystem.AI.Flocking
{
    /// <summary>
    /// Container for Flocking Parameters for use with <see cref="ActorAI_Logic"/> 
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

    }
}