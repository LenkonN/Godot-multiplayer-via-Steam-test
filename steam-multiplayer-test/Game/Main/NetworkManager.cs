using Godot;
using System;
using System.Collections.Generic;

public partial class NetworkManager : Node
{
    public NetworkConnectType NetworkConnectType { get; set; } = NetworkConnectType.None;
    public List<PlayerInfo> NetworkPlayers { get; private set;  } = new List<PlayerInfo>();

    public bool CheckMyOwner(long idObject)
    {
        if (idObject == Multiplayer.GetUniqueId()) return true;
        else return false;
    }

    public bool CheckIsServer(long idObject)
    {
        if (idObject == 1 && Multiplayer.IsServer()) return true;
        else return false;
    }

    public bool CheckOnlyServerButNotServerObject(long idObject)
    {
        if (idObject != 1 && Multiplayer.IsServer()) return true;
        else return false;
    }

    public void AddPlayerInfo(PlayerInfo playerInfo)
    {
        NetworkPlayers.Add(playerInfo);
    }

    public void RemovePlayerInfo()
    {
        
    }
}
