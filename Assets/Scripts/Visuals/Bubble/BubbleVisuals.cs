using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ParticleSystem))]
public class BubbleVisuals : MonoBehaviour
{
    [SerializeField] Bubble bubble;
    ParticleSystem particles;

    [SerializeField] TextMeshPro valueMesh;
    [SerializeField] SpriteRenderer valueRenderer;
    [SerializeField] SpriteRenderer shadowRenderer;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        bubble.ExponentChanged += VisualizeExponent;
    }

    public void VisualizeExponent(Bubble bubble)
    {
        if(bubble.Exponent == 0)
        {
            valueMesh.enabled = false;
            //bubble.Visual.gameObject.SetActive(false);
            valueMesh.enabled = false;
            valueRenderer.enabled = false;
            if(shadowRenderer != null)
                shadowRenderer.enabled = false;
            return;
        }

        //bubble.Visual.gameObject.SetActive(true);
        valueMesh.enabled = true;
        valueRenderer.enabled = true;
        if (shadowRenderer != null)
            shadowRenderer.enabled = true;

        Color bubbleColor = BubbleManager.acc.BubbleColors[bubble.Exponent];
        valueRenderer.color = bubbleColor;
        valueMesh.text = PointParser(bubble.PointValue);

        //var mainModule = particles.main;
        //mainModule.startColor = bubbleColor;

        var colorModule = particles.colorOverLifetime;
        Gradient gradient = new();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(bubbleColor, 0.0f), new GradientColorKey(bubbleColor, 1.0f) },
                         new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
        colorModule.color = gradient;
    }

    public void VisualizeHitPre()
    {
        valueRenderer.enabled = true;
    }

    string PointParser(int pointValue)
    {
        string parsed = pointValue.ToString();
        if(pointValue > 1000)
        {
            parsed = parsed[0] + "K";
        }
        return parsed;
    }
}
