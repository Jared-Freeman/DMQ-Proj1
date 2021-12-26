using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HOW TO ADD A NEW MOVEMENT STYLE:
// 1. Add a MovementStyle enum
// 2. Add a case in the UpdateMovement() AND FixedUpdateMovement() switch statements for your enum (with an appropriately named method). 
//    * See how I did it if you're not super sure what to do.
// 3. Add a [System.Serializable] struct (using your naming convention) containing public movement options for your method
// 4. Add this struct as a public member to the MovementStyleOptions struct
// 5. Implement your method! Note that your method is being called in Update()

// This mover is designed to be super modular. You may add your own MovementStyle by following the format above.
// Since MovementStyle's are implemented JUST in this class, I dont have a good solution for keeping state variable memory footprint low... maybe we can figure something out eventually
// For now just implement your state variables in this class if/when needed. I provided a few basic ones like ProjectileTimeAlive for you.
public class GenericProjectileMover : MonoBehaviour
{
    #region Members

    public enum MovementStyle { None, LinearSimple, ParabolicSimple, PhysicsImpulse, PhysicsContinuousForce, HomingSimple };
    [SerializeField] MovementStyle MovementType = MovementStyle.LinearSimple; //DO NOT CHANGE THIS DURING RUNTIME!    

    public MovementStyleOptions MovementTypeOptions;

    //state variables (you can use these however you want in your mover)
    //counters
    float ProjectileTimeAlive = 0;
    float ProjectileCollisions = 0;



    #endregion

    #region Movement Method Initialization
    void InitializeMovementMethod()
    {
        switch (MovementType)
        {
            case MovementStyle.None:
                break;

            case MovementStyle.LinearSimple:
                LinearSimpleMovement();
                break;

            case MovementStyle.ParabolicSimple:
                ParabolicSimpleMovement();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsContinuousForce:
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsImpulse:
                break;

            case MovementStyle.HomingSimple:
                break;
        };
    }
    #endregion

    #region Update Methods
    void Update()
    {
        UpdateMovement();
        ProjectileTimeAlive += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        FixedUpdateMovement();
    }

    void UpdateMovement()
    {
        switch(MovementType)
        {
            case MovementStyle.None:
                break;

            case MovementStyle.LinearSimple:
                LinearSimpleMovement();
                break;

            case MovementStyle.ParabolicSimple:
                ParabolicSimpleMovement();
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsContinuousForce:
                break;

            //Impl in Fixed FixedUpdateMovement()
            case MovementStyle.PhysicsImpulse:
                break;

            case MovementStyle.HomingSimple:
                HomingSimpleMovement();
                break;

            default:
                Debug.LogError("MovementStyle has no implementation method specified!");
                break;
        };
    }

    void FixedUpdateMovement()
    {

        switch (MovementType)
        {
            case MovementStyle.None:
                break;

            //Impl in Fixed UpdateMovement()
            case MovementStyle.LinearSimple:
                break;

            //Impl in Fixed UpdateMovement()
            case MovementStyle.ParabolicSimple:
                break;

            case MovementStyle.PhysicsContinuousForce:
                PhysicsContinuousForceMovement();
                break;

            case MovementStyle.PhysicsImpulse:
                PhysicsImpulseMovement();
                break;

            //Impl in Fixed UpdateMovement()
            case MovementStyle.HomingSimple:
                break;

            default:
                Debug.LogError("MovementStyle has no implementation method specified!");
                break;
        };
    }
    #endregion

    #region Movement Methods
    /// <summary>
    /// A container struct for organizing options in the inspector. All MovementOptions structs should have a member here!
    /// </summary>
    [System.Serializable]
    public struct MovementStyleOptions
    {
        public LinearSimpleMovementOptions LinearSimpleOptions;
        public ParabolicSimpleMovementOptions ParabolicSimpleOptions;
        public PhysicsContinuousForceMovementOptions PhysicsContinuousForceOptions;
        public LinearSimpleMovementOptions PhysicsImpulseOptions;
        public LinearSimpleMovementOptions HomingSimpleOptions;
    }

    /// <summary>
    /// Linear Simple movement simply moves the object in a direction at a given speed. 
    /// Designers may choose to have the projectile time out and/or disappear after a certain collision count
    /// </summary>
    [System.Serializable]
    public struct LinearSimpleMovementOptions
    {
        public Vector3 InitialDirection;
        public float Speed;

        public float MaxProjectileDuration;
        public int MaxCollisions;
    };

    void LinearSimpleMovement()
    {
        //transform.position += MovementTypeOptions.LinearSimpleOptions.InitialDirection.normalized * MovementTypeOptions.LinearSimpleOptions.Speed * Time.deltaTime;

        transform.Translate(MovementTypeOptions.LinearSimpleOptions.InitialDirection.normalized * MovementTypeOptions.LinearSimpleOptions.Speed * Time.deltaTime);
        if(ProjectileTimeAlive > MovementTypeOptions.LinearSimpleOptions.MaxProjectileDuration)
        {

        }
    }



    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct ParabolicSimpleMovementOptions
    {
        public Vector3 InitialDirection;
        public float Speed;
    };

    void ParabolicSimpleMovement()
    {

    }



    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct PhysicsContinuousForceMovementOptions
    {
        public Vector3 InitialDirection;
        public float Speed;
    };

    void PhysicsContinuousForceMovement()
    {

    }



    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct PhysicsImpulseMovementOptions
    {
        public Vector3 InitialDirection;
        public float Speed;
    };

    void PhysicsImpulseMovement()
    {

    }



    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct HomingSimpleMovementOptions
    {
        public Vector3 InitialDirection;
        public float Speed;
    };

    void HomingSimpleMovement()
    {

    }



    #endregion

    //Can probably add event handlers PER movement style by just adding a switch statement into any event handler here
    #region Event Handlers
    private void OnCollisionEnter(Collision collision)
    {
        ProjectileCollisions++;
    }
    #endregion
}
