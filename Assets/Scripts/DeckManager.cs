using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeckManager : NetworkBehaviour
{
    private CardInfo[] cardStats;
    private Player player;
    public GameObject Copper;
    public GameObject Card;
    List<GameObject> Deck = new List<GameObject>();
    List<GameObject> OpponentDeck = new List<GameObject>();
    private GameObject PlayerDrawPile;
    private GameObject PlayerDiscardPile;
    private GameObject OpponentDrawPile;
    private GameObject OpponentDiscardPile;

    int startNbCards = 10;
    int nbSpecial = 3;

    public void PrepareDeck()
    {
        if (!hasAuthority) return;

        cardStats = Resources.LoadAll<CardInfo>("Cards");

        PlayerDrawPile = GameObject.Find("PlayerDrawPile").gameObject;
        PlayerDiscardPile = GameObject.Find("PlayerDiscardPile").gameObject;
        OpponentDrawPile = GameObject.Find("OpponentDrawPile").gameObject;
        OpponentDiscardPile = GameObject.Find("OpponentDiscardPile").gameObject;

        player = this.GetComponent<Player>();

        DeckSetup();

        Shuffle(Deck);
        PlayerDrawPile.GetComponent<CardPilesDisplay>().SetNumberCards(startNbCards);
    }

    public void DeckSetup()
    {
        // Starting deck: 7 Copper, 3 Special
        for(int i=0; i<startNbCards - nbSpecial; i++)
        {
            GameObject copper = Instantiate(Copper, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(copper, connectionToClient);
            copper.GetComponent<CardDisplay>().CmdSetCopper();

            player.RpcShowCard(copper, "deck");
            if (hasAuthority) 
            {
                copper.transform.SetParent(PlayerDrawPile.transform.GetChild(0).transform, false);
                Deck.Add(copper);
            }
            else 
            {
                copper.transform.SetParent(OpponentDrawPile.transform.GetChild(0).transform, false);
                OpponentDeck.Add(copper);
            }
        }

        for(int i=0; i<nbSpecial; i++)
        {
            GameObject special = Instantiate(Card, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(special, connectionToClient);

            special.GetComponent<CardDisplay>().RpcUISetCardInfo(cardStats[Random.Range(0, cardStats.Length)]);
            
            if (hasAuthority)
            {
                special.transform.SetParent(PlayerDrawPile.transform.GetChild(0).transform, false);
                Deck.Add(special);
                player.RpcShowCard(special, "deck");
            } 
            else 
            {
                special.transform.SetParent(OpponentDrawPile.transform.GetChild(0).transform, false);
                OpponentDeck.Add(special);
                player.RpcShowCard(special, "deck");
            }
        }
    }

    public GameObject DrawCard()
    {   
        GameObject card = Deck[0];
        Deck.RemoveAt(0);

        PlayerDrawPile.GetComponent<CardPilesDisplay>().UpdateNumberCards(1, decrement: true);

        player.RpcShowCard(card, "hand");

        return card;
    }

    private void Shuffle(List<GameObject> d)
    {
        Debug.Log("Shuffling deck");

        var count = d.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = d[i];
            d[i] = d[r];
            d[r] = tmp;
        }
    }
}
