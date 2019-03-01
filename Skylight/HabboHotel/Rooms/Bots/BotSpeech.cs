using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Bots
{
    public class BotSpeech
    {
        public int BotID;
        public string Text;
        public bool Shout;

        public BotSpeech(int botId, string text, bool shout)
        {
            this.BotID = botId;
            this.Text = text;
            this.Shout = shout;
        }
    }
}
