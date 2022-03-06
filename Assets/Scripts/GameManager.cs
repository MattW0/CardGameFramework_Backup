using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private int turnNb = 0;
    private static Player[] players;

    [Header("Health")]
    public int maxHealth = 30;

    [Header("Mana")]
    public int maxMana = 10;

    [Header("Hand")]
    public int handSize = 7;
    // public PlayerHand playerHand;
    // public PlayerHand enemyHand;

    [Header("Deck")]
    public int deckSize = 30; // Maximum deck size
    public int identicalCardCount = 2; // How many identical cards we allow to have in a deck

    [Header("Battlefield")]
    // public PlayerField playerField;
    // public PlayerField enemyField;

    [Header("Turn Management")]
    public GameObject endTurnButton;
    [HideInInspector] public bool isOurTurn = false;
    [SyncVar, HideInInspector] public int turnCount = 1; // Start at 1

    [Command]
    public void CmdUpdatePlayersReady()
    {
        UpdatePlayersReady();
    }

    [Server]
    public void UpdatePlayersReady()
    {
        players = GameObject.FindObjectsOfType<Player>();

        if (players.Length == 1)
        {
            Debug.Log("2 players ready");
            StartGame();
        }
    }

    [Server]
    public void UpdateTurnsPlayed()
    {
        RpcLogToClients("Turn: " + turnNb);
    }

    [ClientRpc]
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    public void StartGame()
    {
        Debug.Log("Game started");

        foreach (Player p in players)
        {
            Debug.Log("Player: " + p.username);
            p.PrepareDeck();
        }
    }
}
