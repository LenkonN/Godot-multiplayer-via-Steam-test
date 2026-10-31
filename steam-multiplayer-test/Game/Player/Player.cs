using Godot;
using System;

public partial class Player : CharacterBody2D
{   
    [Export] private Label _gameNickname { get; set; }
    [Export] private Label _steamNickname { get; set; }

    [Export] public NetPlayer NetPlayer { get; set; }

    [Export] public float Speed = 200.0f;

    public override void _Ready()
    {
        SetNicknames(MainManager.PlayerInfo.GameName, MainManager.PlayerInfo.SteamName);

    }

    public void SetNicknames(string gameNick, string steamNick = "")
    {
        _gameNickname.Text = "Game: " + gameNick;
        _steamNickname.Text = "Steam: " + steamNick;
    }

    public void ChangeSprite()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsMultiplayerAuthority())
            return;

        Vector2 direction = Input.GetVector(
            "ui_left",
            "ui_right",
            "ui_up",
            "ui_down"
        );

        Velocity = direction * Speed;
        MoveAndSlide();
    }

}
