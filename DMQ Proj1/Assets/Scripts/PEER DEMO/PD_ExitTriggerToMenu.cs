using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PeerDemo
{
    public class PD_ExitTriggerToMenu : MonoBehaviour
    {
        public MainMenuLoader Loader;

        void OnTriggerEnter(Collider c)
        {
            if(c.gameObject.GetComponent<Actor_Player>() != null)
                Loader.LoadMainMenu();
        }
    }
}