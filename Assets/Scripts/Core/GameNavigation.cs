using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class GameNavigation : MonoBehaviour
{
    [SerializeField] float backgroundAlpha;

    [Header("Title")]
    [SerializeField] RectTransform TitleText;
    [SerializeField] RectTransform StartButton;
    [SerializeField] RectTransform TitleBackground;
    [Space]
    [SerializeField] float titleTransitionTime;
    [SerializeField] Vector2 inActive_TitleOffset;
    Vector2 activeTitlePos;
    [SerializeField] Vector2 InActive_ButtonOffset;
    Vector2 activeButtonPos;
    [Space]
    [SerializeField] AnimationCurve titleCurve;

    [Header("GameOver")]
    [SerializeField] float timeBeforeGameOverScreen;
    [SerializeField] RectTransform GameOverText;
    [SerializeField] RectTransform GameOverRestartButton;
    [SerializeField] RectTransform GameOverBackground;

    [Header("Game")]
    [SerializeField] GameObject World;
    [SerializeField] GameObject GameCanvas;
    [SerializeField] GameObject ShootBubble;
    [SerializeField] GameObject ReserveBubble;

    Image titleBackground;
    Image gameOverBackground;
    [Space]
    [SerializeField] GameObject titleObject;
    [SerializeField] GameObject gameOverObj;

    public bool InTransition;

    private void Awake()
    {
        titleBackground = TitleBackground.GetComponent<Image>();
        gameOverBackground = GameOverBackground.GetComponent<Image>();

        activeTitlePos = TitleBackground.anchoredPosition;
        activeButtonPos = StartButton.anchoredPosition;
    }


    public IEnumerator Title_Game_Navigation(Action gameStartAction)
    {
        InTransition = true;

        float t;
        Color backgroundColor = titleBackground.color;

        float timer = 0;

        float startAlpha = backgroundAlpha;
        float endAlpha = 0;

        Vector2 titleStart = TitleText.anchoredPosition;
        Vector2 buttonStart = StartButton.anchoredPosition;

        Vector2 titleEnd = activeTitlePos + inActive_TitleOffset;
        Vector2 buttonEnd = activeButtonPos + InActive_ButtonOffset;

        while (timer < titleTransitionTime)
        {
            timer += Time.deltaTime;
            t = timer / titleTransitionTime;


            t = titleCurve.Evaluate(t);

            StartButton.anchoredPosition = Vector2.LerpUnclamped(buttonStart, buttonEnd, t);
            TitleText.anchoredPosition = Vector2.LerpUnclamped(titleStart, titleEnd, t);
            backgroundColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            titleBackground.color = backgroundColor;

            yield return null;
        }

        World.SetActive(true);
        GameCanvas.SetActive(true);
        ShootBubble.SetActive(true);
        ReserveBubble.SetActive(true);
        titleObject.SetActive(false);
        gameStartAction?.Invoke();

        InTransition = false;
    }

    public IEnumerator Game_GameOver_Navigation(Action gameOverAction)
    {
        InTransition = true;

        gameOverAction?.Invoke();
        yield return new WaitForSeconds(timeBeforeGameOverScreen);

        gameOverObj.SetActive(true);

        float t;
        Color backgroundColor = gameOverBackground.color;

        float timer = 0;

        float startAlpha = 0;
        float endAlpha = backgroundAlpha;

        while (timer < titleTransitionTime)
        {
            timer += Time.deltaTime;
            t = timer / titleTransitionTime;
            t = titleCurve.Evaluate(t);

            backgroundColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            gameOverBackground.color = backgroundColor;

            gameOverObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }


        InTransition = false;
    }

    public void GameOver_Game_Navigation()
    {
        gameOverObj.SetActive(false);
    }
}
