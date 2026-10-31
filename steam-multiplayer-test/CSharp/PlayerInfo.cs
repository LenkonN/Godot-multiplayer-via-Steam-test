using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerInfo
{
    public long Id = 0;
    public CSteamID SteamId { get; set; }
    public string GameName = "";
    public string SteamName = "";
}
