using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;

using UnityEngine;

public class MergeVisual : MonoBehaviour, IVisualize<(List<Bubble> chain, Bubble target)>
{
    [SerializeField] float mergeDelay;
    [SerializeField] float mergeTime;
    [SerializeField] AnimationCurve mergeCurve;

    IEvent<Bubble> comboVisual;

    private void Awake()
    {
        comboVisual = GetComponent<ComboVisual>();
    }

    public IEnumerator Visualize((List<Bubble> chain, Bubble target) values)
    {
        yield return new WaitForSeconds(mergeDelay);
        List<Vector3> startPositions = new();
        List<Bubble> toMove = new(values.chain);
        toMove.Remove(values.target);

        Vector3 targetPosition = values.target.transform.position;
        float timer = 0;

        for (int i = 0; i < toMove.Count; i++)
        {
            toMove[i].Particles.Play(false);
        }

        InitiateStartPositions();
        while (timer < mergeTime)
        {
            timer += Time.deltaTime;
            float t = timer / mergeTime;
            t = mergeCurve.Evaluate(t);

            for (int i = 0; i < toMove.Count; i++)
            {
                toMove[i].Visual.position = Vector3.LerpUnclamped(startPositions[i], targetPosition, t);
            }
            yield return null;
        }

        comboVisual.TriggerEvent(values.target);
        ResetVisuals();



        void InitiateStartPositions()
        {
            for (int i = 0; i < toMove.Count; i++)
            {
                startPositions.Add(toMove[i].transform.position);
            }
        }

        void ResetVisuals()
        {
            Transform visual;
            for (int i = 0; i < toMove.Count; i++)
            {
                visual = toMove[i].Visual;
                visual.GetComponent<SpriteRenderer>().enabled = false;
                visual.localPosition = Vector3.zero;
            }
        }
    }

    
}
