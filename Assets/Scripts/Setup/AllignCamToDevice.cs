using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AllignCamToDevice
{

    public const float defaultAspectRatio = 1.777778f;
    const float defaultCameraSize = 5;
    const float mult = defaultCameraSize / defaultAspectRatio;
    static Camera cam;

    const float MAX_CAM_SIZE = 6.9f;
    const float MIN_CAM_SIZE = 5;

    public static void ScaleCamToDevice()
    {
        cam = Camera.main;
        cam.orthographicSize = Mathf.Clamp(mult * ((float)Screen.height / Screen.width), MIN_CAM_SIZE, MAX_CAM_SIZE);
    }
}
