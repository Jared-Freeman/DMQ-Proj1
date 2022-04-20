using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelTransition
{
    public class LT_DungeonLevelInitialization : MonoBehaviour
    {
        /// <summary>
        /// The duration the game is paused before letting players move
        /// </summary>
        public float GameStartDelay = 16f;

        void Start()
        {
            StartCoroutine(InitializeElements(GameStartDelay));
        }

        /// <summary>
        /// Initializes a dungeon level
        /// </summary>
        public IEnumerator InitializeElements(float delay_time)
        {
            float startTime = Time.unscaledTime;

            //wait a few frames for init computations
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            //ensure camera is in right place
            Singleton<Topdown_Multitracking_Camera_Rig>.Instance.WarpToCurrentDesiredLocation();


            //ensure game state is Gameplay
            //Singleton<GameState.GameStateManager>.Instance.InvokeResume();

            float curTime = Time.unscaledTime;
            while(Mathf.Abs(curTime - startTime) < delay_time)
            {
                Singleton<GameState.GameStateManager>.Instance.InvokePause();

                yield return null;
            }
            Singleton<GameState.GameStateManager>.Instance.InvokeResume();
        }
    }
}