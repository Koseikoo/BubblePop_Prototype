using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepImpactVisual : MonoBehaviour, IVisualize<Bubble>
{
    [SerializeField] int impactLayer;
    [SerializeField] float impactForce;
    [SerializeField] float impactForceFallOff;

    [SerializeField] float impactDuration;
    [SerializeField] AnimationCurve impactCurve;

    List<Bubble>[] bubbleLayer;
    Bubble impactBubble;

    private void Start()
    {
        bubbleLayer = new List<Bubble>[impactLayer];
        for (int i = 0; i < bubbleLayer.Length; i++)
            bubbleLayer[i] = new();
    }

    public IEnumerator Visualize(Bubble impactBubble)
    {
        this.impactBubble = impactBubble;
        yield return StartCoroutine(SetImpactLayer(impactBubble));

        for (int i = 0; i < bubbleLayer.Length; i++)
        {
            float intensity = impactForce - (i * impactForceFallOff);
            yield return StartCoroutine(VisualizeImpact(bubbleLayer[i], intensity));
        }
    }

    IEnumerator SetImpactLayer(Bubble centerBubble)
    {
        List<Bubble> closed = new();
        List<Bubble> curLayer = new();
        List<Bubble> open = new() { centerBubble };

        for (int i = 0; i < bubbleLayer.Length; i++)
        {
            for (int j = 0; j < open.Count; j++)
            {
                closed.Add(open[j]);
                foreach (Bubble bubble in open[j].Neighbours)
                {
                    if (closed.Contains(bubble) || curLayer.Contains(bubble) || open.Contains(bubble) || !bubble.IsActive)
                        continue;

                    curLayer.Add(bubble);
                    closed.Add(bubble);
                }
            }

            open = new(curLayer);
            bubbleLayer[i] = new(curLayer);
            curLayer.Clear();
        }

        yield break;
    }

    IEnumerator VisualizeImpact(List<Bubble> impactLayer, float intensity)
    {
        float timer = 0;
        float duration = intensity * impactDuration;

        List<(Vector3 start, Vector3 end)> impactInfo = GetImpactTuples(impactLayer);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = impactCurve.Evaluate(timer / duration);

            for (int i = 0; i < impactInfo.Count; i++)
            {
                impactLayer[i].Visual.position = Vector3.Lerp(impactInfo[i].start, impactInfo[i].end, t);
            }

            yield return null;
        }

        List<(Vector3 start, Vector3 end)> GetImpactTuples(List<Bubble> bubbles)
        {
            List<(Vector3, Vector3)> impactInfo = new();

            foreach (Bubble bubble in bubbles)
            {
                if (!bubble.IsActive)
                    continue;
                Vector3 direction = intensity * (bubble.transform.position - impactBubble.transform.position).normalized;

                impactInfo.Add((bubble.transform.position, bubble.transform.position + direction));
            }

            return impactInfo;

        }
    }


}
