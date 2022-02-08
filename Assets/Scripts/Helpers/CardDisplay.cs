using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    private CardInfo card;
    public Text nameText;
    public Text descriptionText;
    public TMP_Text manaCostText;
    public TMP_Text attackText;
    public TMP_Text healthText;

    public Image artworkImg;

    /*
    public Text manaText;
    public Text attackText;
    public Text healthText;
    */

    public void SetCard(CardInfo card)
    {
        this.card = card;

        nameText.text = card.title;
        descriptionText.text = card.description;

        manaCostText.text = card.cost.ToString();
        attackText.text = card.attack.ToString();
        healthText.text = card.health.ToString();
    }
}
