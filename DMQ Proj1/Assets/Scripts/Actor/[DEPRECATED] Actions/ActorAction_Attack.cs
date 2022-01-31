using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem
{
    public class ActorAction_Attack : MonoBehaviour
    {
        public int DamageAmount = 1;

        [System.Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform attackRoot;

#if UNITY_EDITOR
            //editor only as it's only used in editor to display the path of the attack that is used by the raycast
            [NonSerialized] public List<Vector3> previousPositions = new List<Vector3>();
#endif

        }

        public GameObject hitParticlePrefab;
        public LayerMask targetLayers;

        public AttackPoint[] attackPoints = new AttackPoint[0];

        //later stuff
        [Header("Audio")] public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer attackAudio;

        protected bool m_InAttack = false;
        public GameObject m_Owner;

        protected Vector3[] m_PreviousPos = null;
        protected Vector3 m_Direction;

        protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
        protected static Collider[] s_ColliderCache = new Collider[32];

        public void BeginAttack(bool thowingAttack)
        {
            if (m_Owner)
            {
                DamageAmount = m_Owner.GetComponent<ActorStats>().totalAtk;
            }

            if (attackAudio != null)
                attackAudio.PlayRandomClip();

            m_InAttack = true;

            m_PreviousPos = new Vector3[attackPoints.Length];

            for (int i = 0; i < attackPoints.Length; ++i)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                                    attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                attackPoints[i].previousPositions.Clear();
                attackPoints[i].previousPositions.Add(m_PreviousPos[i]);
#endif
            }
        }
        public void EndAttack()
        {
            if (m_Owner)
            {
                DamageAmount -= m_Owner.GetComponent<ActorStats>().totalAtk;
            }
            m_InAttack = false;


#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                attackPoints[i].previousPositions.Clear();
            }
#endif
        }

        private void FixedUpdate()
        {
            if (m_InAttack)
            {
                for (int i = 0; i < attackPoints.Length; ++i)
                {
                    AttackPoint pts = attackPoints[i];
                    Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
                    Vector3 attackVector = worldPos - m_PreviousPos[i];

                    if (attackVector.magnitude < 0.001f)
                    {
                        // A zero vector for the sphere cast don't yield any result, even if a collider overlap the "sphere" created by radius. 
                        // so we set a very tiny microscopic forward cast to be sure it will catch anything overlaping that "stationary" sphere cast
                        attackVector = Vector3.forward * 0.0001f;
                    }


                    Ray r = new Ray(worldPos, attackVector.normalized);

                    int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache, attackVector.magnitude,
                        ~0,
                        QueryTriggerInteraction.Ignore);

                    for (int k = 0; k < contacts; ++k)
                    {
                        Collider col = s_RaycastHitCache[k].collider;

                        if (col != null)
                        {
                            CheckDamage(col, pts);
                        }
                    }

                    m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                    pts.previousPositions.Add(m_PreviousPos[i]);
#endif
                }
            }
        }

        private bool CheckDamage(Collider other, AttackPoint pts)
        {
            ActorStats d = other.GetComponent<ActorStats>();

            if (d == null)
            {
                //Debug.Log("hit " + other.gameObject.name + " no damageable detected");
                return false;
            }

            if (d.gameObject == m_Owner)
                return true; //ignore self harm, but do not end the attack (we don't "bounce" off ourselves)

            //Debug.Log("hit " + other.gameObject.name);

            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            {
                //hit an object that is not in our layer, this end the attack. we "bounce" off it
                return false;
            }

            if (hitAudio != null)
            {
                hitAudio.PlayRandomClip();
                Debug.Log("hitAudio played");
            }

            DamageMessage data;

            //Debug.Log("damage " + DamageAmount + " to " + other.gameObject.name);

            data.amount = DamageAmount;
            data.damager = this;
            data.direction = m_Direction.normalized;
            if (m_Owner)
                data.damageSource = m_Owner.transform.position;
            else
                data.damageSource = transform.position;
            data.stopCamera = false;
            data.FLAG_IgnoreInvulnerability = false;

            d.ApplyDamage(data);


            return true;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                AttackPoint pts = attackPoints[i];

                if (pts.attackRoot != null)
                {
                    Vector3 worldPos = pts.attackRoot.TransformVector(pts.offset);
                    Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                    Gizmos.DrawSphere(pts.attackRoot.position + worldPos, pts.radius);
                }

                if (pts.previousPositions.Count > 1)
                {
                    UnityEditor.Handles.DrawAAPolyLine(10, pts.previousPositions.ToArray());
                }
            }
        }

#endif
    }


}