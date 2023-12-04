using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

using UnityEngine.UIElements;

public class NetMainMenu : MonoBehaviour
{

    [SerializeField] private GameObject connectMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private GameObject background;

    private async void Start()
    {
        try
        {
            string _server, _domain, _issuer, _key;

            _server = "https://unity.vivox.com/appconfig/197d4-games-22259-udash";
            _domain = "mtu1xp.vivox.com";
            _issuer = "197d4-games-22259-udash";
            _key = "SLtHqz3tvxnwHO1uOVOFHkxGXt6npwLx";

            InitializationOptions options = new InitializationOptions();
            options.SetVivoxCredentials(_server, _domain, _issuer, _key);

            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // LoginToVivoxAsync(blockedPeople, AuthenticationService.Instance.PlayerId.ToString());
            //disconnectButton.onClick.AddListener(delegate { VivoxService.Instance.LeaveChannelAsync(connectChannelName); });

            Debug.Log($"Player Id: " +AuthenticationService.Instance.PlayerId);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        connectMenu.SetActive(false);
        background.SetActive(true);
        mainMenu.SetActive(true);
    }


    public void StartHost()
    {
        HostManager.Instance.StartHost();
    }

    public void StartClient()
    {
        ClientManager.Instance.StartClient(joinCodeInputField.text);
    }

}
