using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldImpactReaction : MonoBehaviour, IEvent<int>
{
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] float lerpDuration;
    [SerializeField] AnimationCurve lerpCurve;

    //[SerializeField] float decayIntensity;


    Color color;
    float currentAlpha;

    Coroutine Lerp;
    Coroutine Decay;

    private void Awake()
    {
        color = renderer.color;
    }

    public void TriggerEvent(int shotBubbleExponent)
    {
        float alpha = (float)shotBubbleExponent / Bubble.MAX_EXPONENT;
        if (Decay != null)
            StopCoroutine(Decay);
        if (Lerp != null)
            StopCoroutine(Lerp);

        Lerp = StartCoroutine(AlphaLerp(alpha));
        
        
    }

    IEnumerator AlphaLerp(float targetAlpha)
    {
        float timer = 0;
        float startAlpha = currentAlpha;
        while (timer < lerpDuration)
        {
            timer += Time.deltaTime;
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, lerpCurve.Evaluate(timer / lerpDuration));
            color.a = currentAlpha;
            renderer.color = color;
            yield return null;
        }

        Decay = StartCoroutine(AlphaDecay());
    }

    IEnumerator AlphaDecay()
    {
        float timer = 0;
        float startAlpha = currentAlpha;
        while (timer < lerpDuration)
        {
            timer += Time.deltaTime;
            currentAlpha = Mathf.Lerp(startAlpha, 0, lerpCurve.Evaluate(timer / lerpDuration));
            color.a = currentAlpha;
            renderer.color = color;
            yield return null;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
