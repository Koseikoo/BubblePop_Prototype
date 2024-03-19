using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowTimerVisual : MonoBehaviour
{
    float maxWidth = 600;
    [SerializeField] RectTransform timerVisual;

    float t;
    float width;

    private void Awake()
    {
        GameData.RowPushTimerChanged += UpdateRowPushTimerVisual;
    }

    void UpdateRowPushTimerVisual()
    {
        t = 1 - (GameData.RowPushTimer / GameData.RowPushInverval);
        width = maxWidth * t;

        timerVisual.sizeDelta = new(width, timerVisual.sizeDelta.y);
    }
}
