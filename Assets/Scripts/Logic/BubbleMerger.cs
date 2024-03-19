using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMerger : MonoBehaviour
{
    IVisualize<(List<Bubble> chain, Bubble target)> mergeVisual;
    IVisualize<int> feedbackVisual;
    IVisualize<(Bubble, Action)> explodeVisual;

    Coroutine mergeRoutine;

    private void Awake()
    {
        mergeVisual = GetComponent<MergeVisual>();
        explodeVisual = GetComponent<ExplodingVisual>();
        feedbackVisual = GetComponent<CameraShakeVisual>();
    }

    public void InitiateBubbleMerge(Bubble bubble)
    {
        mergeRoutine = StartCoroutine(BubbleMerge(bubble));
    }

    public IEnumerator BubbleMerge(Bubble bubble)
    {
        List<Bubble> bubbleCluster = GetSameBubbleChain(bubble);
        SortChain(bubbleCluster);

        if(bubbleCluster.Count == 1)
        {
            yield break;
        }

        GameData.CurrentCombo = 1;
        while (ComboMerge(bubbleCluster, out Bubble comboBubble))
        {
            yield return StartCoroutine(MergeChainToBubble(bubbleCluster, comboBubble));

            GameData.CurrentCombo++;

            bubbleCluster = GetSameBubbleChain(comboBubble);
            SortChain(bubbleCluster);

        }

        yield return StartCoroutine(MergeChainToBubble(bubbleCluster, bubbleCluster[^1]));
        BubbleManager.acc.FieldStructureUpdate(true);
        yield break;
    }

    List<Bubble> GetSameBubbleChain(Bubble bubble)
    {
        List<Bubble> addToOpen = new() { bubble };
        List<Bubble> chain = new();
        List<Bubble> openBubbles = new();

        chain.Add(bubble);
        openBubbles.AddRange(addToOpen);

        do
        {
            
            addToOpen.Clear();

            for (int i = openBubbles.Count - 1; i >= 0; i--)
            {
                AddMatchingNeighbour(openBubbles[i]);
            }

            openBubbles.Clear();
            openBubbles.AddRange(addToOpen);

        } while (openBubbles.Count > 0);

        return chain;

        void AddMatchingNeighbour(Bubble bubble)
        {
            for (int i = 0; i < bubble.Neighbours.Count; i++)
            {
                if (chain.Contains(bubble.Neighbours[i]) || bubble.PointValue != bubble.Neighbours[i].PointValue)
                    continue;

                addToOpen.Add(bubble.Neighbours[i]);
                chain.Add(bubble.Neighbours[i]);
            }
        }
    }

    bool ComboMerge(List<Bubble> chain, out Bubble comboTarget)
    {
        int chainExponent = chain[0].Exponent;

        for (int i = chain.Count - 1; i >= 0; i--)
        {
            foreach (Bubble neighbour in chain[i].Neighbours)
            {
                if (neighbour.Exponent == chainExponent + chain.Count - 1)
                {
                    comboTarget = chain[i];
                    return true;
                }
            }
        }

        comboTarget = null;
        return false;
    }

    

    IEnumerator MergeChainToBubble(List<Bubble> chain, Bubble target)
    {
        (List<Bubble> chain, Bubble target) mergeComponents =
            (
            chain,
            target
            );

        yield return StartCoroutine(mergeVisual.Visualize(mergeComponents));
        int exponent = target.Exponent;

        foreach (Bubble bubble in chain)
        {
            bubble.Exponent = 0;
        }

        target.Exponent = exponent + chain.Count - 1;

        GameData.Score += target.PointValue;
        StartCoroutine(feedbackVisual.Visualize(target.PointValue));
        if (target.Exponent == Bubble.MAX_EXPONENT)
        {
            yield return explodeVisual.Visualize((target,
                () =>
                {
                    ExplosionLogic(target);
                    if (mergeRoutine != null)
                        StopCoroutine(mergeRoutine);
                }));
            
        }

    }

    // Sorts Bubbles from Highest To Lowest Row Index
    void SortChain(List<Bubble> chain)
    {
        List<Bubble> bubbles = chain;

        for (int i = 0; i < bubbles.Count - 1; i++)
        {
            for (int j = 0; j < bubbles.Count - i - 1; j++)
            {
                if (bubbles[j].Row.Index < bubbles[j + 1].Row.Index)
                {
                    Bubble temp = bubbles[j];
                    bubbles[j] = bubbles[j + 1];
                    bubbles[j + 1] = temp;
                }
            }
        }
    }

    void ExplosionLogic(Bubble bubble)
    {
        bubble.Exponent = 0;
        foreach (Bubble neighbour in bubble.Neighbours)
        {
            if (!neighbour.IsActive)
                continue;
            neighbour.Exponent = 0;
        }
        BubbleManager.acc.FieldStructureUpdate(true);
    }
}
