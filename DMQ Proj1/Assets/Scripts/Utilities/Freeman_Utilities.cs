using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//Author: Jared Freeman
//Desc: This class implements some helpful utility methods for use in Unity game design

public static class Freeman_Utilities
{
    public static float MapValueFromRangeToRange(float val_a, float range_a_start, float range_a_end, float range_b_start, float range_b_end)
    {
        float normal = Mathf.InverseLerp(range_a_start, range_a_end, val_a);
        return Mathf.Lerp(range_b_start, range_b_end, normal);
    }
}

namespace Utils
{
    public static class ComponentFinder<T> where T : MonoBehaviour
    {
        /// <summary>
        /// Acquires a list of Components that have a Collider attached as well.
        /// </summary>
        /// <param name="location">Location to perform spherecast check</param>
        /// <param name="radius">Radius of spherecast check</param>
        /// <param name="ignoredGameObjects">List of GameObjects that are ignored during the check.</param>
        /// <param name="checkChildren">Do we traverse children of found objects to find components? EXPENSIVE!</param>
        /// <returns>A list of all components of type within the specified radius IF THEY HAVE AN ATTACHED COLLIDER</returns>
        public static List<T> GetComponentsWithColliderInRadius(Vector3 location, float radius, List<GameObject> ignoredGameObjects = null, bool checkChildren = false)
        {
            List<T> listComponents = new List<T>();

            var cols = UnityEngine.Physics.OverlapSphere(location, radius);

            List<GameObject> listIgnored = ignoredGameObjects;
            if (listIgnored == null) listIgnored = new List<GameObject>();

            T currentComponent;
            foreach (var c in cols)
            {
                if(checkChildren)
                {
                    //somehow this space magic works...
                    foreach(Transform child in c.gameObject.transform)
                    {
                        //duplicate code... Too bad!
                        currentComponent = child.gameObject.GetComponent<T>();
                        if (currentComponent != null && !listIgnored.Contains(c.gameObject))
                        {
                            listComponents.Add(currentComponent);
                        }
                    }
                }

                currentComponent = c.gameObject.GetComponent<T>();
                if(currentComponent != null && !listIgnored.Contains(c.gameObject))
                {
                    listComponents.Add(currentComponent);
                }
            }

            return listComponents;
        }
    }

    /// <summary>
    /// Specifies who will receive this message. For use with Team Scriptable Object
    /// </summary>
    [System.Serializable]
    public class TargetFilterOptions
    {
        [Header("Checkbox == Damage message sent to this team (relative to owner of the message)")]
        public bool Enemy = true;
        public bool Ally = false;
        public bool Neutral = false;
        public bool Self = false;

        /// <summary>
        /// Returns true if Target is acceptable according to this team's perspective given the target filters
        /// </summary>
        /// <param name="InvokingTeam">Team this TargetFilter is being used relative to</param>
        /// <param name="Target">Target from the InvokingTeam's perspective</param>
        /// <returns></returns>
        public bool TargetIsAllowed(Team InvokingTeam, Actor Target)
        {
            //No team, no method of determining filter
            if (InvokingTeam == null) return true;

            if (Self && InvokingTeam.IsSelf(Target._Team))
            {
                return true;
            }
            else if (Ally && InvokingTeam.IsAlly(Target._Team))
            {
                return true;
            }
            else if (Neutral && InvokingTeam.IsNeutral(Target._Team))
            {
                return true;
            }
            else if (Enemy && InvokingTeam.IsEnemy(Target._Team))
            {
                return true;
            }

            return false;
        }
    }

    namespace Stats
    {

        /// <summary>
        /// State info on a current statistic. Includes modifier state data
        /// </summary>
        [System.Serializable]
        public struct StatInstance
        {
            public float Value;
            public StatModifierRecord Modifier;
        }

        /// <summary>
        /// A Stat Record composed of a Default record and an optional Modifier record
        /// </summary>
        [System.Serializable]
        public struct StatRecord
        {
            /// <summary>
            /// The defaults for this stat
            /// </summary>
            public StatDefaultRecord Default;
            /// <summary>
            /// The modifiers for this stat
            /// </summary>
            public StatModifierRecord Modifier;

            //public float Current
            //{
            //    get { return Current; }
            //    set
            //    {
            //        Current = Mathf.Clamp(value, Default.Min, Default.Max);
            //    }
            //}
        }


        /// <summary>
        /// A record for a stat, its multipliers, and 
        /// </summary>
        [System.Serializable]
        public struct StatDefaultRecord
        {
            public StatDefaultRecord(float max, float min = Mathf.NegativeInfinity)
            {
                Max = max;
                Min = min;
            }

            public float Max;
            public float Min;

        }

        /// <summary>
        /// A struct representing a MUTATION to a StatRecord
        /// </summary>
        [System.Serializable]
        public struct StatModifierRecord
        {
            public float Multiply;
            public float Add;
        }
    }

    /// <summary>
    /// Wraps CooldownTracker. A Cooldown with an additional Charge component 
    /// </summary>
    [System.Serializable]
    public class ChargeTracker
    {
        #region Ctor

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="baseCooldown">Cooldown between charge uses</param>
        /// <param name="options"></param>
        /// <param name="rechargeCooldown"></param>
        public ChargeTracker(CooldownTracker baseCooldown, CT_Options options = null, CooldownTracker rechargeCooldown = null)
        {
            Info.Cooldown_BetweenUses = new CooldownTracker(baseCooldown);

            if (rechargeCooldown != null) Info.Cooldown_ChargeRecharging = new CooldownTracker(rechargeCooldown);
            if(options != null) Settings = options;
        }

        #endregion

        #region Members

        public CT_Options Settings = new CT_Options();
        protected CT_Info Info = new CT_Info(); //access this stuff through properties

        #region Properties

        /// <summary>
        /// Can a charge be used right now?
        /// </summary>
        public bool ChargeAvailable 
        {
            get { return (Info.Cooldown_BetweenUses.CooldownAvailable && Info.ChargesRemaining > 0); }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Preset options
        /// </summary>
        [System.Serializable]
        public class CT_Options
        {
            //flags
            /// <summary>
            /// Should there be a cooldown that controls recharging charges?
            /// </summary>
            public bool FLAG_UseChargeRecharging = false; //NYI [2/7/2022]
            /// <summary>
            /// Should the recharge grant all charges at once?
            /// </summary>
            public bool FLAG_RegainAllCharges_ChargeRecharging = false;

            [Min(0)]
            public int MaxCharges;
        }

        /// <summary>
        /// State info
        /// </summary>
        [System.Serializable]
        public class CT_Info
        {
            /// <summary>
            /// The Cooldown specifies cooldown in between charges.
            /// </summary>
            public CooldownTracker Cooldown_BetweenUses;
            /// <summary>
            /// Cooldown to regain charges, if the FLAG is enabled
            /// </summary>
            public CooldownTracker Cooldown_ChargeRecharging;

            public int ChargesRemaining;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Consumes the number of charges specified by <paramref name="amount"/>
        /// </summary>
        /// <param name="amount">How many charges to consume. Usually just leave this set to 1.</param>
        /// <returns></returns>
        public bool ConsumeCharge(int amount = 1)
        {
            if(ChargeAvailable && Info.ChargesRemaining >= amount)
            {
                Info.ChargesRemaining -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// This Charge Tracker gains <paramref name="amount"/> charges
        /// </summary>
        /// <param name="amount">number of charges to regain</param>
        public void RegainCharges(int amount = 1)
        {
            Info.ChargesRemaining = Mathf.Clamp(Info.ChargesRemaining + amount, 0, Settings.MaxCharges);
        }

        #endregion
    }

    /// <summary>
    /// Maintains a generic _Cooldown based in (scaled) Time.
    /// </summary>
    [System.Serializable]
    public class CooldownTracker : System.IDisposable
    {
        #region Members

        private static bool s_FLAG_DEBUG = false;

        private static float s_MIN_COOLDOWN = .0001f;
        private static float s_MAX_COOLDOWN_RATE = 1 / s_MIN_COOLDOWN;

        //can probably replace with new property completely
        [Tooltip("Measured in seconds")]
        [Min(0.0001f)]
        [SerializeField] protected float _Cooldown = 1f;
        protected float _CooldownRate = 1f;
        //[Min(1)] public int MaxCharges = 1; //NYI

        private float LastUsedTime;

        /// <summary>
        /// Fires when the Cooldown is available to use again.
        /// </summary>
        /// <remarks>
        /// This is a LOCAL event. NOT globally-accessible!
        /// </remarks>
        public event System.EventHandler<CooldownTrackerEventArgs> OnCooldownAvailable;
        /// <summary>
        /// Fires when the Cooldown is consumed.
        /// </summary>
        /// <remarks>
        /// This is a LOCAL event. NOT globally-accessible!
        /// </remarks>
        public event System.EventHandler<CooldownTrackerEventArgs> OnCooldownUsed;

        /// <summary>
        /// Async controller for handling cancellation
        /// </summary>
        System.Threading.CancellationTokenSource _CancellationTokenSource = new System.Threading.CancellationTokenSource();

        #region Properties
        public float TimeRemaining
        {
            get
            {
                float val = Cooldown - (Time.time - LastUsedTime);
                return Mathf.Clamp(val, 0, Cooldown);
            }
        }

        //havent tested changing these during runtime
        public float CooldownRate 
        { 
            get { return _CooldownRate; }
            set
            {
                if(_CooldownRate <= s_MAX_COOLDOWN_RATE)
                {
                    _Cooldown = 1 / value;
                    _CooldownRate = value;
                }
            }
        }        
        /// <summary>
        /// The cooldown, in seconds
        /// </summary>
        public float Cooldown
        {
            get { return _Cooldown; }
            set
            {
                if (_CooldownRate >= s_MIN_COOLDOWN)
                {
                    _Cooldown = value;
                    _CooldownRate = 1 / value;
                }
            }
        }

        public bool CooldownAvailable
        { get
            {
                return CanUseCooldown();
            } 
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Event args for cooldown events.
        /// </summary>
        public class CooldownTrackerEventArgs : System.EventArgs
        {
            public CooldownTracker cooldown;

            public CooldownTrackerEventArgs(CooldownTracker cd)
            {
                cooldown = cd;
            }
        }

        #endregion

        #endregion

        #region Destructor

        /// <summary>
        /// Dtor releases any resources associated with this tracker (via async library)
        /// </summary>
        ~CooldownTracker()
        {
            _CancellationTokenSource.Dispose();
        }

        #endregion

        #region Constructors

        public CooldownTracker()
        {
            _CancellationTokenSource = new System.Threading.CancellationTokenSource(); // for some reason this doesnt work in base ctor.
                                                                                       // I placed heap init in member init instead ~Jared F
            //if(s_FLAG_DEBUG) Debug.Log("[CONSTRUCTOR]: ct source created");
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Other _Cooldown to retrieve values from.</param>
        public CooldownTracker(CooldownTracker other) : base()
        {
            if(other != null)
                Cooldown = other.Cooldown;
        }
        public CooldownTracker(float CooldownTime): base()
        {
            Cooldown = CooldownTime;
        }

        #endregion

        //Must be called to start running the _Cooldown tracker
        public void InitializeCooldown()
        {
            LastUsedTime = Time.time - Cooldown; //makes cooldown immediately avaiable
        }

        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        public void Dispose()
        {
            _CancellationTokenSource.Cancel(); //early cancel request
            _CancellationTokenSource.Dispose();
        }


        /// <summary>
        /// Places this tracker on cooldown, if it's available
        /// </summary>
        /// <returns>True if cooldown was consumed</returns>
        public bool ConsumeCooldown()
        {
            if(CanUseCooldown())
            {
                LastUsedTime = Time.time;

                if (_CancellationTokenSource == null)
                {
                    Debug.LogError("no source");
                    return false;
                }
                System.Threading.Tasks.Task t = DelayedCooldownAvailableTask(_CancellationTokenSource.Token);

                OnCooldownUsed?.Invoke(this, new CooldownTrackerEventArgs(this));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Schedules a CooldownAvailable event to fire once the cooldown duration has finished.
        /// </summary>
        protected async System.Threading.Tasks.Task DelayedCooldownAvailableTask(System.Threading.CancellationToken ct)
        {
            System.Threading.CancellationTokenSource cts = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                await System.Threading.Tasks.Task.Delay((int)(1000 * Cooldown) + 1, cts.Token);
                OnCooldownAvailable?.Invoke(this, new CooldownTrackerEventArgs(this));
            }
            catch (System.OperationCanceledException)
            {
                //how do we handle cancel?
                //OnCooldownAvailable?.Invoke(this, new CooldownTrackerEventArgs(this));
                if(s_FLAG_DEBUG) Debug.Log(ToString() + ": Task Canceled early!");
            }
            finally
            {
                if(s_FLAG_DEBUG) Debug.Log(ToString() + ": Disposal of Cancellation Token Source!");
                cts.Dispose();
            }
        }

        //Return true if _Cooldown CAN be used
        public bool CanUseCooldown()
        {
            if(Time.time - LastUsedTime >= _Cooldown) return true;
            return false;
        }
    }

    public static class AI
    {
        /// <summary>
        /// Finds a random point in the specified, 2.5D radius (where y is specified by center argument)
        /// </summary>
        /// <param name="center"> Center of circular search </param>
        /// <param name="MinRange"> Inner radius of circular search </param>
        /// <param name="MaxRange"> Outer radius of circular search </param>
        /// <param name="result"> Resulting point </param>
        /// <param name="CheckCount"> Number of checks before considered unsuccessful </param>
        /// <returns></returns>
        public static bool RandomPointInCircle(Vector3 center, float MinRange, float MaxRange, out Vector3 result, int CheckCount = 30)
        {
            if (MinRange < 0 || MaxRange < 0)
            {
                result = Vector3.zero;
                return false;
            }

            MinRange = Mathf.Min(MinRange, MaxRange);
            float MinSquared = MinRange * MinRange;
            float RandomRadius;
            float RandomAngle;
            Vector2 CircleSample;

            for (int i = 0; i < CheckCount; i++)
            {
                RandomRadius = Random.Range(MinRange, MaxRange);
                RandomAngle = Random.Range(0, 360);

                CircleSample.x = RandomRadius * Mathf.Cos(RandomAngle);
                CircleSample.y = RandomRadius * Mathf.Sin(RandomAngle);

                Vector3 randomPoint = center + new Vector3(CircleSample.x, center.y, CircleSample.y);
                UnityEngine.AI.NavMeshHit hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = Vector3.zero;
            return false;
        }
    }

    public static class TransformUtils
    {
        /// <summary>
        /// Changes the layer of all gameobjects in <paramref name="go"/>'s hierarchy, including <paramref name="go"/>
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer">layer. Unity layer range is [0,31] inclusive</param>
        /// <returns>true if successful</returns>
        public static bool ChangeLayerOfGameObjectAndChildren(GameObject go, int layer)
        {
            //bounds check
            if (layer < 0 || layer >= 32) return false;

            go.layer = layer;
            foreach (Transform t in go.transform)
            {
                t.gameObject.layer = layer;
            }

            return true;
        }
    }

    public static class Physics
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go">The GameObject instance containing Colliders to perform overlap checks on.</param>
        /// <returns>A list of gameobjects that are colliding with <paramref name="go"/>  </returns>
        public static List<GameObject> GetOverlappingGameObjects(GameObject go)
        {
            List<GameObject> List_Return = new List<GameObject>();

            var cs = go.GetComponentsInChildren<Collider>();

            foreach (Collider c in cs)
            {
                if (c.enabled == true)
                {
                    //functionality branches based on Collider type
                    if (c as BoxCollider != null)
                    {
                        var box = c as BoxCollider;

                        Collider[] overlapColliders;

                        overlapColliders = UnityEngine.Physics.OverlapBox(box.transform.position + box.center, box.size / 2, box.transform.rotation
                            , PhysicsCollisionMatrixLayerMasks.MaskForLayer(box.gameObject.layer));

                        foreach (var overlapC in overlapColliders)
                        {
                            if (
                                overlapC.gameObject != go 
                                && !overlapC.gameObject.transform.IsChildOf(go.transform)
                                && !List_Return.Contains(overlapC.gameObject)
                                )
                            {
                                List_Return.Add(overlapC.gameObject);
                            }
                        }

                    }
                    else if (c as SphereCollider != null)
                    {
                        var spr = c as SphereCollider;

                        Collider[] overlapColliders;

                        overlapColliders = UnityEngine.Physics.OverlapSphere(spr.transform.position + spr.center, spr.radius
                            , PhysicsCollisionMatrixLayerMasks.MaskForLayer(spr.gameObject.layer));

                        foreach (var overlapC in overlapColliders)
                        {
                            if (overlapC.gameObject != go 
                                && !overlapC.gameObject.transform.IsChildOf(go.transform)
                                && !List_Return.Contains(overlapC.gameObject))
                            {
                                List_Return.Add(overlapC.gameObject);
                            }
                        }
                    }
                    else if (c as CapsuleCollider != null)
                    {
                        var cap = c as CapsuleCollider;

                        //why did they not follow the same convention for Collider's...
                        Vector3 pointDir = cap.transform.up * (cap.height / 2 - cap.radius);

                        Collider[] overlapColliders;

                        overlapColliders = UnityEngine.Physics.OverlapCapsule(cap.transform.position + pointDir, cap.transform.position - pointDir, cap.radius
                            , PhysicsCollisionMatrixLayerMasks.MaskForLayer(cap.gameObject.layer));

                        foreach (var overlapC in overlapColliders)
                        {
                            if (overlapC.gameObject != go 
                                && !overlapC.gameObject.transform.IsChildOf(go.transform)
                                && !List_Return.Contains(overlapC.gameObject))
                            {
                                List_Return.Add(overlapC.gameObject);
                            }
                        }
                    }

                }
            }

            return List_Return;
        }

        /// <summary>
        /// Uses dimensions of colliders in <paramref name="go"/> AND transform data to see if the gameobject would collide with anything.
        /// </summary>
        /// <param name="go">GameObject to check.</param>
        /// <returns>True if a collision will occur on the next FixedUpdate</returns>
        /// <remarks>
        /// Only supports primitive 3D collider recognition. 
        /// Useful for seeing if an object would create collisions BEFORE FixedUpate is called (use for collision prevention?).
        /// </remarks>
        public static bool GameobjectCollisionExists(GameObject go)
        {
            //get bounds of object
            var cs = go.GetComponentsInChildren<Collider>();

            foreach (Collider c in cs)
            {
                if(c.enabled == true)
                {
                    //functionality branches based on Collider type
                    if (c as BoxCollider != null)
                    {
                        var box = c as BoxCollider;

                        Collider[] overlapColliders;

                        overlapColliders = UnityEngine.Physics.OverlapBox(box.transform.position + box.center, box.size/2, box.transform.rotation
                            , PhysicsCollisionMatrixLayerMasks.MaskForLayer(box.gameObject.layer));

                        foreach(var overlapC in overlapColliders)
                        {
                            if (overlapC.gameObject != go && !overlapC.gameObject.transform.IsChildOf(go.transform))
                            {
                                return true;
                            }
                        }

                    }
                    else if (c as SphereCollider != null)
                    {
                        var spr = c as SphereCollider;

                        Collider[] overlapColliders;

                        overlapColliders = UnityEngine.Physics.OverlapSphere(spr.transform.position + spr.center, spr.radius
                            , PhysicsCollisionMatrixLayerMasks.MaskForLayer(spr.gameObject.layer));

                        foreach (var overlapC in overlapColliders)
                        {
                            if (overlapC.gameObject != go && !overlapC.gameObject.transform.IsChildOf(go.transform))
                            {
                                return true;
                            }
                        }
                    }
                    else if (c as CapsuleCollider != null)
                    {
                        var cap = c as CapsuleCollider;

                        //why did they not follow the same convention for Collider's...
                        Vector3 pointDir = cap.transform.up * (cap.height / 2 - cap.radius);

                        Collider[] overlapColliders;

                        overlapColliders = UnityEngine.Physics.OverlapCapsule(cap.transform.position + pointDir, cap.transform.position - pointDir, cap.radius
                            , PhysicsCollisionMatrixLayerMasks.MaskForLayer(cap.gameObject.layer));

                        foreach (var overlapC in overlapColliders)
                        {
                            if (overlapC.gameObject != go && !overlapC.gameObject.transform.IsChildOf(go.transform))
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// Interface to access Unity CollisionMatrix to get collision masks per layer
        /// </summary>
        /// <remarks>
        /// Courtesy of user bellicapax on Unity Forums.
        /// See https://forum.unity.com/members/bellicapax.1178328/
        /// </remarks>
        public static class PhysicsCollisionMatrixLayerMasks
        {
            private static Dictionary<int, int> _masksByLayer;
            private static bool initialized = false;

            public static void Init()
            {
                _masksByLayer = new Dictionary<int, int>();
                for (int i = 0; i < 32; i++)
                {
                    int mask = 0;
                    for (int j = 0; j < 32; j++)
                    {
                        if (!UnityEngine.Physics.GetIgnoreLayerCollision(i, j))
                        {
                            mask |= 1 << j;
                        }
                    }
                    _masksByLayer.Add(i, mask);
                }
            }

            public static int MaskForLayer(int layer)
            {
                if (!initialized) 
                { 
                    Init();
                    initialized = true;
                }
                return _masksByLayer[layer];
            }
        }

        /// <summary>
        /// Helper struct for some Preset info for ComputeFixedContinuousMovement
        /// </summary>
        [System.Serializable]
        public struct CFC_MoveOptions
        {
            public float MoveSpd;
            public float Deceleration;
            public float Acceleration;
        }

        /// <summary>
        /// PROBABLY DEPRECATED. USE WITH CAUTION!
        /// </summary>
        /// <param name="RB"></param>
        /// <param name="DesiredVelocity"></param>
        /// <param name="RunOptions"></param>
        /// <returns>The resultant velocity inferred from <paramref name="DesiredVelocity"/> and <paramref name="RB"/>'s velocity</returns>
        public static Vector3 PerformFixedContinuousMovement(ref Rigidbody RB, Vector3 DesiredVelocity, ref CFC_MoveOptions RunOptions)
        {
            float MaxSpeed = 5f;
            Vector3 v = DesiredVelocity.normalized * MaxSpeed;
            v = DesiredVelocity; //hehehe

            //if ((v - RB.velocity).sqrMagnitude < v.sqrMagnitude) v = (v - RB.velocity);

            //F = ma = mv/t
            //Cache m/t (we compute v later)
            float c = RB.mass / Time.fixedDeltaTime;

            Vector3 result = v - RB.velocity;

            RB.AddForce(result * c, ForceMode.Force);
            //Debug.LogWarning(v);
            Debug.DrawRay(RB.transform.position + Vector3.up * 2f, result * 2f, Color.cyan);
            //Debug.DrawRay(RB.transform.position + Vector3.up * 2f, RB.velocity * 2f, Color.red);

            return result;
        }
    }
}
