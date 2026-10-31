using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class JsonManager
{
    public string DataToJson(PlayerNetData data)
    {
        string json = JsonSerializer.Serialize(data);
        return json;
    }

    public string DataToJson(MultiplePlayerNetData data)
    {
        string json = JsonSerializer.Serialize(data);
        return json;
    }
}
