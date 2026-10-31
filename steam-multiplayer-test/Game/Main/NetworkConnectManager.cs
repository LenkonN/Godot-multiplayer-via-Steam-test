using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot.Steamworks.Net;
using Godot.Steamworks.Net.Multiplayer.Peer;
using Steamworks;

public partial class NetworkConnectManager : Node
{
    public event Action ServerStarted;
    public event Action ServerConnected;
    public event Action ConnectionFailed;
    public event Action ServerDisconnected;
    public event Action<long> PeerConnected;
    public event Action<long> PeerDisconnected;

    public const int Port = 7777;
    public const int MaxClients = 8;

    public override void _Ready()
    {
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;

        Multiplayer.ServerDisconnected += OnServerDisconnected;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
    }

    public Error Host()
    {
        var peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(Port, MaxClients);

        if (error != Error.Ok)
        {
            return error;
        }

        Multiplayer.MultiplayerPeer = peer;

        ServerStarted?.Invoke();
        return Error.Ok;
    }

    public void HostSteam()
    {
        SteamMultiplayerPeer steamPeer = new SteamMultiplayerPeer();

        Error error = steamPeer.CreateHost(0);

        if (error != Error.Ok)
        {
            steamPeer = null;
            return;
        }

        Multiplayer.MultiplayerPeer = steamPeer;
    }

    public Error Join(string address)
    {
        var peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address, Port);

        if (error != Error.Ok)
        {
            return error;
        }

        Multiplayer.MultiplayerPeer = peer;

        return Error.Ok;
    }

    public void SteamJoin(CSteamID hostId)
    {
        SteamMultiplayerPeer steamPeer = new SteamMultiplayerPeer();

        Error error = steamPeer.CreateClient(hostId.m_SteamID, 0);

        if (error != Error.Ok)
        {
            steamPeer = null;
            return;
        }

        Multiplayer.MultiplayerPeer = steamPeer;

        MainManager.PlayerInfo.Id = Multiplayer.GetUniqueId();
        MainManager.NetworkManager.NetworkConnectType = NetworkConnectType.Steam;

    }

    public void Disconnect()
    {
        Multiplayer.MultiplayerPeer?.Close();
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnPeerDisconnected(long id)
    {
        PeerDisconnected?.Invoke(id);
    }

    private void OnPeerConnected(long id)
    {
        PeerConnected?.Invoke(id);
    }

    private void OnServerDisconnected()
    {
        ServerDisconnected?.Invoke();
    }

    private void OnConnectionFailed()
    {
        Multiplayer.MultiplayerPeer = null;
        ConnectionFailed?.Invoke();
    }

    private void OnConnectedToServer()
    {
        ServerConnected?.Invoke();
    }
}