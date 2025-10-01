using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterCard : MonoBehaviour
{
    [SerializeField] private TMP_Text chapterTitleText;
    [SerializeField] private Image chapterImage;
    private Button button;

    public void Initialize(CardData data, Action<int> OnCardClicked)
    {
        if (chapterTitleText)
        {
            chapterTitleText.text = data.cardName;
        }

        if (chapterImage)
        {
            chapterImage.sprite = data.cardImage;
        }

        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnCardClicked?.Invoke(data.cardID));
        Focus(data.cardID == 0);
    }

    private void Focus(bool focus = true)
    {
        float scale = focus ? 1.1f : 1f;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}