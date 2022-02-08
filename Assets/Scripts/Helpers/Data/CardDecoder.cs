using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Deck {
  public Card[] cards;

}

[System.Serializable]
public class Card{

    public string title;
    public int cost;
    public int attack;
    public int health;
    public string keyword_abilities;
    public string special_abilities;
    public string triggers;
    public string hash;

    public Card(string title)
    {
        this.title = title;
        this.cost = cost;
        this.attack = attack;
        this.health = health;
        this.keyword_abilities = keyword_abilities;
        this.special_abilities = special_abilities;
        this.triggers = triggers;
        this.hash = hash;
    }

}
public class CardDecoder : MonoBehaviour
{
    public static CardDecoder instance;
    public bool reloadJson = false;
    
    private string path_json = "cards_from_100";

    private void Awake()
    {
        if (reloadJson)
        {
            createDeck(path_json);
            reloadJson = false;
        }
    }
    public void createDeck(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        Debug.Log(textAsset.text);

        Deck deck = JsonUtility.FromJson<Deck>(textAsset.text);
        Debug.Log(deck.ToString());
        Debug.Log(deck.cards.Length);

        foreach(var card in deck.cards) {
            Debug.Log(card.cost);
            CreateSOCard(card);
        }
    }
    
    public static void CreateSOCard(Card card)
        {
            CardInfo cardInfo = ScriptableObject.CreateInstance<CardInfo>();

            cardInfo.title = card.title;
            cardInfo.description = card.hash;
            cardInfo.cost = card.cost;
            cardInfo.attack = card.attack;
            cardInfo.health = card.health;

            UnityEditor.AssetDatabase.CreateAsset(cardInfo, $"Assets/Assets/Cards/{card.title}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
        }

}
