using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Camera Position")]
    public Transform CameraPosition;

    private void Update()
    {
        transform.position = CameraPosition.position;
    }
}
