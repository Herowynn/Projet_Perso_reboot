using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
	#region Variables

	[Header("References")]
	private PlayerMovement _pm;
	private Transform _orientation;
	public Transform Cam;
	private Rigidbody _rb;

	[Header("Ledge Grabbing")]
	public float MoveToLedgeSpeed;
	public float MaxLedgeGrabDistance;

	[Header("Timer")]
	public float MinTimeOnLedge;
	private float _timeOnLedge;

	[Header("Ledge jumping")]
	public KeyCode JumpKey = KeyCode.Space;
	public float LedgeJumpForwardForce;
	public float LedgeJumpUpwardForce;

	[Header("Leadge Detection")]
	public float LedgeDetectionLength;
	public float LedgeSphereCastRadius;
	public LayerMask WhatIsLedge;
	private Transform _lastLedge;
	private Transform _currentLedge;
	private RaycastHit _ledgeHit;

	[Header("Exiting")]
	public bool IsExitingLedge;
	public float ExitLedgeTime;
	private float _exitLedgeTimer;

	public bool IsHolding;

	#endregion

	private void Start()
	{
		_pm = GetComponent<PlayerMovement>();
		_orientation = _pm.Orientation;
		_rb = _pm.Rb;
	}

	private void Update()
	{
		LedgeDetection();
		SubStateMachine();
	}

	private void SubStateMachine()
	{
		if(IsHolding)
		{
			FreezeRigidBodyOnLedge();

			_timeOnLedge += Time.deltaTime;

			if (Input.GetKeyDown(JumpKey))
				LedgeJump();
		}

		else if (IsExitingLedge)
		{
			if (_exitLedgeTimer > 0)
				_exitLedgeTimer -= Time.deltaTime;

			else
				IsExitingLedge = false;
		}
	}

	private void LedgeDetection()
	{
		bool ledgeDetected = Physics.SphereCast(transform.position, LedgeSphereCastRadius, Cam.forward, out _ledgeHit, LedgeDetectionLength, WhatIsLedge);

		if (!ledgeDetected)
			return;

		if (_ledgeHit.transform == _lastLedge) 
			return;

		if (_ledgeHit.distance < MaxLedgeGrabDistance && !IsHolding)
			EnterLedgeHold();
	}

	private void LedgeJump()
	{
		ExitLedgeHold();

		StartCoroutine(DelayedJumpForce());
	}

	private IEnumerator DelayedJumpForce()
	{
		yield return new WaitForSeconds(0.05f);

		Vector3 forceToAdd = Cam.forward * LedgeJumpForwardForce + _orientation.up * LedgeJumpUpwardForce;
		_rb.velocity = Vector3.zero;
		_rb.AddForce(forceToAdd, ForceMode.Impulse);
	}

	private void EnterLedgeHold()
	{
		IsExitingLedge = true;
		_exitLedgeTimer = ExitLedgeTime;

		IsHolding = true;

		_currentLedge = _ledgeHit.transform;
		_lastLedge = _ledgeHit.transform;

		_rb.useGravity = false;
		_rb.velocity = Vector3.zero;
	}

	private void FreezeRigidBodyOnLedge()
	{
		_rb.useGravity = false;

		Vector3 directionToLedge = _currentLedge.position - transform.position;
		float distanceToLedge = Vector3.Distance(transform.position, _currentLedge.position);

		if(distanceToLedge < 1f)
		{
			if(_rb.velocity.magnitude < MoveToLedgeSpeed)
				_rb.AddForce(directionToLedge.normalized * MoveToLedgeSpeed * 1000f * Time.deltaTime);
		}

		else
		{
			if (!_pm.IsFrozen) 
				_pm.IsFrozen = true;
		}

		if(distanceToLedge > MaxLedgeGrabDistance)
			ExitLedgeHold();
	}

	private void ExitLedgeHold()
	{
		IsHolding = false;
		_timeOnLedge = 0f;

		_pm.IsFrozen = false;

		_rb.useGravity = true;

		StopAllCoroutines();

		StartCoroutine(ResetLastLedge());
	}

	private IEnumerator ResetLastLedge()
	{
		yield return new WaitForSeconds(1f);

		_lastLedge = null;
	}
}
