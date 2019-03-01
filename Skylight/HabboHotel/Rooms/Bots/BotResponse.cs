using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Bots
{
    public class BotResponse
    {
        public int BotID;
        public List<string> Keywords;
        public string ResponseText;
        public int ResponseMode;
        public int ServerID;

        public BotResponse(int botId, string keywords, string responseText, int responseMode, int serverId)
        {
            this.BotID = botId;
            this.Keywords = keywords.Split(';').ToList();
            this.ResponseText = responseText;
            this.ResponseMode = responseMode;
            this.ServerID = serverId;
        }
    }
}
