using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.Rendering.DebugUI.Table;

public class BubbleSpawner : MonoBehaviour
{
    public const int MAX_FIELD_ROWS = 8;
    public const float BUBBLE_SCALE = .7f;
    public const float BUBBLE_HEIGHT_MULT = BUBBLE_SCALE * 0.8714285f;

    const float ROW_OFFSET_CONST = .25f;
    public const int START_ROWS = 3;

    [SerializeField] Bubble bubblePrefab;

    float _rowOffset;

    private void Start()
    {
        _rowOffset = -(ROW_OFFSET_CONST * BUBBLE_SCALE);
    }

    public void InitializeBubbleField()
    {
        SetFieldScale(BUBBLE_SCALE);
        CreateFieldRows();
        SetStartBubbleValues(START_ROWS);
        SetInitialBubbleNeighbours();

        BubbleManager.acc.FieldStructureUpdate(false);
    }

    public void ResetBubbleField()
    {
        int index = 0;
        foreach (BubbleRow row in BubbleManager.acc.FieldRows)
        {
            _rowOffset *= -1f;
            row.OffsetSign = (int)Mathf.Sign(_rowOffset);
            row.Index = index;
            index++;

            for (int i = 0; i < row.Bubbles.Length; i++)
            {
                Vector3 bubblePosition = new Vector3((i * BUBBLE_SCALE) + _rowOffset, -row.Index * BUBBLE_HEIGHT_MULT, 0);
                row.Bubbles[i].transform.localPosition = bubblePosition;
                row.Bubbles[i].Visual.localPosition = Vector3.zero;
                row.Bubbles[i].transform.localScale = BUBBLE_SCALE * Vector3.one;

            }
        }

        SetStartBubbleValues(START_ROWS);
        SetInitialBubbleNeighbours();
    }

    void SetFieldScale(float scale)
    {
        Vector3 bubbleScale = scale * Vector3.one;
        BubbleManager.acc.ShootBubble.transform.localScale = bubbleScale;
        BubbleManager.acc.ReserveBubble.transform.localScale = bubbleScale;
    }

    void CreateFieldRows()
    {

        for (int i = 0; i < MAX_FIELD_ROWS; i++)
        {
            BubbleRow row = CreateRow(i);
            BubbleManager.acc.FieldRows.Add(row);
        }

        BubbleRow CreateRow(int index)
        {
            _rowOffset *= -1f;
            BubbleRow row = new()
            {
                OffsetSign = (int)Mathf.Sign(_rowOffset),
                Index = index
            };

            for (int i = 0; i < row.Bubbles.Length; i++)
            {
                Vector3 bubblePosition = new Vector3((i * BUBBLE_SCALE) + _rowOffset, -row.Index * BUBBLE_HEIGHT_MULT, 0);
                Bubble bubble = Instantiate(bubblePrefab, BubbleManager.acc.SpawnParent);
                bubble.transform.localPosition = bubblePosition;
                bubble.transform.localScale = BUBBLE_SCALE * Vector3.one;
                BubbleManager.acc.FieldBubbles.Add(bubble);

                bubble.Row = row;
                row.Bubbles[i] = bubble;
            }
            return row;
        }
    }

    void SetStartBubbleValues(int rows)
    {
        for (int i = 0; i < rows; i++)
        {
            BubbleManager.acc.SetRowBubbles(BubbleManager.acc.FieldRows[i]);
        }

        BubbleManager.acc.ShootBubble.Exponent = 1;
        BubbleManager.acc.ReserveBubble.Exponent = 2;
    }

    void SetInitialBubbleNeighbours()
    {
        List<BubbleRow> rows = BubbleManager.acc.FieldRows;

        for (int i = 0; i < rows.Count; i++)
        {
            BubbleManager.acc.AssignRowBubbleNeighbours(rows[i]);
        }

    }
    
}
