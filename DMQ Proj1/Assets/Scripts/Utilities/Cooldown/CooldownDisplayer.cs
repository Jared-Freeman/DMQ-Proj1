using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class CooldownDisplayer : MonoBehaviour
    {
        public CooldownListener Listener { get; private set; } = new CooldownListener();

        private void Update()
        {
            if(Listener.Cooldown != null)
            {
                Debug.DrawRay(transform.position, transform.right * Listener.PercentRemaining * 3f, Color.green);
            }
        }
    }
}
