using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveBubbleSwitcher : MonoBehaviour
{
    private void OnMouseDown()
    {
        BubbleManager.acc.SwitchShooterWithReserve();
    }

}
