using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct PlayerNetData
{
    public long Id { get; set; }
    public string SteamId { get; set; }
    public string GameName { get; set; }
    public string SteamName { get; set; }
}

public struct MultiplePlayerNetData
{
    public List<PlayerNetData> playerNetDatas { get; set; }
}
