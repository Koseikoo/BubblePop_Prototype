using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAimVisuals : MonoBehaviour
{
    [SerializeField] Bubble bubble;

    [SerializeField] SpriteRenderer renderer;
    [SerializeField] float aimedAlpha;

    private void Awake()
    {
        bubble.IsAimTargetChanged += BubbleShotPreVisual;
    }

    public void BubbleShotPreVisual(Bubble bubble)
    {
        renderer.enabled = bubble.IsAimTarget;
        Color color = BubbleManager.acc.BubbleColors[BubbleManager.acc.ShootBubble.Exponent];
        color.a = aimedAlpha;
        renderer.color = color;


    }

}
