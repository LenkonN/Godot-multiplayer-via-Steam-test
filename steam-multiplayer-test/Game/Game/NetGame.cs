using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class NetGame : Node
{
    [Export] private Game _game { get; set; }

    private readonly HashSet<long> _spawnedPlayers = new();

    public override void _Ready()
    {
        MainManager.NetworkConnectManager.PeerConnected += PeerContected;
        MainManager.NetworkConnectManager.PeerDisconnected += PeerDisconected;
    }

    // SERVER STUFF
    public void StartServer()
    {
        if (!Multiplayer.IsServer()) return;
        SpawnPlayer(1);
    }

    // CLIENT STUFF


    // SERVER + CLIENT STUFF

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void SpawnPlayer(long peerId)
    {
        if (_spawnedPlayers.Contains(peerId)) return;

        Player player = _game.SpawnPlayer();
        player.Name = $"Player_{peerId}";

        player.NetPlayer.OwnerId = peerId;

        player.SetMultiplayerAuthority((int)peerId);

        _spawnedPlayers.Add(peerId);

        GetParent().AddChild(player);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void DespawnPlayer(long peerId)
    {
        _game.DeletePlayer(peerId);
        _spawnedPlayers.Remove(peerId);
    }

    // MAIN NETWORK STUFF

    private void PeerContected(long peerId)
    {
        //Only server
        if (!Multiplayer.IsServer()) return;

        //New player for all (include server)
        Rpc(nameof(SpawnPlayer), peerId);

        //Give to client existing players;
        foreach (long existingPeerId in Multiplayer.GetPeers())
        {
            if (existingPeerId == peerId) continue;

            RpcId(peerId, nameof(SpawnPlayer), existingPeerId);
        }

        // Create first player
        RpcId(peerId, nameof(SpawnPlayer), 1L);
    }

    private void PeerDisconected(long peerId)
    {
        if (!Multiplayer.IsServer()) return;

    }
}
