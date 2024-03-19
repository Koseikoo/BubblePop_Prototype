using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BubbleRow
{
    public const int LENGTH = 6;
    
    public int Index;
    public int OffsetSign;
    public Bubble[] Bubbles = new Bubble[LENGTH];

    public bool IsEmpty()
    {
        for (int i = 0; i < Bubbles.Length; i++)
        {
            if (Bubbles[i].IsActive && Bubbles[i].IsConnected)
                return false;
        }
        return true;
    }
}
