using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerLobbyData 
{
    public string PlayerName { get; private set; }
    public ulong ClientId { get; private set;}

    public PlayerLobbyData(string playerName, ulong clientId)
    {
        PlayerName = playerName;
        ClientId = clientId;
    }
}
