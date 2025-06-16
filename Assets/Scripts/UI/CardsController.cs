using UnityEngine;

public class CardsController : MonoBehaviour
{
    [SerializeField] private Card[] cards;

    private void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentUser == null)
        {
            Debug.LogError("GameManager or CurrentUser is not initialized.");
            return;
        }

       // string language = GameManager.Instance.SelectedLanguage ?? GameManager.Instance.CurrentUser.language;
       // LocalizationManager.Instance.LoadLanguage(language);

        int cardIndex = 0;
        foreach (Card card in cards)
        {
            card.Initialize(
                "test",
                null,
                cardIndex == 2 // Example condition to set the third card as selected
            );
            card.OnClick += (c) =>
            {
                foreach (Card otherCard in cards)
                {
                    if (otherCard != c)
                    {
                        otherCard.ResetCard();
                    }
                }
            };
            cardIndex++;
        }
    }
}