using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragDrop : NetworkBehaviour
{
    public Player p;
    private GameObject Canvas;
    private GameObject startParent;
    private GameObject dropZone;

    private Vector2 startPosition;

    public float returnDuration = 0.9f;
    public float timeStartReturn = 0f;

    public bool isDragging = false;
    private bool isOverDropZone = false;
    private bool isDraggable = false;
    private bool isReturning = false;

    private void Start()
    {
        Canvas = GameObject.Find("Canvas");
        if (hasAuthority)
        {
            isDraggable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Debug.Log("Dragging"+transform.position);
            transform.SetParent(Canvas.transform, true);
        }
        // Return card to original position
        else if (isReturning)
        {
            transform.position = Vector2.Lerp(transform.position, startPosition, timeStartReturn);
            timeStartReturn += Time.deltaTime;

            // If position is close enough to startPosition, then stop
            if (Vector2.Distance(transform.position, startPosition) < 0.1f)
            {
                transform.position = startPosition;
                transform.SetParent(startParent.transform, true);
                isReturning = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }

    public void StartDrag()
    {
        if (!isDraggable) return;
        // if (isReturning) return;

        isDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        Debug.Log("StartDrag:"+startPosition.ToString());
    }

    public void EndDrag()
    {
        if (!isDraggable) return;

        isDragging = false;

        if (isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);
            isDraggable = false;
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            p = networkIdentity.GetComponent<Player>();
            // p.PlayCard(gameObject);
        }
        else
        {
            isReturning = true;
            timeStartReturn = 0f;
        }
    }
}
