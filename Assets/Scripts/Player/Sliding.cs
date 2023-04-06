using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform Orientation;
    public Transform PlayerObj;
    Rigidbody _rb;
    PlayerMovement _pm;

    [Header("Sliding")]
    public float MaxSlideTime;
    public float SlideForce;
    float _slideTimer;
    bool _sliding;

    public float SlideYScale;
    float _startYScale;

    [Header("Input")]
    public KeyCode SlideKey = KeyCode.LeftControl;
    float _horizontalInput;
    float _verticalInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _pm = GetComponent<PlayerMovement>();

        _startYScale = transform.localScale.y;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(SlideKey) && (_horizontalInput != 0 || _verticalInput != 0))
            StartSlide();

        if (Input.GetKeyUp(SlideKey) && _sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (_sliding)
            SlidingMovement();
    }

    void StartSlide()
    {
        _sliding = true;

        PlayerObj.localScale = new Vector3(PlayerObj.localScale.x, SlideYScale, PlayerObj.localScale.z);
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = MaxSlideTime;
    }

    void SlidingMovement()
    {
        Vector3 inputDirection = Orientation.forward * _verticalInput + Orientation.right * _horizontalInput;

        if(!_pm.OnSlope() || _rb.velocity.y > -0.1f)
        {
            _rb.AddForce(inputDirection.normalized * SlideForce, ForceMode.Force);

            _slideTimer -= Time.deltaTime;
        }

        else
        {
            _rb.AddForce(_pm.GetSlopeMoveDirection(inputDirection) * SlideForce, ForceMode.Force);
        }

        if(_slideTimer <= 0)
            StopSlide();
    }

    void StopSlide()
    {
        _sliding = false;

        PlayerObj.localScale = new Vector3(PlayerObj.localScale.x, _startYScale, PlayerObj.localScale.z);
    }
}
