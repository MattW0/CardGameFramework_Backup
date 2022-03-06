using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    private bool frontUp = true;
    public void Flip()
    {
        if (frontUp)
        {
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/cardBack");

            // Hiding text and other info
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }

            frontUp = false;
        }

        else
        {
            if (gameObject.GetComponent<CardDisplay>().isCopper)
            {
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/copper");
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/cardFront");
            }

            // Showing text and other info
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }

            frontUp = true;
        }
    }
}

