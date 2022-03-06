using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardZoom : NetworkBehaviour
{
    private GameObject Canvas;
    public GameObject ZoomCardPrefab;
    private GameObject zoomedCard;
    private CardDisplay cardInfo;
    private Transform t;
    public int hoverOffset = 250;


    public void Awake()
    {
        Canvas = GameObject.Find("Canvas");
        cardInfo = gameObject.GetComponent<CardDisplay>();
        t = gameObject.transform;
    }

    public void OnHoverEnter()
    {
        if (!hasAuthority) return;
        if (gameObject.GetComponent<DragDrop>().isDragging) return;

        // Instantiate zoom prefab at mouse location
        zoomedCard = Instantiate(ZoomCardPrefab, new Vector2(Input.mousePosition.x,
                                                             t.position.y + hoverOffset),
                                                             Quaternion.identity);
                                                             
        zoomedCard.transform.SetParent(Canvas.transform, true);
        zoomedCard.layer = LayerMask.NameToLayer("Zoom");

        if (cardInfo.isCopper)
        {
            zoomedCard.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/copper");
            foreach (RectTransform child in zoomedCard.transform)
            {
                GameObject go = child.gameObject;
                if(!go.name.Equals("Cost")) go.SetActive(false);
            }
        }
        else
        {
            // Setting card text and stats from parent
            zoomedCard.GetComponent<CardDisplay>().UISetCardInfo(cardInfo.card); 
        }
    }
    public void OnHoverExit()
    {
        Destroy(zoomedCard);
    }
}
