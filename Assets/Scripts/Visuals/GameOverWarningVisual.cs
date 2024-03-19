using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverWarningVisual : MonoBehaviour, IEvent<BubbleRow>
{
    [SerializeField] float maxScale;
    [SerializeField] float minScale;

    [SerializeField] float warningSpeed;
    [SerializeField] Color warningColor;

    Coroutine warningRoutine;

    public void TriggerEvent(BubbleRow row)
    {
        if(warningRoutine != null)
            StopCoroutine(warningRoutine);
        warningRoutine = StartCoroutine(WarningLerp(row, GetRowRenderer(row)));
    }

    IEnumerator WarningLerp(BubbleRow row, SpriteRenderer[] renderer)
    {
        Color[] baseColors = new Color[renderer.Length];
        for (int i = 0; i < renderer.Length; i++)
        {
            baseColors[i] = renderer[i].color;
        }

        while (true)
        {
            float t = Mathf.Sin(Time.time * warningSpeed) *.5f + .5f;

            for (int i = 0; i < row.Bubbles.Length; i++)
            {
                row.Bubbles[i].Visual.localScale = Mathf.Lerp(minScale, maxScale, t) * Vector3.one;
                renderer[i].color = Color.Lerp(baseColors[i], warningColor, t);
            }
            yield return null;
        }
    }

    SpriteRenderer[] GetRowRenderer(BubbleRow row)
    {
        SpriteRenderer[] renderer = new SpriteRenderer[row.Bubbles.Length];
        for (int i = 0; i < row.Bubbles.Length; i++)
        {
            renderer[i] = row.Bubbles[i].Visual.GetComponent<SpriteRenderer>();
        }
        return renderer;
    }
}
