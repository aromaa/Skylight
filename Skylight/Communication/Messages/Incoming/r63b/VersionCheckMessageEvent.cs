using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class VersionCheckMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string version = message.PopFixedString();
            if (version != "RELEASE63-201211141113-913728051")
            {
                session.Stop("Wrong swf version");
            }
        }
    }
}
