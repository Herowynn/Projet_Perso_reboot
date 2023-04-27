using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrunning : MonoBehaviour
{
    [Header("Wall Run")]
    public LayerMask Wall;
    public float WallRunForce;
    public float WallJumpUpForce;
    public float WallJumpSideForce;
    public float WallClimbSpeed;
    public float MaxWallRunTime;
    float _wallRunTimer;
    bool _canReWallRun;

    [Header("Input")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode UpwardsRunKey = KeyCode.LeftShift;
    public KeyCode DownwardsRunKey = KeyCode.LeftAlt;
    bool _upwardsRunning;
    bool _downwardsRunning;
    float _horizontalInput;
    float _verticalInput;

    [Header("Detection")]
    public float WallCheckDistance;
    RaycastHit _leftWallHit;
    RaycastHit _rightWallHit;
    bool _wallLeft;
    bool _wallRight;

    [Header("Exiting")]
    public float ExitWallTime;
    float _exitWallTimer;
    bool _exitingWall;

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
        _canReWallRun = true;
        _pm = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckForWalls();
        StateMachine();
        if (GetComponentInParent<PlayerMovement>().Grounded())
            _canReWallRun = true;
    }

    private void FixedUpdate()
    {
        if (_pm.IsWallRunning)
            WallRunningMovement();
    }

    void CheckForWalls()
    {
        _wallRight = Physics.Raycast(transform.position, Orientation.right, out _rightWallHit, WallCheckDistance, Wall);
        _wallLeft = Physics.Raycast(transform.position, -Orientation.right, out _leftWallHit, WallCheckDistance, Wall);
    }

    void StateMachine()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        _upwardsRunning = Input.GetKey(UpwardsRunKey);
        _downwardsRunning = Input.GetKey(DownwardsRunKey);

        if((_wallLeft || _wallRight) && _verticalInput > 0 && !GetComponentInParent<PlayerMovement>().Grounded() && !_exitingWall && _canReWallRun)
        {
            if(!_pm.IsWallRunning)
                StartWallRun();

            if (_wallRunTimer > 0)
                _wallRunTimer -= Time.deltaTime;

            if(_wallRunTimer <= 0 && _pm.IsWallRunning)
            {
                _exitingWall = true;
                _exitWallTimer = ExitWallTime;
                _canReWallRun = false;
            }


            if (Input.GetKey(JumpKey))
                WallJump();
        }
        
        else if (_exitingWall)
        {
            if(_pm.IsWallRunning)
                StopWallRun();

            if (_exitWallTimer > 0)
                _exitWallTimer -= Time.deltaTime;

            if (_exitWallTimer <= 0)
                _exitingWall = false;
        }

        else
        {
            if(_pm.IsWallRunning)
                StopWallRun();
        }
    }

    void StartWallRun()
    {
        _pm.IsWallRunning = true;

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
        _pm.IsWallRunning = false;

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
