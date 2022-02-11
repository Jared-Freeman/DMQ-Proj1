using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mover
{
    public class Mover_ContinuousYaw : MonoBehaviour
    {
        [Header("deg / sec")]
        public float AngularSpeed = 1f;

        void Update()
        {
            transform.RotateAround(transform.position, Vector3.up, AngularSpeed * Time.deltaTime);
        }
    }
}