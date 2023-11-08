using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorSpawner : NetworkBehaviour
{
    public GameObject doorPrefab;  // The door prefab to be spawned.

    private void Start()
    {
        SpawnDoor();
    }

    // You can call this method to spawn a door on the server.
    public void SpawnDoor()
    {

            // Spawn the door at a specific position and rotation.
            Vector3 spawnPosition = new Vector3(30f, 1.399f, 23.065f); // Adjust this position as needed.
            Quaternion spawnRotation = Quaternion.Euler(0f, -89.5f, 0f);  // Adjust this rotation as needed.

            // Use the NetworkSpawnManager to spawn the door as a networked object.
            GameObject doorObject = Instantiate(doorPrefab, spawnPosition, spawnRotation);
            doorObject.GetComponent<NetworkObject>().Spawn();

    }
}
