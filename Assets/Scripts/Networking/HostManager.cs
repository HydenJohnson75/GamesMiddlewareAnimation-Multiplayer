using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostManager : MonoBehaviour
{
    [SerializeField] private string gameScene = "TestTransferScene";

    public static HostManager Instance {get; private set;}

    private bool gameStarted;
    public string joinCode { get; private set;}
    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async void StartHost()
    {
        Allocation relayAllocation;

        try
        {
            relayAllocation = await RelayService.Instance.CreateAllocationAsync(4);
        }
        catch (Exception e)
        {
            Debug.LogError("Relay Failed :" + e.Message);
            throw;
        }

        try
        {
           joinCode = await RelayService.Instance.GetJoinCodeAsync(relayAllocation.AllocationId);
        }
        catch(Exception e) 
        {
            Debug.LogError("Relay join code error :" + e.Message);
            throw;
        }

        var realyServerData = new RelayServerData(relayAllocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(realyServerData);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApproveCallback;
        NetworkManager.Singleton.OnServerStarted += ServerStarted;

        ClientData = new Dictionary<ulong, ClientData>();

        Debug.Log(joinCode);

        NetworkManager.Singleton.StartHost();
    }

    private void ServerStarted()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong obj)
    {
        if (ClientData.ContainsKey(obj))
        {
            if (ClientData.Remove(obj))
            {
                Debug.Log($"Removed client {obj}");
            }
        }
    }

    private void ApproveCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(ClientData.Count >= 4) 
        {
            response.Approved = true;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);

        Debug.Log($"Added client {request.ClientNetworkId}");
    }


    public void StartGame()
    {
        gameStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
