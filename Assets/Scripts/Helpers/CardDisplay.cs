using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class CardDisplay : NetworkBehaviour
{
    public CardInfo card;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text costText;
    public TMP_Text attackText;
    public TMP_Text healthText;
    public Image artworkImg;
    public bool isCopper;

    /*
    public Text manaText;
    public Text attackText;
    public Text healthText;
    */

    [ClientRpc]
    public void RpcUISetCardInfo(CardInfo card)
    {
        UISetCardInfo(card);
    }

    [Command]
    public void CmdUISetCardInfo(CardInfo card)
    {
        UISetCardInfo(card);
    }

    public void UISetCardInfo(CardInfo card)
    {
        this.card = card;

        nameText.text = card.title;
        descriptionText.text = card.description;

        costText.text = card.cost.ToString();
        attackText.text = card.attack.ToString();
        healthText.text = card.health.ToString();

        isCopper = false;
    }

    [Command]
    public void CmdSetCopper()
    {
        SetCopper();
    }
    void SetCopper()
    {
        // nameText.text = "Copper";
        // costText.text = "0";
        isCopper = true;
    }
}
