using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardZoom : NetworkBehaviour
{
    private GameObject Canvas;
    public GameObject ZoomCard;

    private GameObject zoomedCard;
    private Sprite bg;

    public Transform t;

    public int zoomFactor = 3;
    public int hoverOffset = 250;


    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
        bg = gameObject.GetComponent<Image>().sprite;
    }

    public void OnHoverEnter()
    {
        if (!hasAuthority) return;

        zoomedCard = Instantiate(ZoomCard, new Vector2(Input.mousePosition.x,
                                                       t.position.y + hoverOffset),
                                                       Quaternion.identity);
        zoomedCard.GetComponent<Image>().sprite = bg;
        zoomedCard.transform.SetParent(Canvas.transform, true);
        zoomedCard.layer = LayerMask.NameToLayer("Zoom");
                    
        RectTransform rect = zoomedCard.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(80 * zoomFactor, 110 * zoomFactor);

        // foreach (Transform child in rect)
        //      print("Foreach loop: " + child);

        //      RectTransform rectChild = child.GetComponent<RectTransform>();
        //     rect.sizeDelta = new Vector2(80 * zoomFactor, 110 * zoomFactor);
    }
    public void OnHoverExit()
    {
        Destroy(zoomedCard);
    }
}
