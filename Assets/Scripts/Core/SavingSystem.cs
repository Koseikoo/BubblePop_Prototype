using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SavingSystem
{
    public static void SaveGame()
    {
        PlayerPrefs.SetInt("HighScore", GameData.HighScore);
        PlayerPrefs.SetInt("Level", GameData.CurrentLevel);
        PlayerPrefs.SetInt("PointsToNextLevel", GameData.PointsToNextLevel);
        PlayerPrefs.SetInt("LevelPoints", GameData.LevelPoints);
    }

    public static void LoadGame()
    {
        GameData.HighScore = PlayerPrefs.GetInt("HighScore");
        GameData.CurrentLevel = PlayerPrefs.GetInt("Level");
        GameData.PointsToNextLevel = PlayerPrefs.GetInt("PointsToNextLevel");
        GameData.LevelPoints = PlayerPrefs.GetInt("LevelPoints");
    }

}
