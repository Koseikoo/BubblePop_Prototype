using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FieldState
{
    public FieldState(List<BubbleRow> fieldRows, int ShootExponent, int ReserveExponent)
    {
        FieldExponents = new int[BubbleSpawner.MAX_FIELD_ROWS, BubbleRow.LENGTH];

        for (int i = 0; i < fieldRows.Count; i++)
        {
            for (int j = 0; j < fieldRows[i].Bubbles.Length; j++)
            {
                FieldExponents[i, j] = fieldRows[i].Bubbles[j].Exponent;
            }
        }

        this.ShootExponent = ShootExponent;
        this.ReserveExponent = ReserveExponent;

    }

    public int[,] FieldExponents;
    public int ShootExponent;
    public int ReserveExponent;
}

public class ManageFieldState : MonoBehaviour
{
    public static ManageFieldState acc;
    private void Awake()
    {
        acc = this;
    }

    const float MAX_HISTORY = 8;

    public List<FieldState> FieldHistory = new();
    int currentIndex;

    public void AddToFieldHistory(List<BubbleRow> fieldRows, int ShootExponent, int ReserveExponent)
    {
        if (FieldHistory.Count >= MAX_HISTORY)
        {
            FieldHistory.RemoveAt(0);
            currentIndex--;
        }

        FieldHistory.Add(new(fieldRows, ShootExponent, ReserveExponent));
        currentIndex++;
    }

    public void GoForward()
    {
        if (currentIndex == FieldHistory.Count - 1)
            return;

        currentIndex++;
        SetFieldBubbles(FieldHistory[currentIndex]);

    }

    public void GoBackward()
    {
        if (currentIndex == 0)
            return;

        currentIndex--;
        SetFieldBubbles(FieldHistory[currentIndex]);
    }

    void SetFieldBubbles(FieldState state)
    {
        List<BubbleRow> rows = BubbleManager.acc.FieldRows;

        for (int i = 0; i < rows.Count; i++)
        {
            for (int j = 0; j < rows[i].Bubbles.Length; j++)
            {
                rows[i].Bubbles[j].Exponent = state.FieldExponents[i, j];
            }
        }

        BubbleManager.acc.ShootBubble.Exponent = state.ShootExponent;
        BubbleManager.acc.ReserveBubble.Exponent = state.ReserveExponent;
    }


}
