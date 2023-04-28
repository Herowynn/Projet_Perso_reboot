using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	#region variables

	[Header("Movement")]
    [SerializeField] float _moveSpeed;
    public float WalkSpeed;
    public float SprintSpeed;
    public float WallRunSpeed;
    public float GroundDrag;
	public Transform Orientation;
	private Vector3 _moveDirection;

	[Header("Jump")]
    public float JumpForce;
    public float JumpColldown;
    public float AirMultiplier;
    private bool _readyToJump;

    [Header("Crouching")]
    public float CrouchSpeed;
    public float CrouchYScale;
    private float _startYScale;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    private float _playerHeight;
    public LayerMask WhatIsGround;

    [Header("Slope Handling")]
    public float MaxSloapAngle;
    private RaycastHit _slopeHit;
    private bool _exitingSlope;

    [Header("Inputs")]
    private float _horizontalInput;
    private float _verticalInput;

    [Header("Player References")]
    public Rigidbody Rb;

    [Header("Current State")]
    public MovementState State;
    public bool IsWallRunning;
    public bool IsFrozen;

	#endregion

	#region Default Functions

	private void Awake()
	{
		Rb = GetComponent<Rigidbody>();
	}

	private void Start()
    {
        _readyToJump = true;

        _startYScale = transform.localScale.y;

        _playerHeight = GetComponentInChildren<MeshRenderer>().bounds.size.y;
    }

    private void Update()
    {
        InputDetection();
        SpeedControl();
        StateHandler();

        if (Grounded())
            Rb.drag = GroundDrag;
        else
            Rb.drag = 0f;
    }

	private void FixedUpdate()
	{
		MovePlayer();
	}

	#endregion

	#region My Functions

	public bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * .5f + .2f, WhatIsGround);
    }

    private void InputDetection()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(JumpKey) && _readyToJump && Grounded())
        {
            _readyToJump = false;

            Jump();

            StartCoroutine(ResetJump());
        }

        else if (Input.GetKeyDown(CrouchKey) && Grounded())
        {
            _readyToJump = false;
            transform.localScale = new Vector3(transform.localScale.x, CrouchYScale, transform.localScale.z);
            Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        else if(Input.GetKeyUp(CrouchKey) && CanUncrouch())
        {
            _readyToJump = true;
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }

    private bool CanUncrouch()
    {
        return !Physics.Raycast(transform.position, Vector3.up, _playerHeight * .5f + .2f);
    }

	private void OnTriggerStay(Collider other)
	{
        if (other.tag == "MovingPlatform")
            transform.SetParent(other.transform);
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag == "MovingPlatform")
            transform.SetParent(null);
	}

	private void StateHandler()
    {
        if (IsFrozen)
        {
            Rb.velocity = Vector3.zero;
            _moveSpeed = 0;
        }

        else if (IsWallRunning)
        {
            State = MovementState.WallRunning;
            _moveSpeed = WallRunSpeed;
        }

        else if(Input.GetKey(CrouchKey) && Grounded())
        {
            State = MovementState.Crouching;
            _moveSpeed = CrouchSpeed;
        }

        else if(Grounded() && Input.GetKey(SprintKey))
        {
            State = MovementState.Sprinting;
            _moveSpeed = SprintSpeed;
        }

        else if (Grounded())
        {
            State = MovementState.Walking;
            _moveSpeed = WalkSpeed;
        }

        else
        {
            State = MovementState.Air;
        }
    }

    private void MovePlayer()
    {
        if (IsFrozen)
            return;

        _moveDirection = Orientation.forward * _verticalInput + Orientation.right * _horizontalInput;

        if (OnSlope() && !_exitingSlope)
        {
            Rb.AddForce(GetSlopeMoveDirection(_moveDirection) * _moveSpeed * 20f, ForceMode.Force);

            if (Rb.velocity.y > 0)
                Rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if(Grounded())
            Rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);

        else if (!Grounded())
            Rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * AirMultiplier, ForceMode.Force);

        if (!IsWallRunning)
            Rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !_exitingSlope)
        {
            if (Rb.velocity.magnitude > _moveSpeed)
                Rb.velocity = Rb.velocity.normalized * _moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                Rb.velocity = new Vector3(limitedVel.x, Rb.velocity.y, limitedVel.z);
            }
        }
        
    }

    private void Jump()
    {
        _exitingSlope = true;

        Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

        Rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(JumpColldown);

        _readyToJump = true;

        _exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight *.5f + .3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < MaxSloapAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

	#endregion
}
