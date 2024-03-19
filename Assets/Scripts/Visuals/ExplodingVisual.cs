using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplodingVisual : MonoBehaviour, IVisualize<(Bubble bubble, Action action)>
{
    [SerializeField] float minScale;
    [SerializeField] float maxScale;

    [Header("Expansion")]
    [SerializeField] AnimationCurve expansionCurve;
    [SerializeField] float expansionDuration;


    [Header("Contraction")]
    [SerializeField] AnimationCurve contractionCurve;
    [SerializeField] float contractionDuration;
    [Space]
    [SerializeField] float pullForce;

    [Header("Explosion")]
    [SerializeField] AnimationCurve explosionCurve;
    [SerializeField] float explosionDuration;
    [SerializeField] float explodeForce;
    [SerializeField] float gravity;

    Bubble centerBubble;
    List<Vector3> startPositions = new();
    float startScale;

    public IEnumerator Visualize((Bubble bubble, Action action)info)
    {
        startScale = info.bubble.Visual.localScale.x;
        centerBubble = info.bubble;
        startPositions.Clear();
        List<Bubble> neighbours = GetNeighbours(info.bubble);
        Vector3[] pullPositions = GetPullPositions(neighbours);
        Vector3[] explosionVelocities = GetExplosionVelocities(neighbours);

        yield return StartCoroutine(Expansion());
        yield return StartCoroutine(Contraction(neighbours, pullPositions));
        centerBubble.ExplosionParticles.Play(false);
        yield return StartCoroutine(Explosion(neighbours, explosionVelocities));
        info.action?.Invoke();
        yield return new WaitForFixedUpdate();
        ResetBubbles(neighbours);


        // scale up

        // scale down and pull adj active bubbles

        // scale up, explode, push active adj bubbles away

        // explode adj bubbles after x seconds

        yield break;

        
    }

    IEnumerator Expansion()
    {
        float timer = 0;
        float t;
        float scaler;

        while (timer < expansionDuration)
        {
            timer += Time.deltaTime;
            t = expansionCurve.Evaluate(timer / expansionDuration);
            scaler = Mathf.Lerp(startScale, maxScale, t);
            centerBubble.Visual.localScale = scaler * Vector3.one;

            yield return null;
        }
    }

    IEnumerator Contraction(List<Bubble> bubbles, Vector3[] pullPositions)
    {
        float timer = 0;
        float t;
        float scaler;

        while (timer < contractionDuration)
        {
            timer += Time.deltaTime;
            t = contractionCurve.Evaluate(timer / contractionDuration);
            scaler = Mathf.Lerp(0, maxScale, t);
            centerBubble.Visual.localScale = scaler * Vector3.one;

            for (int i = 0; i < bubbles.Count; i++)
            {
                bubbles[i].transform.position = Vector3.Lerp(pullPositions[i], bubbles[i].transform.position, t);
            }

            yield return null;
        }
    }

    IEnumerator Explosion(List<Bubble> bubbles, Vector3[] velocities)
    {
        float timer = 0;


        while (timer < explosionDuration)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < bubbles.Count; i++)
            {
                bubbles[i].transform.position += velocities[i] * Time.deltaTime;
                velocities[i] += gravity * Time.deltaTime * Vector3.down;
            }

            yield return null;
        }

        for (int i = 0; i < bubbles.Count; i++)
        {
            bubbles[i].Particles.Play(false);
        }
    }

    void ResetBubbles(List<Bubble> bubbles)
    {
        for (int i = 0; i < bubbles.Count; i++)
        {
            bubbles[i].transform.position = startPositions[i];
        }

        centerBubble.Visual.localScale = startScale * Vector3.one;
    }

    List<Bubble> GetNeighbours(Bubble bubble)
    {
        List<Bubble> active = new();

        for (int i = 0; i < bubble.Neighbours.Count; i++)
        {
            if (!bubble.Neighbours[i].IsActive)
                continue;

            active.Add(bubble.Neighbours[i]);
            startPositions.Add(bubble.Neighbours[i].transform.position);
        }
        return active;
    }

    Vector3[] GetPullPositions(List<Bubble> bubbles)
    {
        Vector3[] pullPositions = new Vector3[bubbles.Count];
        for (int i = 0; i < bubbles.Count; i++)
        {
            pullPositions[i] = pullForce * (centerBubble.transform.position - bubbles[i].transform.position).normalized;
            pullPositions[i] += bubbles[i].transform.position;
        }
        return pullPositions;
    }

    Vector3[] GetExplosionVelocities(List<Bubble> bubbles)
    {
        Vector3[] velocity = new Vector3[bubbles.Count];
        for (int i = 0; i < bubbles.Count; i++)
        {
            velocity[i] = explodeForce * (bubbles[i].transform.position - centerBubble.transform.position).normalized;
        }
        return velocity;
    }
}
