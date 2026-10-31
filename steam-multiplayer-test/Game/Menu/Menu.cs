using Godot;
using Steamworks;
using System;

public partial class Menu : Control
{
    [Export] private LineEdit _gameNicknameLine { get; set; }
    [Export] private LineEdit _ipServer { get; set; }
    [Export] private Button _buttonClient { get; set; }
    [Export] private Button _buttonServer { get; set; }
    [Export] private Button _buttonSingle { get; set; }
    [Export] private Button _buttomSteamServer { get; set; }

    public override void _Ready()
    {
        _buttonClient.Pressed += OnClient;
        _buttonServer.Pressed += OnServer;
        _buttonSingle.Pressed += OnSignle;
        _buttomSteamServer.Pressed += OnSteamServer;
    }

    public override void _ExitTree()
    {
        _buttonClient.Pressed -= OnClient;
        _buttonServer.Pressed -= OnServer;
        _buttonSingle.Pressed -= OnSignle;
        _buttomSteamServer.Pressed -= OnSteamServer;
    }

    public override void _Process(double delta)
    {
        SaveNickname();
    }

    private void SaveNickname()
    {
        if (MainManager.PlayerInfo.GameName != _gameNicknameLine.Text)
        {
            MainManager.PlayerInfo.GameName = _gameNicknameLine.Text;
            MainManager.PlayerInfo.SteamName = SteamFriends.GetPersonaName();
        }
    }

    private void OnClient()
    {
        MainManager.NetworkConnectManager.Join(_ipServer.Text);
        MainManager.NetworkManager.NetworkConnectType = NetworkConnectType.ENet;
        NetGame();
    }

    private void OnServer()
    {
        MainManager.NetworkConnectManager.Host();
        MainManager.NetworkManager.NetworkConnectType = NetworkConnectType.ENet;
        NetGame();
    }

    private void OnSignle()
    {
        MainManager.NetworkManager.NetworkConnectType = NetworkConnectType.None;
        Game();
    }

    private void OnSteamServer()
    {
        MainManager.SteamConnectManager.CreateLobby();
        MainManager.NetworkManager.NetworkConnectType = NetworkConnectType.Steam;
        NetGame();
    }

    private void Game()
    {
        MainManager.NetworkManager.NetworkConnectType = NetworkConnectType.None;
        MainManager.Main.CreateGame();
        MainManager.Main.DeleteMenu();
    }

    private void NetGame()
    {
        MainManager.PlayerInfo.Id = Multiplayer.GetUniqueId();

        MainManager.Main.CreateGame();
        MainManager.Main.DeleteMenu();
    }
}
