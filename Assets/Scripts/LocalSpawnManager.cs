using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalSpawnManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject monsterPrefab;


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //NetworkManager.Singleton.OnClientConnectedCallback += testSpawn;
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    }

    private void Update()
    {

    }

    private void testSpawn(ulong obj)
    {
        SpawnPlayer2(obj);
    }

    private void LoadClient(ulong clientId)
    {
        // Spawn player for all clients, including the local client
        SpawnPlayerServerRpc(clientId, true);

        Debug.Log(clientId);
    }

    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer) return;

        if (sceneName == "LocalHorrorScene")
        {
            //VivoxService.Instance.ParticipantAddedToChannel += SetParticipantAudioLocation;

            SpawnMonsterServerRpc();
            
            foreach (ulong clientid in clientsCompleted)
            {

                SpawnPlayerServerRpc(clientid, true);
            }

            AudioManager.instance.DisableAudioListeners();
        }

        // Spawn players for all completed clients when the scene is loaded
        Debug.Log(clientsCompleted.Count);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId, bool isLocalPlayer)
    {
        GameObject player = Instantiate(Player);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, isLocalPlayer);


        if (isLocalPlayer)
        {

        }
    }

    private void SpawnPlayer2(ulong playerId)
    {
        GameObject player = Instantiate(Player);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId);

    }

    [ServerRpc (RequireOwnership =false)]
    private void SpawnMonsterServerRpc()
    {
        GameObject monsterSpawn = GameObject.FindGameObjectWithTag("MonsterSpawn");
        GameObject monster = Instantiate(monsterPrefab, monsterSpawn.transform.position, Quaternion.identity);
        monster.GetComponent<NetworkObject>().Spawn();
    }
}

    //private void Start()
    //{
    //    DontDestroyOnLoad(this.gameObject);
    //}

//public override void OnNetworkSpawn()
//{
//    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
//    NetworkManager.Singleton.OnClientConnectedCallback += LoadClient;
//}

//private void LoadClient(ulong obj)
//{
//    //GameObject player = Instantiate(Player);

//    SpawnPlayer(obj, true);
//}

//private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
//{
//    if(IsServer && sceneName == "LocalHorrorScene")
//    {
//        SpawnMonster();
//    }

//    //if (sceneName == "LocalHorrorScene")
//    //{
//    //    foreach (ulong id in clientsCompleted)
//    //    {
//    //        SpawnPlayer(id, id == NetworkManager.Singleton.LocalClientId);
//    //    }
//    //}
//}

//private void SpawnPlayer(ulong playerId, bool isLocalPlayer)
//{
//    GameObject player = Instantiate(Player);
//    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, isLocalPlayer);

//    // Example: Attach player controls only for the local player
//    if (isLocalPlayer)
//    {
//        // Attach player controls or other local-specific scripts
//    }
//}

//private void SpawnMonster()
//{
//    GameObject monsterSpawn = GameObject.FindGameObjectWithTag("MonsterSpawn");
//    GameObject monster = Instantiate(monsterPrefab, monsterSpawn.transform.position, Quaternion.identity);
//    monster.GetComponent<NetworkObject>().Spawn();
//}
