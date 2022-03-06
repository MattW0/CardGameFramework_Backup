using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    NetworkManager nm;
    private GameObject PlayerArea;
    private GameObject OpponentArea;
    private GameObject PlayerDropZone;
    private GameObject OpponentDropZone;
    private GameObject PlayerHand;
    private GameObject OpponentHand;
    private DeckManager dm;
    private GameManager gm;

    
    // Events that the UI will subscribe to
    public event System.Action<int> OnPlayerNumberChanged;
    public event System.Action<Color32> OnPlayerColorChanged;
    public event System.Action<int> OnPlayerDataChanged;

    // Players List to manage playerNumber
    static readonly List<Player> playersList = new List<Player>();
    List<GameObject> cardsInHand = new List<GameObject>();

    ///////////////////////////////////////////////////////////////////////////////////
    [Header("Player Info")]
    [SyncVar(hook = nameof(UpdatePlayerName))] public string username; // SyncVar hook to call a command whenever a username changes (like when players load in initially).

    [Header("Portrait")]
    public Sprite portrait; // For the player's icon at the top left of the screen & in the PartyHUD.

    [Header("Deck")]
    public Deck deck;
    public Sprite cardback;

    [Header("Stats")]
    [SyncVar] public int maxMana = 10;
    [SyncVar] public int currentMax = 0;
    [SyncVar] public int _mana = 0;
    public int mana
    {
        get { return Mathf.Min(_mana, maxMana); }
        set { _mana = Mathf.Clamp(value, 0, maxMana); }
    }

    // Quicker access for UI scripts
    [HideInInspector] public static Player localPlayer;
    [HideInInspector] public bool hasEnemy = false; // If we have set an enemy.
    [HideInInspector] public PlayerInfo enemyInfo;
    [HideInInspector] public static GameManager gameManager;
    [SyncVar, HideInInspector] public bool firstPlayer = false; // Is it player 1, player 2, etc.

    internal static void ResetPlayerNumbers()
    {
        int playerNumber = 0;
        foreach (Player player in playersList)
            player.playerNumber = playerNumber++;
    }

    [Header("Player UI")]
    public GameObject playerUIPrefab;
    GameObject playerUI;

    [Header("SyncVars")]

    /// <summary>
    /// This is appended to the player name text, e.g. "Player 01"
    /// </summary>
    [SyncVar(hook = nameof(PlayerNumberChanged))]
    public int playerNumber = 0;

    /// <summary>
    /// This is updated by UpdateData which is called from OnStartServer via InvokeRepeating
    /// </summary>
    [SyncVar(hook = nameof(PlayerDataChanged))]
    public int playerData = 0;

    /// <summary>
    /// Random color for the playerData text, assigned in OnStartServer
    /// </summary>
    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color32 playerColor = Color.white;

    // This is called by the hook of playerNumber SyncVar above
    void PlayerNumberChanged(int _, int newPlayerNumber)
    {
        OnPlayerNumberChanged?.Invoke(newPlayerNumber);

    }

    // This is called by the hook of playerData SyncVar above
    void PlayerDataChanged(int _, int newPlayerData)
    {
        OnPlayerDataChanged?.Invoke(newPlayerData);
    }

    // This is called by the hook of playerColor SyncVar above
    void PlayerColorChanged(Color32 _, Color32 newPlayerColor)
    {
        OnPlayerColorChanged?.Invoke(newPlayerColor);
    }

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerArea = GameObject.Find("PlayerArea");
        OpponentArea = GameObject.Find("OpponentArea");

        PlayerArea.gameObject.SetActive(true);
        OpponentArea.gameObject.SetActive(true);

        // Add this to the static Players List
        playersList.Add(this);

        // set the Player Color SyncVar
        playerColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

        // Start generating updates
        InvokeRepeating(nameof(UpdateData), 1, 1);
    }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer()
    {
        CancelInvoke();
        playersList.Remove(this);
    }

    // This only runs on the server, called from OnStartServer via InvokeRepeating
    [ServerCallback]
    void UpdateData()
    {
        playerData = UnityEngine.Random.Range(100, 1000);
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        // if (hasAuthority)
        // {
        //     // Instantiate the player UI as child of the Players Panel
        //     playerUI = Instantiate(playerUIPrefab, PlayerArea.transform.Find("PlayerInfo").transform);
        // }
        // else 
        // {
        //     playerUI = Instantiate(playerUIPrefab, OpponentArea.transform.Find("OpponentInfo").transform);
        // }

        // // Set this player object in PlayerUI to wire up event handlers
        // playerUI.GetComponent<PlayerUI>().SetPlayer(this, isLocalPlayer);

        // Invoke all event handlers with the current data
        OnPlayerNumberChanged?.Invoke(playerNumber);
        OnPlayerColorChanged?.Invoke(playerColor);
        OnPlayerDataChanged?.Invoke(playerData);

        PlayerDropZone = GameObject.Find("PlayerDropZone");
        OpponentDropZone = GameObject.Find("OpponentDropZone");
        PlayerHand = GameObject.Find("PlayerHand");
        OpponentHand = GameObject.Find("OpponentHand");

        // Host calls gameManager (on server) to check if an 2 players are ready
        if (hasAuthority) CmdCheckPlayersReady();
    }

    [Command]
    public void CmdCheckPlayersReady()
    {
        gm.UpdatePlayersReady();
    }

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;

        // Get and update the player's username and stats
        CmdLoadPlayer(PlayerPrefs.GetString("Name"));
    }


    [Command]
    public void CmdLoadPlayer(string user)
    {
        // Update the player's username, which calls a SyncVar hook.
        // Learn more here : https://mirror-networking.com/docs/Guides/Sync/SyncVarHook.html
        username = user;
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient()
    {
        // Remove this player's UI object
        Destroy(playerUI);

        // Disable the main panel for local player
        if (isLocalPlayer)
            // nm.mainPanel.gameObject.SetActive(false);
            Debug.Log("OnStopClient");
    }

    public void PrepareDeck()
    {
        dm = this.GetComponent<DeckManager>();
        dm.PrepareDeck();
        Debug.Log("Deck setup");
    }

    [Command]
    public void CmdDrawCards()
    {
        int drawNb = 1;

        for(int i=0; i<drawNb; i++)
        {
            GameObject card = dm.DrawCard();
            cardsInHand.Add(card);
        }
    }

    [ClientRpc]
    public void RpcShowCard(GameObject card, string type)
    {
        switch (type)
        {
            case "deck":
                card.GetComponent<CardFlipper>().Flip();
                break;

            case "hand":
                if (hasAuthority)
                    {
                        card.GetComponent<CardFlipper>().Flip();
                        card.transform.SetParent(PlayerHand.transform, false);
                    }
                else card.transform.SetParent(OpponentHand.transform, false);
                break;

            case "played":
                if (hasAuthority)
                {
                    card.transform.SetParent(PlayerDropZone.transform, false);
                    break;
                }
                card.GetComponent<CardFlipper>().Flip();
                card.transform.SetParent(OpponentDropZone.transform, false);
                break;
        }
    }

    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }

    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity opponentIdentity = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(opponentIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetSelfCard()
    {
        //Debug.Log("Targeted by self");
    }

    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
        //Debug.Log("Targeted by other");
    }

    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);
    }

    [ClientRpc]
    void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().numberOfClicks++;
        //Debug.Log("This card has been clicked " + card.GetComponent<IncrementClick>().numberOfClicks + " times.");
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    void UpdatePlayerName(string oldUser, string newUser)
    {
        // Update username
        username = newUser;

        // Update game object's name in editor (only useful for debugging).
        gameObject.name = newUser;
    }

    [ClientRpc]
    private void RpcPlayersSetup()
    {
        // Find all Players and add them to the list.
        Player[] onlinePlayers = FindObjectsOfType<Player>();

        // Loop through all online Players (should just be one other Player)
        foreach (Player player in onlinePlayers)
        {
            // There should only be one other Player online, so if it's not us then it's the enemy.
            if (player != this)
            {
                // Get & Set PlayerInfo from our Enemy's gameObject
                PlayerInfo currentPlayer = new PlayerInfo(player.gameObject);
                enemyInfo = currentPlayer;
                hasEnemy = true;
                // enemyInfo.data.casterType = Target.OPPONENT;
                //Debug.LogError("Player " + username + " Enemy " + enemy.username + " / " + enemyInfo.username); // Used for Debugging
            }
        }

        gameManager = FindObjectOfType<GameManager>();
        // health = gameManager.maxHealth;
        maxMana = gameManager.maxMana;
        // deck.deckSize = gameManager.deckSize;
        // deck.handSize = gameManager.handSize;
    }
}
