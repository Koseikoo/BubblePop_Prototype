using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowPushVisual : MonoBehaviour, IVisualize<Bubble[]>
{
    [SerializeField] float pushTime;
    [SerializeField] AnimationCurve pushCurve;

    public IEnumerator Visualize(Bubble[] rowBubbles)
    {
        float timer = 0;

        float startY = rowBubbles[0].transform.position.y;

        (Vector3 start, Vector3 end)[] lerpPositions = GetBubblePositions();

        while (timer < pushTime)
        {
            timer += Time.deltaTime;
            float t = pushCurve.Evaluate(timer / pushTime);

            for (int i = 0; i < rowBubbles.Length; i++)
            {
                rowBubbles[i].transform.position = Vector3.Lerp(lerpPositions[i].start, lerpPositions[i].end, t);
            }

            yield return null;
        }

        (Vector3, Vector3)[] GetBubblePositions()
        {
            (Vector3, Vector3)[] lerpPositions = new (Vector3, Vector3)[rowBubbles.Length];

            for (int i = 0; i < lerpPositions.Length; i++)
            {
                lerpPositions[i] = (rowBubbles[i].transform.position, rowBubbles[i].transform.position +
                    (BubbleSpawner.BUBBLE_HEIGHT_MULT * Vector3.down));
            }

            return lerpPositions;
        }
    }

    
}
