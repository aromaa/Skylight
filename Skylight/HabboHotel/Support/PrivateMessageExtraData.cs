using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class PrivateMessageExtraData
    {
        [JsonProperty("receiver-ids")]
        public List<uint> ReceiverIds = new List<uint>();

        [JsonProperty("receiver-usernames")]
        public List<string> ReceiverUsernames = new List<string>();

        [JsonProperty("receiver-session-ids")]
        public List<int> ReceiverSessionIds = new List<int>();
    }
}
