using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelectUI : MonoBehaviour
{
    public Button[] Buttons;
    public RectTransform BGImage;
    public RectTransform Rect;

    public float AnimSpeed;

    private void Start()
    {
        Rect = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        foreach (var button in Buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();

            rect.anchoredPosition += new Vector2(0, -1000);
        }

        BGImage.anchoredPosition += new Vector2(-2000, 0);

        Sequence tween = DOTween.Sequence();
        tween.Append(BGImage.DOAnchorPosX(BGImage.anchoredPosition.x + 2000, .24f));
        tween.Append(BGImage.DOShakeAnchorPos(0.5f, 25f, 20));
        tween.InsertCallback(0.20f, () =>
        {
            StartCoroutine(ButtonPopup());
        });
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
    {
        //tween.Join(BGImage.DOShakeAnchorPos(0.5f, 25f, 20));

        //foreach (var button in Buttons)
        //{
        //    RectTransform rect = button.GetComponent<RectTransform>();

        //    rect.anchoredPosition += new Vector2(0, -1000);
        //}

        foreach (var button in Buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();

            Sequence tween2 = DOTween.Sequence();
            tween2.Append(rect.DOAnchorPosY(rect.anchoredPosition.y + 1000, 0.25f));
            tween2.Append(rect.DOShakeAnchorPos(0.5f, 25f, 20));

            yield return new WaitForSeconds(AnimSpeed);
        }
    }
}
