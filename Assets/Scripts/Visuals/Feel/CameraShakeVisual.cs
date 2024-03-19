using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeVisual : MonoBehaviour, IVisualize<int>
{
    [SerializeField] int maxShakes;
    [SerializeField] float maxIntensity;
    [SerializeField] float timePerShake;

    float relativeIntensity;
    int shakes;

    Coroutine shake;

    public IEnumerator Visualize(int mergeScore)
    {
        if (shake != null)
            StopCoroutine(shake);
        shake = StartCoroutine(ShakeRoutine(mergeScore));
        yield break;

    }

    IEnumerator ShakeRoutine(int mergeScore)
    {
        float timer;
        relativeIntensity = mergeScore / BubbleManager.MAX_VALUE;
        shakes = Mathf.RoundToInt(Mathf.Lerp(1, maxShakes, relativeIntensity));
        Vector2[] shakePositions = GetShakePositions(shakes, relativeIntensity);

        for (int i = 0; i < shakePositions.Length - 1; i++)
        {
            timer = 0;
            while (timer < timePerShake)
            {
                timer += Time.deltaTime;
                float t = timer / timePerShake;

                Vector3 newPosition = Vector3.Lerp(shakePositions[i], shakePositions[i + 1], t);
                newPosition.z = -10;
                GameManager.acc.cam.transform.position = newPosition;

                yield return null;
            }
        }
    }

    Vector2[] GetShakePositions(int shakes, float intensity)
    {
        Vector2[] shakePositions = new Vector2[shakes + 1];

        shakePositions[0] = GameManager.acc.cam.transform.position;
        for (int i = 1; i < shakePositions.Length - 1; i++)
            shakePositions[i] = Random.insideUnitCircle.normalized * (relativeIntensity * maxIntensity);
        shakePositions[^1] = GameManager.acc.cam.transform.position;

        return shakePositions;
    }
}
