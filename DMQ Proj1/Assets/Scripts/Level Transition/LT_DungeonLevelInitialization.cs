using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelTransition
{
    public class LT_DungeonLevelInitialization : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(InitializeElements());
        }

        /// <summary>
        /// Initializes a dungeon level
        /// </summary>
        public IEnumerator InitializeElements()
        {
            //wait a few frames for init computations
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            //ensure camera is in right place
            Singleton<Topdown_Multitracking_Camera_Rig>.Instance.WarpToCurrentDesiredLocation();
            //ensure game state is Gameplay
            Singleton<GameState.GameStateManager>.Instance.InvokeResume();
        }
    }
}