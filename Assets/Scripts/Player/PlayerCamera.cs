using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [SerializeField] private float _sensX;
	[SerializeField] private float _sensY;

    [Header("References")]
    public Transform Orientation;
    public Transform CamHolder;

    [Header("Rotations")]
    float _xRot;
    float _yRot;

    private void Start()
    {
        _sensX = PlayerPrefs.GetFloat("HorizontalSensitivity") * 100;
        _sensY = PlayerPrefs.GetFloat("VerticalSensitivity") * 100;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
		float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _sensX;
		float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _sensY;

		_yRot += mouseX;

		_xRot -= mouseY;
		_xRot = Mathf.Clamp(_xRot, - 45f, 45f);

		CamHolder.rotation = Quaternion.Euler(_xRot, _yRot, 0f);
		Orientation.rotation = Quaternion.Euler(0f, _yRot, 0f);
	}

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, .25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0f, 0f, zTilt), .25f);
    }
}
