using UnityEngine;
using Mirror;

// Doesnt do anything special but it's set up to be built-upon
[AddComponentMenu("Network Manager Extended")]
public class NetworkManagerExtended : NetworkManager
{
    // Called when Player connects to the server and joins the game
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = Instantiate(playerPrefab);
        player.name = "Player " + (numPlayers + 1);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
