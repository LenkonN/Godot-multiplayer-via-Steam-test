using Godot;
using System;

public partial class NetworkMessageManager : Node
{
    public event Action<long, NetworkMessageType> NetworkDataRequested;
    public event Action<long, NetworkMessageType, string> NetworkDataGot;

    public void ReqNetData(long peerReqId, NetworkMessageType dataType)
    {
        RpcId(peerReqId, nameof(GetReq), (int)dataType);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void GetReq(int networkMessageType)
    {
        long peerRequester = Multiplayer.GetRemoteSenderId();
        WorkWithReqNetData(peerRequester, (NetworkMessageType)networkMessageType);
    }

    public void SendNetData(long peerSendId, NetworkMessageType networkMessageType, string json)
    {
        RpcId(peerSendId, nameof(GetNetData), (int)networkMessageType, json);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void GetNetData(int networkMessageType, string json)
    {
        long peerSenderId = Multiplayer.GetRemoteSenderId();
        WorkWithGetNetData(peerSenderId, (NetworkMessageType)networkMessageType, json);
    }

    public void WorkWithReqNetData(long peerRequesterId, NetworkMessageType networkMessageType)
    {
        NetworkDataRequested?.Invoke(peerRequesterId, networkMessageType);
    }

    public void WorkWithGetNetData(long peerSenderId, NetworkMessageType networkMessageType, string json)
    {
        NetworkDataGot?.Invoke(peerSenderId, networkMessageType, json);
    }
}
