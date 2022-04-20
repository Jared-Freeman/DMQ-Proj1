using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover_SmoothRotatePlayer : MonoBehaviour
{
    /// <summary>
    /// Rotation speed in degrees per second
    /// </summary>
    public float RotationSpeed = 120f; //deg/sec

    /// <summary>
    /// The rigidbody reference to gather velocity from.
    /// </summary>
    public Rigidbody ReferenceRigidbody;

    /// <summary>
    /// Multiplier that scales incoming velocity in order to help control for the effects of rapid, small velocity changes.
    /// </summary>
    /// <remarks>
    /// <para>i.e., an agent may be jostled in a tiny radius, causing the velocity to rapidly invert several times in a second.</para>
    /// <para>set to 1 for no relative contribution</para>
    /// </remarks>
    public AnimationCurve VelocityRelativeContribution = new AnimationCurve();

    /// <summary>
    /// The forward direction this component wishes to rotate toward
    /// </summary>
    public Vector3 DesiredRotationFacing { get; private set; }

    #region Initialization

    private void Awake()
    {
        //tests
        if (!Utils.Testing.ReferenceIsValid(ReferenceRigidbody)) Destroy(this);
        if (!Utils.Testing.ReferenceIsValid(VelocityRelativeContribution)) Destroy(this);
    }

    #endregion

    #region Updates

    private void FixedUpdate()
    {

    }

    private void Update()
    {

    }

    #endregion
}
