using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public const int MIN_VALUE = 2;
    public static int MAX_VALUE = 2048;

    public static BubbleManager acc;
    public BubbleMerger Merger;

    public List<BubbleRow> FieldRows = new(); // highest Row is Index 0
    public List<Bubble> FieldBubbles = new();
    public Bubble ShootBubble;
    public Bubble ReserveBubble;

    public Transform SpawnParent;
    public List<Color> BubbleColors;

    IVisualize<Bubble[]> rowPushVisual;
    IVisualize<(BubbleRow, Action<BubbleRow>)> rowSpawnVisual;
    IEvent<BubbleRow> gameOverWarningVisual;
    IEvent<bool> clearFieldVisual;

    [SerializeField] float maxRowPushInterval;
    [SerializeField] float minRowPushInterval;
    [SerializeField] int fallOffRounds;
    [Space]
    [SerializeField] int highestStartExponent;

    private void Awake()
    {
        acc = this;
        
        GameData.HighestSpawnExponent = highestStartExponent;
        rowPushVisual = GetComponent<RowPushVisual>();
        rowSpawnVisual = GetComponent<RowFadeInVisual>();
        gameOverWarningVisual = GetComponent<GameOverWarningVisual>();
        clearFieldVisual = GetComponent<PerfectTrigger>();
    }

    public IEnumerator RowPushRoutine()
    {
        int pushedRows = 1;
        while (true)
        {
            GameData.RowPushInverval = Mathf.Lerp(maxRowPushInterval, minRowPushInterval, (float)pushedRows / fallOffRounds);
            yield return new WaitUntil(() => !GameData.PausedGame);



            GameData.RowPushTimer += Time.deltaTime;
            if (GameData.RowPushTimer < GameData.RowPushInverval)
            {
                yield return null;
                continue;
            }

            yield return new WaitUntil(() => !GameData.ActiveShot && !GameData.Aiming);

            if (!FieldRows[^1].IsEmpty())
            {
                GameManager.acc.SwitchGameState((int)GameStates.GameOver);
                yield break;
            }

            gameOverWarningVisual.TriggerEvent(FieldRows[^2]);
            yield return StartCoroutine(NextRowUpdate());

            GameData.RowPushTimer = 0;
            pushedRows++;
        }
    }

    public IEnumerator NextRowUpdate()
    {
        GameData.ActiveRowPush = true;

        yield return StartCoroutine(PushDownRows());
        MoveUpLastRow();
        UpdateAllRowIndex();

        AssignRowBubbleNeighbours(FieldRows[0]);
        AssignRowBubbleNeighbours(FieldRows[1]);
        AssignRowBubbleNeighbours(FieldRows[^1]);
        GameData.ActiveRowPush = false;
        yield return StartCoroutine(rowSpawnVisual.Visualize((
            FieldRows[0],
            SetRowBubbles
            )));

    }

    public void FieldStructureUpdate(bool validate)
    {
        bool clearField = true;
        foreach (Bubble bubble in FieldBubbles)
        {
            if (validate)
            {
                bubble.Validated = false;
                bubble.BaseConnectionCheck();
            }

            if(bubble.IsActive)
                clearField = false;
        }

        clearFieldVisual.TriggerEvent(clearField);

        ManageFieldState.acc.AddToFieldHistory(FieldRows, ShootBubble.Exponent, ReserveBubble.Exponent);
    }

    public List<int> GetFieldExponents()
    {
        List<int> fieldExponents = new();

        foreach (Bubble bubble in FieldBubbles)
        {
            if (!bubble.IsActive)
                continue;
            else if (!fieldExponents.Contains(bubble.Exponent))
                fieldExponents.Add(bubble.Exponent);
        }
        return fieldExponents;
    }

    public int GetFieldAverage()
    {
        int bubbles = 0;
        int sum = 0;

        foreach (Bubble bubble in FieldBubbles)
        {
            if (!bubble.IsActive)
                continue;
            bubbles++;
            sum += bubble.Exponent;
        }

        return Mathf.RoundToInt((float)sum / bubbles);
    }

    public void AssignRowBubbleNeighbours(BubbleRow row)
    {
        BubbleRow preRow = null;
        BubbleRow postRow = null;

        if (row.Index > 0)
            preRow = FieldRows[row.Index - 1];
        if (row.Index < FieldRows.Count - 1)
            postRow = FieldRows[row.Index + 1];

        for (int i = 0; i < row.Bubbles.Length; i++)
        {
            row.Bubbles[i].Neighbours.Clear();

            if (preRow != null)
            {
                row.Bubbles[i].Neighbours.Add(preRow.Bubbles[i]);
                if (ValidIndex(i + row.OffsetSign))
                    row.Bubbles[i].Neighbours.Add(preRow.Bubbles[i + row.OffsetSign]);
            }

            if (i + 1 < BubbleRow.LENGTH)
                row.Bubbles[i].Neighbours.Add(row.Bubbles[i + 1]);
            if (i - 1 >= 0)
                row.Bubbles[i].Neighbours.Add(row.Bubbles[i - 1]);

            if (postRow != null)
            {
                row.Bubbles[i].Neighbours.Add(postRow.Bubbles[i]);
                if (ValidIndex(i + row.OffsetSign))
                    row.Bubbles[i].Neighbours.Add(postRow.Bubbles[i + row.OffsetSign]);
            }
        }

        static bool ValidIndex(int index)
        {
            return index < BubbleRow.LENGTH && index >= 0;
        }
    }

    public IEnumerator PushDownRows()
    {
        for (int i = FieldRows.Count - 1; i >= 0; i--)
        {
            yield return StartCoroutine(rowPushVisual.Visualize(FieldRows[i].Bubbles));
        }
    }

    public void SetRowBubbles(BubbleRow row)
    {
        foreach (Bubble bubble in row.Bubbles)
        {
            bubble.Exponent = UnityEngine.Random.Range(1, GameData.HighestSpawnExponent);
            bubble.IsConnected = true;
        }
    }

    public void SwitchShooterWithReserve()
    {
        (ReserveBubble.Exponent, ShootBubble.Exponent) = (ShootBubble.Exponent, ReserveBubble.Exponent);
    }

    void UpdateAllRowIndex()
    {
        for (int i = FieldRows.Count - 1; i >= 0; i--)
        {
            FieldRows[i].Index = i;
        }
    }

    void MoveUpLastRow()
    {
        BubbleRow row = FieldRows[^1];
        FieldRows.RemoveAt(FieldRows.Count - 1);
        FieldRows.Insert(0, row);

        foreach (Bubble bubble in row.Bubbles)
        {
            Vector3 bubblePosition = bubble.transform.position;
            bubblePosition.y = SpawnParent.position.y;
            bubble.transform.position = bubblePosition;
            bubble.Visual.localScale = .9f * Vector3.one;
        }

    }
}
