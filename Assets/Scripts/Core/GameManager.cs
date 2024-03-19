using System;
using UnityEngine;

public enum GameStates
{
    Title,
    Game,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager acc;

    [Header("Initiation")]

    public BubbleSpawner Spawner;
    public BubbleShooter Shooter;

    [SerializeField] InputManager IN;
    [SerializeField] GameNavigation NAV;
    [SerializeField] TapToPlayVisual TAP;

    public Camera cam;

    Touch activeTouch;

    GameStates state;
    Coroutine gameRoutine;

    Action activeUpdate; 

    private void Awake()
    {
        acc = this;
        AllignCamToDevice.ScaleCamToDevice();
    }

    void Start()
    {
        SwitchGameState((int)GameStates.Title);
    }

    private void Update()
    {

        activeUpdate?.Invoke();
    }

    void GameUpdate()
    {
        if (IN.GetActiveTouch(out activeTouch))
            Shooter.ProcessShootingInput(activeTouch);
    }


    public void SwitchGameState(int newState)
    {
        if (NAV.InTransition)
            return;

        switch ((GameStates)newState)
        {
            case GameStates.Title:
                TAP.TapRoutine = StartCoroutine(TAP.TapToPlayRoutine());
                break;

            case GameStates.Game:
                StopCoroutine(TAP.TapRoutine);
                activeUpdate = GameUpdate;

                if (state == GameStates.Title)
                {
                    StartCoroutine(NAV.Title_Game_Navigation(() =>
                    {
                        Spawner.InitializeBubbleField();
                        PlayGameLoop();
                    }));
                }
                else if (state == GameStates.GameOver)
                {
                    GameData.Score = 0;
                    GameData.RowPushTimer = 0;
                    PlayGameLoop();
                    NAV.GameOver_Game_Navigation();
                    Spawner.ResetBubbleField();
                }

                SavingSystem.LoadGame();
                break;

            case GameStates.GameOver:
                SavingSystem.SaveGame();
                StartCoroutine(NAV.Game_GameOver_Navigation(() =>
                {
                    foreach (Bubble bubble in BubbleManager.acc.FieldBubbles)
                    {
                        bubble.DetacheBubble();
                    }
                }));

                StopCoroutine(gameRoutine);
                activeUpdate = null;
                break;
        }

        state = (GameStates)newState;
    }

    public void PlayGameLoop()
    {
        if (gameRoutine != null && GameData.PausedGame)
        {
            GameData.PausedGame = false;
            return;
        }
        gameRoutine = StartCoroutine(BubbleManager.acc.RowPushRoutine());
    }
}
