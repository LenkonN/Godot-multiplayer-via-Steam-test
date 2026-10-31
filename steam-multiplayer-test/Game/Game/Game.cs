using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
    [Export] private PackedScene _playerScene { get; set; }

    [Export] private Marker2D _spawnPos { get; set; }
    [Export] private NetGame _netGame { get; set; }

    public override void _Ready()
    {
        if (MainManager.NetworkManager.NetworkConnectType is not NetworkConnectType.None) _netGame.StartServer();
        else SpawnPlayer();
    }

    public Player SpawnPlayer()
    {
        Player player = _playerScene.Instantiate<Player>();
        MainManager.GameManager.Players.Add(player);
        player.GlobalPosition = _spawnPos.GlobalPosition;

        if (MainManager.NetworkManager.NetworkConnectType is NetworkConnectType.None) AddChild(player);

        return player;
    }

    public void DeletePlayer(long id)
    {
        for (int i = MainManager.GameManager.Players.Count - 1; i > 0; i--)
        {
            if (MainManager.GameManager.Players[i].Name == $"Player_{id}")
            {
                MainManager.GameManager.Players[i].QueueFree();
            }
        }
    }
}
