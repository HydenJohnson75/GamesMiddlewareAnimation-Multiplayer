using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    List<GameObject> allPlayers;
    public static AudioManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisableAudioListeners()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();

        Debug.Log(allPlayers.Count);

        foreach (GameObject player in allPlayers)
        {
            NetworkObject netPlayer = player.GetComponent<NetworkObject>();

            if (!netPlayer.IsLocalPlayer)
            {
                player.GetComponent<AudioListener>().enabled = false;
            }
        }
    }
}
