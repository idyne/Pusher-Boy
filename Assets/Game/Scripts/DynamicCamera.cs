using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [SerializeField] private Zoom cameraZoom;
    public void AdjustCameraZoom()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float zoom = (-7f / 3) * (Level.Instance.Board.BoardWidth - 10);
        zoom += screenRatio * 20 - 10;
        cameraZoom.AdjustZoom(zoom);
    }
}
