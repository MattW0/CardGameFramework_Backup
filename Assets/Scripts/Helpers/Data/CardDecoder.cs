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
}

#if UNITY_EDITOR
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
            Debug.Log("Creating " + deck.cards.Length + " cards");

            foreach(var card in deck.cards) {
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

                UnityEditor.AssetDatabase.CreateAsset(cardInfo, $"Assets/Assets/NewCards/{card.title}.asset");
                UnityEditor.AssetDatabase.SaveAssets();
            }
    }
#endif

