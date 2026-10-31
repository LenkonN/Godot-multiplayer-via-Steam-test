using Godot;
using Godot.Steamworks.Net.Multiplayer.Peer;
using Steamworks;
using System;

public partial class SteamConnectManager : Node
{
    public CSteamID LobbyId { get; private set; }

    private SteamMultiplayerPeer _steamPeer;

    private Callback<LobbyCreated_t> _lobbyCreated;
    private Callback<LobbyEnter_t> _lobbyEntered;
    private Callback<GameLobbyJoinRequested_t> _lobbyJoinRequested;

    private Callback<LobbyChatUpdate_t> _lobbyChatUpdate;

    public override void _Ready()
    {
        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        _lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);

        _lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
        SteamFriends.ActivateGameOverlay("Friends");
        MainManager.NetworkConnectManager.HostSteam();
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        SteamMatchmaking.JoinLobby(lobbyId);
    }

    public void LeaveLobby()
    {
        if (LobbyId.m_SteamID == 0)
            return;

        SteamMatchmaking.LeaveLobby(LobbyId);
        LobbyId = default;
    }

    //Always when player enter, exit, kick and more
    public void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        int memberCount = SteamMatchmaking.GetNumLobbyMembers(LobbyId);

        for (int i = 0; i < memberCount; i++)
        {
            CSteamID memberId = SteamMatchmaking.GetLobbyMemberByIndex(LobbyId, i);
            string memberName = SteamFriends.GetFriendPersonaName(memberId);
        }
    }

    //On host steam lobby
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
    }

    //On new player in lobby
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        CSteamID hostId = SteamMatchmaking.GetLobbyOwner(LobbyId);
        CSteamID localId = SteamUser.GetSteamID();

        if (hostId != localId)
        {
            MainManager.NetworkConnectManager.SteamJoin(hostId);
            

            MainManager.Main.DeleteMenu();
            MainManager.Main.CreateGame();
        }
    }


    //Accept invite and connect from steam (ONLY ACCEPT, NOT CONNECT)
    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        JoinLobby(callback.m_steamIDLobby);
    }

    public override void _ExitTree()
    {
        LeaveLobby();

        _lobbyCreated?.Dispose();
        _lobbyEntered?.Dispose();
        _lobbyJoinRequested?.Dispose();
        _lobbyChatUpdate?.Dispose();
    }
}
