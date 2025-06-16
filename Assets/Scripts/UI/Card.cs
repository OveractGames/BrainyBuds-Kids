using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public string cardDescription;
    public Sprite cardImage;

    public event Action<Card> OnClick;

    public void Initialize(string description, Sprite image, bool isSelected = false)
    {
        Debug.Log("Card initialized with description: " + description);
        cardDescription = description;
        cardImage = image;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);
        if (isSelected)
        {
            OnCardClicked();
        }
    }

    private void OnCardClicked()
    {
        Debug.Log($"Card {cardDescription} clicked!");
        DisplayCardInfo();
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        OnClick?.Invoke(this);
    }

    public void ResetCard()
    {
        transform.localScale = Vector3.one;
    }

    public void DisplayCardInfo()
    {
        Debug.Log($"Description: {cardDescription}");
    }
}