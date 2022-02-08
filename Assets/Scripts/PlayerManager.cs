using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject Card;
    public GameObject PlayerArea;
    public GameObject OpponentArea;
    public GameObject PlayerDropZone;
    public GameObject OpponentDropZone;

    private CardInfo[] cards;
    List<GameObject> cardsDrawn = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        OpponentArea = GameObject.Find("OpponentArea");
        PlayerDropZone = GameObject.Find("PlayerDropZone");
        OpponentDropZone = GameObject.Find("OpponentDropZone");
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        cards = Resources.LoadAll<CardInfo>("Cards");
        Debug.Log(cards.Length);
        Debug.Log(cards[0].ToString());
    }

    [Command]
    public void CmdDrawCards()
    {
        for(int i=0; i<5; i++)
        {
            GameObject playerCard = Instantiate(Card, new Vector2(0, 0), Quaternion.identity);
            playerCard.GetComponent<CardDisplay>().SetCard(cards[Random.Range(0, cards.Length)]);
            cardsDrawn.Add(playerCard);

            NetworkServer.Spawn(playerCard, connectionToClient);
            RpcShowCard(playerCard, "dealt");
        }
    }

    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "played");

        if (isServer)
        {
            UpdateTurnsPlayed();
        }
    }

    [Server]
    void UpdateTurnsPlayed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateTurnsPlayed();
        RpcLogToClients("Turns played: " + gm.turnsPlayed);
    }

    [ClientRpc]
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "dealt")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.GetComponent<CardFlipper>().Flip();
                card.transform.SetParent(OpponentArea.transform, false);
            }
        }

        else if (type == "played")
        {
            card.transform.SetParent(PlayerDropZone.transform, false);
            if (!hasAuthority)
            {
                card.GetComponent<CardFlipper>().Flip();
            }
        }
    }

    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }

    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity opponentIdentity = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(opponentIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetSelfCard()
    {
        Debug.Log("Targeted by self");
    }

    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
        Debug.Log("Targeted by other");
    }

    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);
    }

    [ClientRpc]
    void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().numberOfClicks++;
        Debug.Log("This card has been clicked " + card.GetComponent<IncrementClick>().numberOfClicks + " times.");
    }
}
