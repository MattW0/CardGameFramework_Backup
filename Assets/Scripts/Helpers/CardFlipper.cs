using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    public Sprite cardFront;
    public Sprite cardBack;

    public void Flip()
    {
        Sprite currenSprite = gameObject.GetComponent<Image>().sprite;

        if (currenSprite == cardFront)
        {
            gameObject.GetComponent<Image>().sprite = cardBack;

            // Hiding text and other info
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = cardFront;

            // Showing text and other info
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}

