using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectTrigger : MonoBehaviour, IEvent<bool>
{
    [SerializeField] Animator perfectNotationAnimator;

    public void TriggerEvent(bool clear)
    {
        if (!clear)
            return;

        perfectNotationAnimator.SetTrigger("ComboAnimation");
    }
}
