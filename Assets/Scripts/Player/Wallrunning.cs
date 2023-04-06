using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrunning : MonoBehaviour
{
    [Header("wallrunning")]
    public LayerMask Wall;
    public LayerMask Ground;
    public float WallRunForce;
    public float WallJumpUpForce;
    public float WallJumpSideForce;
    public float WallClimbSpeed;
    public float MaxWallRunTime;
    float _wallRunTimer;

    [Header("Input")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode UpwardsRunKey = KeyCode.LeftShift;
    public KeyCode DownwardsRunKey = KeyCode.LeftControl;
    bool _upwardsRunning;
    bool _downwardsRunning;
    float _horizontalInput;
    float _verticalInput;

    [Header("Detection")]
    public float WallCheckDistance;
    public float MinJumpHeight;
    RaycastHit _leftWallHit;
    RaycastHit _rightWallHit;
    bool _wallLeft;
    bool _wallRight;

    [Header("Exiting")]
    bool _exitingWall;
    public float ExitWallTime;
    float _exitWallTimer;

    [Header("Gravity")]
    public bool UseGravity;
    public float GravityCounterForce;

    [Header("References")]
    public Transform Orientation;
    public PlayerCamera Cam;
    PlayerMovement _pm;
    Rigidbody _rb;

    private void Start()
    {
        _pm = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckForWalls();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (_pm.WallRunning)
            WallRunningMovement();
    }

    void CheckForWalls()
    {
        _wallRight = Physics.Raycast(transform.position, Orientation.right, out _rightWallHit, WallCheckDistance, Wall);
        _wallLeft = Physics.Raycast(transform.position, -Orientation.right, out _leftWallHit, WallCheckDistance, Wall);
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, MinJumpHeight, Ground);
    }

    void StateMachine()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        _upwardsRunning = Input.GetKey(UpwardsRunKey);
        _downwardsRunning = Input.GetKey(DownwardsRunKey);

        if((_wallLeft || _wallRight) && _verticalInput > 0 && AboveGround() && !_exitingWall)
        {
            if(!_pm.WallRunning)
                StartWallRun();

            if (_wallRunTimer > 0)
                _wallRunTimer -= Time.deltaTime;

            if(_wallRunTimer <= 0 && _pm.WallRunning)
            {
                _exitingWall = true;
                _exitWallTimer = ExitWallTime;
            }


            if (Input.GetKey(JumpKey))
                WallJump();
        }
        
        else if (_exitingWall)
        {
            if(_pm.WallRunning)
                StopWallRun();

            if (_exitWallTimer > 0)
                _exitWallTimer -= Time.deltaTime;

            if (_exitWallTimer <= 0)
                _exitingWall = false;
        }

        else
        {
            if(_pm.WallRunning)
                StopWallRun();
        }
    }

    void StartWallRun()
    {
        _pm.WallRunning = true;

        _wallRunTimer = MaxWallRunTime;

        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        Cam.DoFov(90f);

        if (_wallLeft)
            Cam.DoTilt(-5f);

        if (_wallRight)
            Cam.DoTilt(5f);

    }

    void WallRunningMovement()
    {
        _rb.useGravity = UseGravity;

        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((Orientation.forward - wallForward).magnitude > (Orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        _rb.AddForce(wallForward * WallRunForce, ForceMode.Force);

        if (_upwardsRunning)
            _rb.velocity = new Vector3(_rb.velocity.x, WallClimbSpeed, _rb.velocity.z);

        if (_downwardsRunning)
            _rb.velocity = new Vector3(_rb.velocity.x, -WallClimbSpeed, _rb.velocity.z);

        if(!(_wallLeft && _horizontalInput > 0) && !(_wallRight && _horizontalInput < 0))
            _rb.AddForce(-wallNormal * 100f, ForceMode.Force);

        if(UseGravity)
            _rb.AddForce(transform.up * GravityCounterForce, ForceMode.Force);
    }

    void StopWallRun()
    {
        _pm.WallRunning = false;

        Cam.DoFov(80f);
        Cam.DoTilt(0f);
    }

    void WallJump()
    {
        _exitingWall = true;
        _exitWallTimer = ExitWallTime;

        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;

        Vector3 forceToApply = transform.up * WallJumpUpForce + wallNormal * WallJumpSideForce;

        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        _rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}