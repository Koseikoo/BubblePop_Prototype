using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    static int score;
    public static int Score
    {
        get { return score; }
        set
        {
            score = value;
            ScoreChanged?.Invoke();
            LastScore = score;

            if(score > HighScore)
                HighScore = score;

        }
    }

    static int highScore;
    public static int HighScore
    {
        get { return highScore; }
        set
        {
            highScore = value;
            HighScoreChanged?.Invoke();
        }
    }
    public static int LastScore;

    public static bool ActiveRowPush;
    public static bool ActiveShot;
    public static bool Aiming;

    public static float RowPushInverval;
    static float rowPushTimer;
    public static float RowPushTimer
    {
        get { return rowPushTimer; }
        set
        {
            rowPushTimer = value;
            RowPushTimerChanged?.Invoke();

        }
    }

    public static bool PausedGame;

    public static float HighestExponent;
    public static float LowestExponent;

    public static int CurrentCombo;

    public static int HighestSpawnExponent;

    static int currentLevel;
    public static int CurrentLevel
    {
        get { return currentLevel; }
        set
        {
            currentLevel = value;
            LevelChanged?.Invoke();
        }
    }
    public static int PointsToNextLevel;
    static int levelPoints;
    public static int LevelPoints
    {
        get { return levelPoints; }
        set
        {
            levelPoints = value;
            LevelPointsChanged?.Invoke();

        }
    }

    public static event Action ScoreChanged;
    public static event Action HighScoreChanged;
    public static event Action LevelPointsChanged;
    public static event Action LevelChanged;
    public static event Action RowPushTimerChanged;


}
