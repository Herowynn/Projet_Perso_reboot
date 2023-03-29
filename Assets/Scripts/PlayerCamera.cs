using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float SensX;
    public float SensY;

    public Transform Orientation;

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

        transform.rotation = Quaternion.Euler(_xRot, _yRot, 0f);
        Orientation.rotation = Quaternion.Euler(0f, _yRot, 0f);

    }
}
