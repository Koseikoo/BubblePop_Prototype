using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactVisual : MonoBehaviour, IVisualize<Bubble>
{
    [SerializeField] AnimationCurve impactCurve;
    [SerializeField] float impactDuration;
    [SerializeField] float impactForceMult;
    [SerializeField] bool impactByEponent;

    public IEnumerator Visualize(Bubble bubble)
    {
        float timer = 0;

        List<(Bubble, Vector3, Vector3)> impactInfo = GetImpactTuples(bubble);

        while (timer < impactDuration)
        {
            timer += Time.deltaTime;
            float t = impactCurve.Evaluate(timer / impactDuration);

            foreach ((Bubble bubble, Vector3 start, Vector3 end) info in impactInfo)
            {
                info.bubble.Visual.position = Vector3.Lerp(info.start, info.end, t);
            }
            yield return null;
        }

        List<(Bubble bubble, Vector3 start, Vector3 end)> GetImpactTuples(Bubble bubble)
        {
            List<(Bubble, Vector3, Vector3)> impactInfo = new();

            foreach (Bubble neighbour in bubble.Neighbours)
            {
                if (!neighbour.IsActive)
                    continue;
                float exponentMult = impactByEponent ? bubble.Exponent : 1;
                Vector3 direction = exponentMult * impactForceMult * (neighbour.transform.position - bubble.transform.position).normalized;

                impactInfo.Add((neighbour, neighbour.transform.position, neighbour.transform.position + direction));
            }

            return impactInfo;

        }
    }

    



    
}
