using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToPlayVisual : MonoBehaviour
{
    [SerializeField] RectTransform tapTextRect;
    [Space]
    [SerializeField] Vector2 moveOffset;
    Vector2 startPosition;
    Vector2 endPosition;
    [SerializeField] float endScale;
    [SerializeField] float speed;

    public Coroutine TapRoutine;

    public IEnumerator TapToPlayRoutine()
    {
        startPosition = tapTextRect.anchoredPosition;
        endPosition = tapTextRect.anchoredPosition + moveOffset;

        while (true)
        {
            float t = Mathf.Sin(Time.time * speed) * .5f + .5f;

            tapTextRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            tapTextRect.localScale = Mathf.Lerp(1, endScale, t) * Vector3.one;

            yield return null;
        }
    }
}
