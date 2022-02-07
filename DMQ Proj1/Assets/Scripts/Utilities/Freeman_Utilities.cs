using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            public bool FLAG_UseChargeRecharging = false;
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
    public class CooldownTracker
    {
        #region Members

        private static float s_MIN_COOLDOWN = .0001f;
        private static float s_MAX_COOLDOWN_RATE = 1 / s_MIN_COOLDOWN;

        //can probably replace with new property completely
        [Tooltip("Measured in seconds")]
        [Min(0.0001f)]
        [SerializeField] protected float _Cooldown = 1f;
        protected float _CooldownRate = 1f;
        //[Min(1)] public int MaxCharges = 1; //NYI

        private float LastUsedTime;

        #endregion

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

        public CooldownTracker()
        {
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

        //Must be called to start running the _Cooldown tracker
        public void InitializeCooldown()
        {
            LastUsedTime = Time.time - Cooldown; //makes cooldown immediately avaiable
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
                return true;
            }
            return false;
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
        /// 
        /// </summary>
        /// <param name="RB"></param>
        /// <param name="DesiredVelocity"></param>
        /// <param name="RunOptions"></param>
        /// <returns>The resultant velocity inferred from <paramref name="DesiredVelocity"/> and <paramref name="RB"/>'s velocity</returns>
        public static Vector3 PerformFixedContinuousMovement(ref Rigidbody RB, Vector3 DesiredVelocity, ref CFC_MoveOptions RunOptions)
        {
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


            //F = ma = mv/t
            //Cache m/t (we compute v later)
            float ForceCoefficient = RB.mass / Time.fixedDeltaTime;

            Vector3 FilteredRBVelocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);

            //TODO: Improve: Clamp maximum allowable velocity gains based on some Acceleration

            ////Create direction of movement desired by player
            //Vector3 InputDirection = new Vector3(InputMap.x, 0, InputMap.y); //TODO: processing based on horizontal angle host
            //{
            //    float AngleDiff = Vector3.SignedAngle(FilteredRBVelocity.normalized, InputDirection, Vector3.up);

            //    //TODO: Consider adding these to movement properties (global scope)
            //    float MIN_ANGLE_TOLERANCE = 40.0f; //Very important. Controls the arc of player slide control when turning. Stay below 90!
            //    float MAX_ANGLE_TOLERANCE = 158.4f; //Less important. Controls the angle tolerance for braking. Stay above 90, and stay above MIN!

            //    //Preprocessing input. Clamp to within 180deg if we are going beyond standard movement speed
            //    //TODO: verify angle computation is correct in all cases
            //    if (
            //        FilteredRBVelocity.sqrMagnitude != 0
            //        && (FilteredRBVelocity.sqrMagnitude > RunOptions.MoveSpd * RunOptions.MoveSpd)
            //        && Mathf.Abs(AngleDiff) > MIN_ANGLE_TOLERANCE
            //        && Mathf.Abs(AngleDiff) < MAX_ANGLE_TOLERANCE
            //        )
            //    {
            //        float AngleDelta = 0;
            //        if (AngleDiff > 0) AngleDelta = -(MIN_ANGLE_TOLERANCE - Mathf.Abs(AngleDiff));
            //        else AngleDelta = (MIN_ANGLE_TOLERANCE - Mathf.Abs(AngleDiff));

            //        float SinD, CosD;
            //        SinD = Mathf.Sin(Mathf.Deg2Rad * AngleDelta);
            //        CosD = Mathf.Cos(Mathf.Deg2Rad * AngleDelta);

            //        //reassign clamped value
            //        InputDirection = new Vector3(
            //            (CosD * InputDirection.x - SinD * InputDirection.z)
            //            , InputDirection.y
            //            , (SinD * InputDirection.x + CosD * InputDirection.z)
            //            );

            //    }
            //}
            //Naive model
            //if(InputDirection.sqrMagnitude > 1) InputDirection = InputDirection.normalized;
            //DesiredVelocity = Vector3.zero; if (InputDirection.sqrMagnitude > 0) DesiredVelocity = InputDirection * MoveSpd;

            //If a direction exists, accelerate towards it. otherwise decelerate towards 0
            //if (/*InputDirection.sqrMagnitude == 0*/ false)
            //{
            //    //apply the brakes

            //    //for comparison to make sure we dont go in the opposite direction
            //    Vector3 Temp = DesiredVelocity;

            //    DesiredVelocity = DesiredVelocity - (DesiredVelocity.normalized * RunOptions.Acceleration * Time.fixedDeltaTime);

            //    //clamp to 0
            //    if (Vector3.Dot(DesiredVelocity, Temp) < 0) DesiredVelocity = Vector3.zero;
            //}
            //else
            //{
            //    //accel
            //    DesiredVelocity = DesiredVelocity + InputDirection.normalized * RunOptions.Acceleration * Time.fixedDeltaTime;

            //    //clamp desired velocity max to move speed max
            //    if (DesiredVelocity.sqrMagnitude > RunOptions.MoveSpd * RunOptions.MoveSpd)
            //        DesiredVelocity = DesiredVelocity.normalized * RunOptions.MoveSpd;

            //}




            Vector3 DesiredVelocityDiff = Vector3.zero;

            //Filtered velocity represents the maximum velocity we can affect in this timestep (the rigidbody can exceed this by a LOT in normal gameplay)
            Vector3 FilteredVelocity = FilteredRBVelocity;
            //max is clamped to current player-desired velocity max.
            if (FilteredVelocity.sqrMagnitude > DesiredVelocity.sqrMagnitude) FilteredVelocity = DesiredVelocity.magnitude * FilteredVelocity.normalized;

            //compute parallel component of current velocity to desired velocity
            if (DesiredVelocity.sqrMagnitude != 0) // parallel component of RB velocity to the intended velocity
                DesiredVelocityDiff = (Vector3.Dot(DesiredVelocity, FilteredVelocity) / DesiredVelocity.magnitude) * DesiredVelocity.normalized;


            //GENERATE ADDVELOCITY
            Vector3 AddVelocity;

            //Recall that we are comparing parallel vectors here
            if (Vector3.Dot(DesiredVelocity, FilteredRBVelocity) < 0)
            {
                //decelerating. Parallel component of velocity can be at MOST Deceleration * Time.FixedDeltaTime            
                Vector3 Parallel = Vector3.zero;
                Vector3 Perpendicular = Vector3.zero;

                Parallel = (Vector3.Dot(DesiredVelocity, FilteredVelocity) / FilteredVelocity.magnitude) * FilteredVelocity.normalized;
                //Perpendicular = DesiredVelocity - Parallel;

                AddVelocity = (Parallel.normalized * RunOptions.Deceleration * Time.fixedDeltaTime) + Perpendicular;
                //AddVelocity = ((-Parallel.normalized * Deceleration * Time.fixedDeltaTime) + Perpendicular);


                //AddVelocity = (DesiredVelocity - DesiredVelocityDiff);
                //if (AddVelocity.sqrMagnitude > Deceleration * Deceleration * Time.fixedDeltaTime * Time.fixedDeltaTime) AddVelocity = AddVelocity.normalized * Deceleration * Time.fixedDeltaTime;
            }
            else
            {
                //sustaining velocity
                AddVelocity = (DesiredVelocity - DesiredVelocityDiff);

            }



            //Damp (within standard movement speed control)
            if (/*InputDirection.sqrMagnitude > 0 && */FilteredRBVelocity.sqrMagnitude <= RunOptions.MoveSpd * RunOptions.MoveSpd) //First check was causing sloppy deceleration within standard MoveSpd
            {
                RB.AddForce(AddVelocity * ForceCoefficient, ForceMode.Force); //TODO: Consider projecting this onto surface normal of whatever we're standing on (for movement along slopes)

                RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);



                //old idea
                //if ((AddVelocity).sqrMagnitude < (FilteredRBVelocity - DesiredVelocityDiff).sqrMagnitude)
                //{
                //    //RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff).normalized * AddVelocity.magnitude * ForceCoefficient, ForceMode.Force);
                //    RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
                //}
                //else
                //{
                //    RB.AddForce(-(FilteredRBVelocity - DesiredVelocityDiff) * ForceCoefficient, ForceMode.Force);
                //}

            }
            //Damp (exceeding movement speed max)
            else
            {
                //"Parachute" idea: Add the velocity * forceCoefficient, but pull "backward" on the rigidbody by the amount needed to maintain current velocity (a direction change, but not a velocity one)

                Vector3 ModifiedVelocity = AddVelocity + FilteredRBVelocity;

                Vector3 ParachuteVelocity = Vector3.zero;
                //if (ModifiedVelocity.sqrMagnitude > FilteredRBVelocity.sqrMagnitude) ParachuteVelocity = -(FilteredRBVelocity - ModifiedVelocity).magnitude * ModifiedVelocity.normalized;
                if (ModifiedVelocity.sqrMagnitude > FilteredRBVelocity.sqrMagnitude) ParachuteVelocity = Mathf.Abs(ModifiedVelocity.magnitude - FilteredRBVelocity.magnitude) * -ModifiedVelocity.normalized; //TODO: Optimize this!

                RB.AddForce((AddVelocity + ParachuteVelocity) * ForceCoefficient, ForceMode.Force); //TODO: Consider projecting this onto surface normal of whatever we're standing on (for movement along slopes)

            }

            //uhh...
            return Vector3.zero;
        }
    }
}
