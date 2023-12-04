using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Vivox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour
{
    private NetworkList<LobbyPlayerState> players;
    [SerializeField] private Transform cardHolder;
    [SerializeField] private LocalPlayerCardScript [] playerCards;
    [SerializeField] Button readyButton;
    [SerializeField] Button unReadyButton;
    [SerializeField] Button startButton;
    [SerializeField] TMP_Text lobbyCode;
    public Dictionary<ulong, GameObject> playerInfo = new Dictionary<ulong, GameObject>();

    public bool isReady { get; private set; }
    public static LobbyUI Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        players = new NetworkList<LobbyPlayerState>();
        //VivoxService.Instance.ParticipantRemovedFromChannel += OnParticipantDisconnected;
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            players.OnListChanged += PlayerStateChanged;
        }

        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;

            foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                ClientConnected(client.ClientId);
            }
        }

        if (IsHost)
        {
            lobbyCode.text = "Lobby Code: " + HostManager.Instance.joinCode;
            
        }

        
    }

    private void PlayerStateChanged(NetworkListEvent<LobbyPlayerState> changeEvent)
    {
        for(int i = 0; i < playerCards.Length; i++) 
        {
            if(players.Count > i)
            {
                playerCards[i].UpdateUI(players[i]);    
            }
            else
            {
                playerCards[i].DisableUI();
            }
        }

        foreach(var player in players)
        {

            if (player.ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if(player.IsReady)
            {

                unReadyButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
            }
            if(!player.IsReady) 
            {
                readyButton.gameObject.SetActive(true);
                unReadyButton.gameObject.SetActive(false);
            }

        }

        foreach (var player in players)
        {
            if (!player.IsReady)
            {
                startButton.gameObject.SetActive(false);
                break;
            }
            else
            {
                if (IsHost)
                {
                    startButton.gameObject.SetActive(true);
                }
            }
        }



    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            players.OnListChanged -= PlayerStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnected;
        }
    }

    private void ClientDisconnected(ulong obj)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == obj)
            {
                players.RemoveAt(i);
                break;
            }
        }
    }




    private void ClientConnected(ulong obj)
    {
        players.Add(new LobbyPlayerState(obj, false));
    }

    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("LocalHorrorScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerUnreadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for(int i = 0; i< players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            players[i] = new LobbyPlayerState(players[i].ClientId, false);
        }
    }

    public void ReadyUp()
    {
        PlayerReadyUpServerRpc();
    }

    public void UnReady()
    {
        PlayerUnreadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyUpServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            players[i] = new LobbyPlayerState(players[i].ClientId, true);
        }
    }
}
