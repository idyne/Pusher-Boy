using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;

    }

    public void AdjustZoom(float zoom)
    {
        transform.position = initialPosition + transform.forward * zoom;
    }
}
