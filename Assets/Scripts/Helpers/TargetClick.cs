using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TargetClick : NetworkBehaviour
{
    public Player p;

    public void OnTargetClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        p = networkIdentity.GetComponent<Player>();

        if (hasAuthority)
        {
            p.CmdTargetSelfCard();
        }
        else
        {
            p.CmdTargetOtherCard(gameObject);
        }

    }
}
