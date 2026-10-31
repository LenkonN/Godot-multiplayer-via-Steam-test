using Godot;
using System;

public partial class Main : Node
{
    [Export] private PackedScene _menuScene { get; set; }

    [Export] private PackedScene _gameScene { get; set; }

    private Game _game { get; set; }

    private Menu _menu { get; set; }

    [Export] private NetworkConnectManager _networkConnectManager { get; set; }
    [Export] private NetworkManager _networkManager { get; set; }
    [Export] private NetworkMessageManager _networkMessageManager { get; set; }
    [Export] private SteamConnectManager _steamConnectManager { get; set; }

    public override void _Ready()
    {
        MainManager.Main = this;
        MainManager.NetworkConnectManager = _networkConnectManager;
        MainManager.NetworkManager = _networkManager;
        MainManager.NetworkMessageManager = _networkMessageManager;
        MainManager.SteamConnectManager = _steamConnectManager;
        CreateMenu();
    }

    public void DeleteGame()
    {
        _game.QueueFree();
    }

    public void CreateGame()
    {
        _game = _gameScene.Instantiate<Game>();
        AddChild(_game);
    }

    public void CreateMenu()
    {
        _menu = _menuScene.Instantiate<Menu>();
        AddChild(_menu);
    }

    public void DeleteMenu()
    {
        _menu.QueueFree();
    }
}
