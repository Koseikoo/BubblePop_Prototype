using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowFadeInVisual : MonoBehaviour, IVisualize<(BubbleRow row, Action<BubbleRow> setAction)>
{
    [SerializeField] float fadeInTime;
    [SerializeField] AnimationCurve fadeInCurve;
    Vector3 _bubbleScale;
    float _timer;

    private void Start()
    {
        _bubbleScale = BubbleSpawner.BUBBLE_SCALE * Vector3.one;
    }

    public IEnumerator Visualize((BubbleRow row, Action<BubbleRow> setAction) info)
    {
        _timer = 0;
        foreach (Bubble bubble in info.row.Bubbles)
        {
            bubble.transform.localScale = Vector3.zero;
        }

        info.setAction(info.row);

        while (_timer < fadeInTime)
        {
            _timer += Time.deltaTime;
            float t = fadeInCurve.Evaluate(_timer / fadeInTime);

            foreach (Bubble bubble in info.row.Bubbles)
            {
                bubble.transform.localScale = Vector3.Lerp(Vector3.zero, _bubbleScale, t);
            }

            yield return null;
        }

    }
}
