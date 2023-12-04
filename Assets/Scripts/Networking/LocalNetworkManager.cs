using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LocalNetworkManager : MonoBehaviour
{
    [SerializeField] GameObject loginMenu;
    [SerializeField] GameObject selectClientTypeMenu;
    [SerializeField] GameObject lobbyMenu;
    [SerializeField] Button enterButton;
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    [SerializeField] Button startButton;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] GameObject playerLayout;
    [SerializeField] GameObject localPlayerCard;
    string playerName;

    public static LocalNetworkManager instance;

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
        enterButton.onClick.AddListener(ChangeToTypeMenu);
        hostButton.onClick.AddListener(HostGame);
        clientButton.onClick.AddListener(JoinAsClient);
        NetworkManager.Singleton.OnClientConnectedCallback += CreateCards;
    }

    private void CreateCards(ulong obj)
    {
        LocalPlayerCardScript playerCard = Instantiate(localPlayerCard, playerLayout.transform).GetComponent<LocalPlayerCardScript>();
        playerCard.SetPlayerName(playerName);

    }

    private void ChangeToTypeMenu()
    {
        playerName = playerNameInput.text;
        loginMenu.SetActive(false);
        selectClientTypeMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void HostGame()
    {
        NetworkManager.Singleton.StartHost();
        selectClientTypeMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    private void JoinAsClient()
    {
        NetworkManager.Singleton.StartClient();
        selectClientTypeMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }
}
