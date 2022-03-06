using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardPilesDisplay : MonoBehaviour
{
    public TMP_Text numberCardsText;
    public Image cardPileImage;
    // Start is called before the first frame update
    void Start()
    {
        // string name = gameObject.name;
        // if (name == "DrawPile")
        // {
        //     cardPileImage.sprite = Resources.Load<Sprite>("Sprites/CardBack");
        //     cardPileImage.color = new Color(1, 1, 1, 1f);
        // }
    }

    public void SetNumberCards(int nb)
    {
        numberCardsText.text = nb.ToString();
    }
    public void UpdateNumberCards(int nb, bool decrement)
    {
        int nbCards = int.Parse(numberCardsText.text);
        if (decrement)
        {
            nbCards -= nb;
        }
        else
        {
            nbCards += nb;
        }
        numberCardsText.text = nbCards.ToString();

        // if (nbCards < 1)
        // {
        //     ToggleCardbackSwap();
        // }
    }

    private void ToggleCardbackSwap()
    {
        if (cardPileImage.sprite != null)
        {
            cardPileImage.sprite = null;
            cardPileImage.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            cardPileImage.sprite = Resources.Load<Sprite>("Sprites/CardBack");
        }
    }

}
