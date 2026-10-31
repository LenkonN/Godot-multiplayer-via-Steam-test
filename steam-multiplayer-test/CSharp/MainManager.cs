using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MainManager
{
    public static Main Main { get; set; }
    public static NetworkConnectManager NetworkConnectManager { get; set; }
    public static NetworkManager NetworkManager { get; set; }
    public static NetworkMessageManager NetworkMessageManager { get; set; }

    public static SteamConnectManager SteamConnectManager { get; set; }

    public static JsonManager JsonManager = new JsonManager();

    public static GameManager GameManager = new GameManager();

    public static PlayerInfo PlayerInfo = new PlayerInfo();
}
