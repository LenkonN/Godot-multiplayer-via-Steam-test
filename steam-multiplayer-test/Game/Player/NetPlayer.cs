using Godot;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class NetPlayer : Node
{
    [Export] private Player _player { get; set; }
    public long OwnerId { get; set; }
    public CSteamID OwnerSteamId { get; set; }

    public override void _Ready()
    {
        MainManager.NetworkMessageManager.NetworkDataRequested += OnDataRequested;
        MainManager.NetworkMessageManager.NetworkDataGot += OnDataGot;

        WelcomePlayer();
    }

    public override void _ExitTree()
    {
        MainManager.NetworkMessageManager.NetworkDataRequested -= OnDataRequested;
        MainManager.NetworkMessageManager.NetworkDataGot -= OnDataGot;
    }

    // GLOBAL STUFF

    private void OnDataRequested(long idRequester, NetworkMessageType messageType)
    {
        if (messageType is NetworkMessageType.Server_NewPlayerWelcome)
        {
            SendMyPlayerDataToServer();
        }
    }

    private void OnDataGot(long idSender, NetworkMessageType messageType, string json)
    {
        if (messageType is NetworkMessageType.Client_NewPlayerWelcome)
        {
            AddNewPlayerDataOnServer(json, idSender);
        }

        if (messageType is NetworkMessageType.Server_UpdatePlayerDatasOnAllClients)
        {
            GetAllPlayerDatasAndUpdateNicknames(json);
        }
    }

    private void OnAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_iImage <= 0)
            return;
    }

    //SERVER STUFF

    private void WelcomePlayer()
    {
        if (MainManager.NetworkManager.CheckOnlyServerButNotServerObject(OwnerId)) //Only server but not server player object;
        {
            MainManager.NetworkMessageManager.ReqNetData(OwnerId, NetworkMessageType.Server_NewPlayerWelcome);
        }
        else if (MainManager.NetworkManager.CheckIsServer(OwnerId))
        {
            MainManager.NetworkManager.AddPlayerInfo(MainManager.PlayerInfo);
        }
    }

    private void AddNewPlayerDataOnServer(string json, long idSender)
    {
        if (!MainManager.NetworkManager.CheckIsServer(OwnerId)) return;

        PlayerNetData playerNetData = JsonSerializer.Deserialize<PlayerNetData>(json);
        PlayerInfo playerInfo = new PlayerInfo()
        {
            Id = idSender,
            SteamId = SteamUser.GetSteamID(),
            GameName = playerNetData.GameName,
            SteamName = playerNetData.SteamName
        };
        MainManager.NetworkManager.AddPlayerInfo(playerInfo);

        UpdatePlayerDatasOnAllClients();
    }

    private void UpdatePlayerDatasOnAllClients()
    {
        if (!MainManager.NetworkManager.CheckIsServer(OwnerId)) return;

        MultiplePlayerNetData multiplePlayerNetData = new MultiplePlayerNetData();
        multiplePlayerNetData.playerNetDatas = new List<PlayerNetData>();

        foreach (PlayerInfo playerInfo in MainManager.NetworkManager.NetworkPlayers)
        {

            PlayerNetData playerNetData = new PlayerNetData()
            {
                Id = playerInfo.Id,
                SteamId = playerInfo.SteamId.ToString(),
                GameName = playerInfo.GameName,
                SteamName = playerInfo.SteamName
            };

            multiplePlayerNetData.playerNetDatas.Add(playerNetData);
        }

        string json = MainManager.JsonManager.DataToJson(multiplePlayerNetData);

        foreach (int peer in Multiplayer.GetPeers())
        {
            if (peer == 1) // server
            {
                continue;
            }

            MainManager.NetworkMessageManager.SendNetData(peer, NetworkMessageType.Server_UpdatePlayerDatasOnAllClients, json);
        }

        UpdateAllNicknamesOnServerSide();
    }

    private void UpdateAllNicknamesOnServerSide()
    {
        if (!MainManager.NetworkManager.CheckIsServer(OwnerId)) return;

        foreach (Player player in MainManager.GameManager.Players)
        {
            foreach (PlayerInfo playerInfo in MainManager.NetworkManager.NetworkPlayers)
            {
                if (player.NetPlayer.OwnerId == playerInfo.Id)
                {
                    player.SetNicknames(playerInfo.GameName, playerInfo.SteamName);
                }
            }
        }
    }

    //CLIENT STUFF

    private void SendMyPlayerDataToServer()
    {
        if (!MainManager.NetworkManager.CheckMyOwner(OwnerId)) return;

        string json = MainManager.JsonManager.DataToJson(
            new PlayerNetData
            {
                SteamId = SteamUser.GetSteamID().ToString(),
                GameName = MainManager.PlayerInfo.GameName,
                SteamName = MainManager.PlayerInfo.SteamName
            }
            );

        MainManager.NetworkMessageManager.SendNetData(1, NetworkMessageType.Client_NewPlayerWelcome, json);
    }

    // ALL OBJECT STUFF

    private void GetAllPlayerDatasAndUpdateNicknames(string json)
    {
        MultiplePlayerNetData multiplePlayerNetData = JsonSerializer.Deserialize<MultiplePlayerNetData>(json);


        foreach (PlayerNetData playerNetData in multiplePlayerNetData.playerNetDatas)
        {
            if (playerNetData.Id == OwnerId)
            {
                OwnerSteamId = new CSteamID (ulong.Parse(playerNetData.SteamId));
                _player.SetNicknames(playerNetData.GameName, playerNetData.SteamName);
            }
        }
    }
}
