using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _moveSpeed;
    public float WalkSpeed;
    public float SprintSpeed;
    public float WallRunSpeed;

    public float GroundDrag;

    [Header("Jump")]
    public float JumpForce;
    public float JumpColldown;
    public float AirMultiplier;
    bool _readyToJump;

    [Header("Crouching")]
    public float CrouchSpeed;
    public float CrouchYScale;
    float _startYScale;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask WhatIsGround;

    [Header("Slope Handling")]
    public float MaxSloapAngle;
    RaycastHit _slopeHit;
    bool _exitingSlope;

    public Transform Orientation;

    float _horizontalInput;
    float _verticalInput;

    Vector3 _moveDirection;

    Rigidbody _rb;

    public MovementState State;

    public bool WallRunning;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _readyToJump = true;

        _startYScale = transform.localScale.y;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
        StateHandler();
        Debug.Log(Grounded());

        if (Grounded())
            _rb.drag = GroundDrag;
        else
            _rb.drag = 0f;
    }

    bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, WhatIsGround);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(JumpKey) && _readyToJump && Grounded() && !WallRunning)
        {
            _readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), JumpColldown);
        }

        if (Input.GetKeyDown(CrouchKey) && Grounded())
        {
            transform.localScale = new Vector3(transform.localScale.x, CrouchYScale, transform.localScale.z);
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if(Input.GetKeyUp(CrouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }

    void StateHandler()
    {
        if (WallRunning)
        {
            State = MovementState.WallRunning;
            _moveSpeed = WallRunSpeed;
        }

        if(Input.GetKey(CrouchKey) && Grounded())
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

    void MovePlayer()
    {
        _moveDirection = Orientation.forward * _verticalInput + Orientation.right * _horizontalInput;

        if (OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection(_moveDirection) * _moveSpeed * 20f, ForceMode.Force);

            if (_rb.velocity.y > 0)
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if(Grounded())
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);

        else if (!Grounded())
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * AirMultiplier, ForceMode.Force);

        if(!WallRunning)
            _rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        if (OnSlope() && !_exitingSlope)
        {
            if (_rb.velocity.magnitude > _moveSpeed)
                _rb.velocity = _rb.velocity.normalized * _moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }
        
    }

    void Jump()
    {
        _exitingSlope = true;

        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        _rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        _readyToJump = true;

        _exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight *.5f + .3f))
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
}
