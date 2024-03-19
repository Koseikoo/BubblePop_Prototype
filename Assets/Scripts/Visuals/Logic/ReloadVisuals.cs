using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadVisuals : MonoBehaviour, IVisualize<(Action shooter, Action reserve)>
{
    [SerializeField] float reloadSpeed;
    [SerializeField] Transform shootingBubbleVisual;
    [SerializeField] AnimationCurve reloadCurve;

    public IEnumerator Visualize((Action shooter, Action reserve) setActions)
    {
        setActions.shooter();
        setActions.reserve();
        shootingBubbleVisual.position = BubbleManager.acc.ReserveBubble.transform.position;

        shootingBubbleVisual.gameObject.SetActive(true);
        float timer = 0;
        Vector3 localStartPosition = shootingBubbleVisual.transform.localPosition;

        while (timer < reloadSpeed)
        {
            timer += Time.deltaTime;
            float t = timer / reloadSpeed;
            t = reloadCurve.Evaluate(t);
            shootingBubbleVisual.localPosition = Vector3.Lerp(localStartPosition, Vector3.zero, t);
            yield return null;
        }
       
    }
}
