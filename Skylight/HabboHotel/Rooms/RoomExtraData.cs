using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomExtraData
    {
        [JsonProperty("blacklisted-cmds")]
        public List<string> BlacklistedCmds = new List<string>();

        [JsonProperty("roomsettings-logic")]
        public List<string> RoomSettingsLogic = new List<string>();

        [JsonProperty("sellroom-price")]
        public Dictionary<string, int> SellRoomPrice = new Dictionary<string,int>();

        [JsonProperty("rp-enabled")]
        public bool RoleplayEnabled = false;
    }
}
