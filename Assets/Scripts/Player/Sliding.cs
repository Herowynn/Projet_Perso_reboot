using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
	#region Variables

	[Header("References")]
    public Transform Orientation;
    public Transform PlayerObj;
	private Rigidbody _rb;
	private PlayerMovement _pm;

    [Header("Sliding")]
    public float MaxSlideTime;
    public float SlideForce;
	private float _slideTimer;
    private bool _sliding;

    [Header("Y Sliding Scale")]
    public float SlideYScale;
    private float _startYScale;

    [Header("Input")]
    public KeyCode SlideKey = KeyCode.LeftControl;
    private float _horizontalInput;
    private float _verticalInput;

	#endregion

	#region Default Functions

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

        if (Input.GetKeyDown(SlideKey) && Input.GetKey(GetComponentInParent<PlayerMovement>().SprintKey) && (_horizontalInput != 0 || _verticalInput != 0) && GetComponentInParent<PlayerMovement>().Grounded())
            StartSlide();

        else if (Input.GetKeyUp(SlideKey) && _sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (_sliding)
            SlidingMovement();
    }

	#endregion

	#region My Functions

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
            _rb.AddForce(_pm.GetSlopeMoveDirection(inputDirection) * SlideForce, ForceMode.Force);

        if(_slideTimer <= 0)
            StopSlide();
    }

    void StopSlide()
    {
        _sliding = false;

        PlayerObj.localScale = new Vector3(PlayerObj.localScale.x, _startYScale, PlayerObj.localScale.z);
    }

	#endregion
}
