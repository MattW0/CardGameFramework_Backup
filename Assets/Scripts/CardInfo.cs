using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName ="Card")]
public class CardInfo : ScriptableObject
{
    public string title;
    public string description;

    //public Sprite artwork;

    public int cost;
    public int attack;
    public int health;

}
