using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    public float SensX;
    public float SensY;

    public Transform Orientation;
    public Transform CamHolder;

    float _xRot;
    float _yRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * SensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * SensY;

        _yRot += mouseX;

        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, _yRot - 90f, 90f);

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
