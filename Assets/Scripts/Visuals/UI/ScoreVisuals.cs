using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreVisuals : MonoBehaviour
{
    const float HIGHEST_SCORE_BUMP = 2000;

    [SerializeField] TextMeshProUGUI scoreMesh;
    [SerializeField] TextMeshProUGUI highScoreMesh;
    [SerializeField] AnimationCurve bumpCurve;
    [SerializeField] AnimationCurve scoreCurve;
    [SerializeField] float bumpTime;
    Coroutine scoreBump;

    Vector3 _minScale = 1.2f * Vector3.one;
    Vector3 _maxScale = 2 * Vector3.one;

    int visualScore;

    bool updateHighscore;

    private void Start()
    {
        GameData.ScoreChanged += UpdateScoreVisuals;
        GameData.HighScoreChanged += UpdateHighScoreVisual;
    }

    void UpdateHighScoreVisual()
    {
        if(GameData.Score < GameData.HighScore)
            highScoreMesh.text = ParseScore(GameData.HighScore);
    }

    void UpdateScoreVisuals()
    {
        if(scoreBump != null)
            StopCoroutine(scoreBump);
        scoreBump = StartCoroutine(ScoreBumpRoutine());
    }

    IEnumerator ScoreBumpRoutine()
    {
        float timer = 0;
        float intensity = (GameData.Score - GameData.LastScore) / HIGHEST_SCORE_BUMP;
        Vector3 bumpScale = Vector3.Lerp(_minScale, _maxScale, intensity);

        int visualScoreStart = visualScore;


        while (timer < bumpTime)
        {
            timer += Time.deltaTime;
            float t = bumpCurve.Evaluate(timer / bumpTime);

            scoreMesh.transform.localScale = Vector3.Lerp(Vector3.one, bumpScale, t);
            visualScore = (int)Mathf.Lerp(visualScoreStart, GameData.Score, scoreCurve.Evaluate(timer / bumpTime));
            scoreMesh.text = ParseScore(visualScore);

            if(GameData.Score >= GameData.HighScore)
                highScoreMesh.text = ParseScore(visualScore);



            yield return null;
        }
    }

    string ParseScore(int score)
    {
        if(score > 1000)
        {
            string cleanScore = (score * .001f).ToString("0.0");
            return $"{cleanScore}K";
        }

        return score.ToString();

    }
}
