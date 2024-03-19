using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public bool GetActiveTouch(out Touch activeTouch)
    {
        if (Input.touchCount == 0 || EventSystem.current.IsPointerOverGameObject())
        {
            activeTouch = default;
            return false;
        }

        activeTouch = Input.GetTouch(0);
        return true;

    }
}
