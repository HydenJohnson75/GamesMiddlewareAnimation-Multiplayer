using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class VivoxSetupScript : MonoBehaviour
{

    List<string> blockedPeople;
    private async Task InitializeVivoxAsync(string playerId)
    {
        await VivoxService.Instance.InitializeAsync();

        blockedPeople = new List<string>();
        blockedPeople.Add("NoOne");
        await LoginToVivoxAsync(blockedPeople, playerId);
    }

    private async Task LoginToVivoxAsync(List<string> blockedList, string displayName)
    {
        LoginOptions options = new LoginOptions();
        options.DisplayName = displayName;
        options.BlockedUserList = blockedList;
        await VivoxService.Instance.LoginAsync(options);
    }

    // Start is called before the first frame update
    void Start()
    {
        ClientConnected(AuthenticationService.Instance.PlayerId);
    }

    private async void ClientConnected(string obj)
    {
        await InitializeVivoxAsync(obj);

        await OnVivoxUserLoggedIn();
    }

    private async Task OnVivoxUserLoggedIn()
    {
        Debug.Log(VivoxService.Instance.AvailableInputDevices);
        //Channel3DProperties properties = new Channel3DProperties();
        //await VivoxService.Instance.JoinEchoChannelAsync("RandyTest", ChatCapability.AudioOnly);
        await VivoxService.Instance.JoinGroupChannelAsync("RandyTest", ChatCapability.AudioOnly);
        //await VivoxService.Instance.JoinPositionalChannelAsync("New", ChatCapability.AudioOnly, properties);

        //VivoxService.Instance.ParticipantAddedToChannel += OnParticipantAdded;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
