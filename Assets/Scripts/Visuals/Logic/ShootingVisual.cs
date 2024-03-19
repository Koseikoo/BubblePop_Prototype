using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingVisual : MonoBehaviour, IVisualize<List<Vector2>>
{
    [SerializeField] float bubbleSpeed;
    [SerializeField] TrailRenderer trailRenderer;

    public IEnumerator Visualize(List<Vector2> newValue)
    {
        Transform visualTransform = BubbleManager.acc.ShootBubble.Visual;

        Vector2 startPosition;
        Vector2 endPosition;

        float t;
        int i;
        float distance;
        float timer;
        float shootTime;


        for (i = 0; i < newValue.Count - 1; i++)
        {
            startPosition = newValue[i];
            endPosition = newValue[i + 1];

            distance = Vector2.Distance(startPosition, endPosition);
            timer = 0;
            shootTime = distance * (1 / bubbleSpeed);

            while (timer < shootTime)
            {
                timer += Time.deltaTime;
                t = timer / shootTime;

                visualTransform.position = Vector2.Lerp(startPosition, endPosition, t);
                yield return null;
            }
        }
        BubbleManager.acc.ShootBubble.Visual.gameObject.SetActive(false);
        trailRenderer.ResetBounds();
        trailRenderer.ResetLocalBounds();
        visualTransform.position = BubbleManager.acc.ReserveBubble.transform.position;
    }

}
