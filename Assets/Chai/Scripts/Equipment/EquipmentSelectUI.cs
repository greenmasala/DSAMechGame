using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelectUI : MonoBehaviour
{
    public Button[] Buttons;

    public float AnimSpeed;

    private void OnEnable()
    {
        StartCoroutine(ButtonPopup());
        //foreach (var button in Buttons)
        //{
        //    TextMeshProUGUI[] texts = button.GetComponentsInChildren<TextMeshProUGUI>();
        //    foreach (var text in texts)
        //    {
        //        text.enabled = text.GetComponentInParent<Button>().interactable;
        //    }
        //}
    }

    IEnumerator ButtonPopup()
    {// Step 1: Move ALL buttons down first
        foreach (var button in Buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();

            rect.anchoredPosition += new Vector2(0, -1000);
        }

        // Step 2: Animate buttons upward one by one
        foreach (var button in Buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();

            rect.DOAnchorPosY(rect.anchoredPosition.y + 1000, 0.25f)
                .SetUpdate(true);

            yield return new WaitForSeconds(AnimSpeed);
        }
    }
}
