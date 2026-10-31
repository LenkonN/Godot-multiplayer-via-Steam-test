using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum NetworkMessageType
{
    Server_NewPlayerWelcome, Client_NewPlayerWelcome,
    Server_UpdatePlayerDatasOnAllClients
}

public enum NetworkDataType
{
    Player
}

public enum NetworkConnectType
{
    None, ENet, Steam
}
