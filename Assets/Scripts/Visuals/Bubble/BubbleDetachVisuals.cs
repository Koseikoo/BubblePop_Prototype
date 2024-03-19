using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDetachVisuals : MonoBehaviour, IVisualize<(Bubble bubble, Action action)>
{
    [SerializeField] Bubble bubble;
    [Space]
    [SerializeField] float gravityForce = 9.81f;
    const float explodeHeightY = -2.5f;
    [SerializeField] AnimationCurve driftCurve;
    [SerializeField] float driftIntensity;

    [SerializeField] Vector3 velocity;
    Vector3 startPos;
    Transform bubbleTransform;

    private void Awake()
    {
        bubble.fallingVisual = this;
    }


    public IEnumerator Visualize((Bubble bubble, Action action) info)
    {
        bubbleTransform = info.bubble.Visual;
        startPos = bubbleTransform.localPosition;
        velocity = Vector3.up + (driftIntensity * startPos.x * Vector3.left);
        velocity += (Vector3)UnityEngine.Random.insideUnitCircle;
        velocity.Normalize();

        while (bubbleTransform.position.y > explodeHeightY)
        {
            bubbleTransform.position += Time.fixedDeltaTime * velocity;
            velocity += gravityForce * Time.fixedDeltaTime * Vector3.down;
            yield return new WaitForFixedUpdate();
        }

        info.bubble.Particles.Play(false);
        info.action?.Invoke();
        yield return new WaitForFixedUpdate();
        bubbleTransform.localPosition = Vector3.zero;
    }
}
