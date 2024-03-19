using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class LevelVisual : MonoBehaviour
{
    [SerializeField] ParticleSystem levelUpFX;
    [SerializeField] ProceduralImage levelBar;
    [SerializeField] TextMeshProUGUI curLevelMesh;
    [SerializeField] RectTransform curLevelRect;
    [Space]
    [SerializeField] TextMeshProUGUI nextLevelMesh;
    [SerializeField] RectTransform nextLevelRect;
    [Space]
    [SerializeField] float levelUpLerpTime;
    [SerializeField] float maxScale;
    [SerializeField] AnimationCurve scaleCurve;



    Coroutine levelUpRoutine;

    private void Awake()
    {
        GameData.LevelPointsChanged += VisualizePointChange;
        GameData.LevelChanged += LevelUpVisual;
    }

    void VisualizePointChange()
    {
        if (GameData.PointsToNextLevel == 0)
            return;

        int levelPoints = Mathf.Clamp(GameData.LevelPoints, 1, GameData.PointsToNextLevel);
        levelBar.fillAmount = (float)levelPoints / GameData.PointsToNextLevel;
    }

    void LevelUpVisual()
    {
        levelUpFX.Play();
        if (levelUpRoutine != null)
            StopCoroutine(levelUpRoutine);
        levelUpRoutine = StartCoroutine(LevelUpLerp());

        curLevelMesh.text = GameData.CurrentLevel.ToString();
        nextLevelMesh.text = (GameData.CurrentLevel + 1).ToString();
    }

    IEnumerator LevelUpLerp()
    {
        float scale;
        float t;
        float timer = 0;
        while (timer < levelUpLerpTime)
        {
            timer += Time.deltaTime;
            t = scaleCurve.Evaluate(timer / levelUpLerpTime);
            scale = Mathf.Lerp(1, maxScale, t);

            curLevelRect.localScale = scale * Vector3.one;
            nextLevelRect.localScale = scale * Vector3.one;

            yield return null;
        }
    }
}
