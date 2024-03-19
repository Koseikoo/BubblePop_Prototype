using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    [SerializeField] int POINT_CONSTANT;
    [SerializeField] float POINT_MULT;

    private void Awake()
    {
        GameData.ScoreChanged += AddPoints;
    }

    public void AddPoints()
    {
        int pointsToAdd = GameData.Score - GameData.LastScore;

        if(GameData.PointsToNextLevel == 0)
            GameData.PointsToNextLevel = POINT_CONSTANT;

        GameData.LevelPoints += pointsToAdd;
        if (GameData.LevelPoints >= GameData.PointsToNextLevel)
            LevelUp();
    }

    void LevelUp()
    {
        GameData.LevelPoints -= GameData.PointsToNextLevel;
        GameData.PointsToNextLevel += (int)(GameData.PointsToNextLevel * POINT_MULT);

        GameData.CurrentLevel++;
    }

    
}
