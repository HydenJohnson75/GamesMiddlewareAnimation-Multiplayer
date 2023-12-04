using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerCardScript : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] Image readyUpImage;
    [SerializeField] private GameObject visuals;

    bool isReady;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Unready()
    {
        readyUpImage.color = Color.red;
    }

    private void ReadyUp()
    {
        readyUpImage.color = Color.green;
    }

    internal void SetPlayerName(string _playerName)
    {
        playerNameText.text = _playerName;
    }

    public void UpdateUI(LobbyPlayerState state)
    {
        playerNameText.text = state.ClientId.ToString();
        if(state.IsReady)
        {
            ReadyUp();
        }
        else
        {
            Unready();  
        }
        visuals.SetActive(true);
    }

    public void DisableUI()
    {
        visuals.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
