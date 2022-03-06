using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class IncrementClick : NetworkBehaviour
{
    private Player p;

    [SyncVar]
    public int numberOfClicks = 0;
    
    public void IncrementClicks()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        p = networkIdentity.GetComponent<Player>();
        p.CmdIncrementClick(gameObject);
    }

}
