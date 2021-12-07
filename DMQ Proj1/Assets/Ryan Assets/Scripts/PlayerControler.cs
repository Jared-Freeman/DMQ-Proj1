using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControler : MonoBehaviour
{
    #region Members
    Inventory inventory;
    int resetAttackStatehash = Animator.StringToHash("lastStrike");
    #region variables
    [Header("variables")]
    [Range(.1f, 500f)]
    [Tooltip("m / sec^2")]
    public float acceleration = 1;

    [Range(.1f, 500f)]
    [Tooltip("m / sec^2")]
    public float damping = 1;

    [Tooltip("Meters / sec")]
    public float MoveSpd = 1;
    public float dashMultiplier = 2;
    public float jumpHeight = 1;
    public bool dashing = false;
    #endregion

    #region External components
    [Space(10)]

    [Header("External components")]
    public GameObject HorizontalMovementAngleHost;
    [SerializeField]
    private Rigidbody RB;
    
    private PlayerControls controls;
    public PlayerInput Input;
    public Animator anim;
    #endregion

    #region Movement maps
    [Space(20)]

    [Header("Movement maps")]
    //affects horizontal movement velocity. Controlled via input
    [SerializeField]
    Vector2 VelocityMap = Vector2.zero; 
    
    [SerializeField]
    Vector2 InputMap = Vector2.zero;
    
    [SerializeField]
    Vector2 DragVector = Vector2.zero;
    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<Inventory>();
        RB = gameObject.GetComponent<Rigidbody>();
        if (HorizontalMovementAngleHost == null)
        {
            HorizontalMovementAngleHost = gameObject;
        }
        controls = new PlayerControls();
        InitializePlayer();
    }

    public void InitializePlayer()
    {
        Input.onActionTriggered += Input_onActionTriggered;
    }

    void Update()
    {
        if (new Vector3(RB.velocity.x, 0, RB.velocity.z).sqrMagnitude > MoveSpd * MoveSpd * 4)
        {
            Debug.Log("Exceeding");
        }
        if(anim.GetCurrentAnimatorStateInfo(0).tagHash == resetAttackStatehash)
        {
            anim.ResetTrigger("AttackTrigger");
        }
        if (anim.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("dashing"))
        {
            anim.ResetTrigger("Dash");
        }
        
    }

    private void FixedUpdate()
    {
        //if (new Vector3(RB.velocity.x, 0, RB.velocity.z).sqrMagnitude < MoveSpd * MoveSpd * 4)
        HorizontalMovementInput();
        UpdateRotation();

        //RB.AddForce(Vector3.down * AddedDownwardForce * RB.mass * Time.fixedDeltaTime, ForceMode.Force);
    }

    #region Input Handlers
    public Vector2 desiredVelocity = Vector2.zero;
    public Vector2 normalizdVelocity = Vector2.zero;
    public float normalizdVelocityMagnitude = 0;
    private void HorizontalMovementInput()
    {
        Vector3 RetainedVelocity = RB.velocity;

        if (InputMap.sqrMagnitude > 1)
        {
            InputMap.Normalize();
        }

        DragVector = Vector3.zero;
        //kill all stuff not in the dir we wanna go
        if (InputMap != Vector2.zero)
        {
            DragVector = Vector2.Perpendicular(InputMap);
        }
        else
        {
            DragVector = -1 * VelocityMap;
        }

        DragVector.Normalize();

        DragVector *= Vector2.Dot(DragVector, -1 * VelocityMap) * Time.fixedDeltaTime * damping;

        desiredVelocity = InputMap * MoveSpd;
        if(dashing)
        {
            desiredVelocity *= dashMultiplier;
        }

        float acc = InputMap.magnitude > 0.1f ? acceleration : damping;

        if (VelocityMap.sqrMagnitude > MoveSpd)
        {
            //VelocityMap = VelocityMap.normalized * MoveSpd; //max spd clamp
            acc = damping;
        }

        VelocityMap.x = Mathf.MoveTowards(VelocityMap.x, desiredVelocity.x, acc );
        VelocityMap.y = Mathf.MoveTowards(VelocityMap.y, desiredVelocity.y, acc);

        normalizdVelocity = VelocityMap / MoveSpd;
        normalizdVelocityMagnitude = normalizdVelocity.magnitude;
        anim.SetFloat("Velocity", normalizdVelocityMagnitude);
        if(VelocityMap.magnitude > 0.1f)
        {
            anim.SetBool("Moving", true);
        }
        else
        {
            anim.SetBool("Moving", false);
        }

        Vector3 VelocityMap3 = new Vector3(VelocityMap.x, 0, VelocityMap.y);

        //TODO: Re-add retained velocity information back into RB calculations. Else we don't respect most rigidbody interactions (e.g. falling doesn't work)
        //VelocityMap3 += RetainedVelocity; //for falling, etc

        //Debug.DrawRay(gameObject.transform.position, new Vector3(DragVector.x, 0, DragVector.y), Color.red);
        //Debug.DrawRay(gameObject.transform.position, new Vector3(InputMap.x, 0, InputMap.y), Color.blue);
        //Debug.DrawRay(gameObject.transform.position, new Vector3(VelocityMap.x, 0, VelocityMap.y), Color.green);

        RB.AddForce(VelocityMap3 - RB.velocity, ForceMode.VelocityChange);
    }
    private void UpdateRotation()
    {
        Vector2 movementInput = InputMap.normalized;
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
    private void JumpInput()
    {
        //Debug.Log("JumpInput");

        StopCoroutine(JumpStart());
        StartCoroutine(JumpStart());

        //float JumpSpeed = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
        //RB.AddForce(JumpSpeed * Vector3.up, ForceMode.VelocityChange);

        //StartCoroutine(JumpNormal(1));
        //RB.AddForce(HorizontalMovementAngleHost.transform.forward * 5 * RB.mass, ForceMode.Impulse);
    }

    public void DashReset()
    {
        dashing = false;
    }

    public void DashSet()
    {
        dashing = true;
    }
    #endregion

    public IEnumerator JumpNormal(float duration)
    {
        float StartTime = Time.time;
        float LastCurTime = Time.time;
        float CurTime = Time.time;
        while (Mathf.Abs(CurTime - StartTime) < duration)
        {
            LastCurTime = CurTime;
            CurTime = Time.time;
            RB.AddForce(HorizontalMovementAngleHost.transform.forward * 50 * RB.mass * (Mathf.Abs(CurTime - LastCurTime) / duration), ForceMode.Force);

            yield return new WaitForFixedUpdate();
        }

    }
    public IEnumerator JumpStart()
    {
        yield return new WaitForFixedUpdate();

        float JumpSpeed = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        RB.AddForce(JumpSpeed * Vector3.up, ForceMode.VelocityChange);

    }

    private void Input_onActionTriggered(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == controls.Player.Movement.name)
        {
            InputMap = ctx.ReadValue<Vector2>();
        }
        if (ctx.action.name == controls.Player.Jump.name)
        {
            if (ctx.performed)
            {
                //JumpInput();
            }
        }
        if (ctx.action.name == controls.Player.Attack.name)
        {
            if (ctx.performed)
            {
                anim.SetTrigger("AttackTrigger");
            }
            
        }
        if (ctx.action.name == controls.Player.SpecialAction.name)
        {
            if (ctx.performed)
            {
                SpecialActionHandle();
            }

        }
        if (ctx.action.name == controls.Player.Wepon1Equip.name)
        {
            if (ctx.performed)
            {
                if (inventory.currentEquipNumber >= 0)
                    anim.SetInteger("OldWeapon", inventory.allPossibleWapons[inventory.equipWep[inventory.currentEquipNumber].index].weaponAnimType);
                else
                    anim.SetInteger("OldWeapon", -1);
                if (inventory.currentEquipNumber == 0)
                {
                    anim.SetInteger("Weapon", -1);
                    inventory.changeToNumber = -1;
                }
                else
                {
                    anim.SetInteger("Weapon", inventory.allPossibleWapons[inventory.equipWep[0].index].weaponAnimType);
                    inventory.changeToNumber = 0;
                }
                anim.SetTrigger("WeaponChangeTrigger");

            }
        }
        if (ctx.action.name == controls.Player.Wepon2Equip.name)
        {
            if (ctx.performed)
            {
                if (inventory.currentEquipNumber == 1)
                {
                    anim.SetInteger("Weapon", -1);
                    inventory.changeToNumber = -1;
                }
                else
                {
                    anim.SetInteger("Weapon", inventory.allPossibleWapons[inventory.equipWep[1].index].weaponAnimType);
                    inventory.changeToNumber = 1;
                }
                anim.SetTrigger("WeaponChangeTrigger");
            }
        }
        if (ctx.action.name == controls.Player.QuitGame.name)
        {
            if (ctx.performed)
            {
                Application.Quit();
            }
        }
    }
    void SpecialActionHandle()
    {
        if(inventory.allPossibleWapons[inventory.equipWep[inventory.currentEquipNumber].index].specialAction == SpecialAction.Dash)
        {
            anim.SetTrigger("SpecialAction");
        }
    }
}
