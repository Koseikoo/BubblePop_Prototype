using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboVisual : MonoBehaviour, IEvent<Bubble>
{
    const float BASE_SIZE = 100;
    const float SIZE_MULT = 20;

    [SerializeField] Vector3 visualOffset;
    [SerializeField] TextMeshProUGUI comboTextMesh;
    RectTransform comboTransform;
    Animator comboAnimation;

    private void Awake()
    {
        if (comboTextMesh == null)
        {
            return;
        }
        comboTransform = comboTextMesh.GetComponent<RectTransform>();
        comboAnimation = comboTextMesh.GetComponent<Animator>();
    }

    public void TriggerEvent(Bubble bubble)
    {
        if (GameData.CurrentCombo <= 1)
            return;

        Vector2 screenPos = GameManager.acc.cam.WorldToScreenPoint(bubble.transform.position + visualOffset);
        comboTextMesh.text = $"x{GameData.CurrentCombo}";
        comboTextMesh.fontSize = BASE_SIZE + (SIZE_MULT * GameData.CurrentCombo);
        comboTransform.position = screenPos;
        comboAnimation.SetTrigger("ComboAnimation");

    }

    
}
