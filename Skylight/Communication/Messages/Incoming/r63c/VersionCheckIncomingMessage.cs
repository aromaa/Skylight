using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class VersionCheckIncomingMessage : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string version = message.PopFixedString();
            if (version != "PRODUCTION-201601012205-226667486")
            {
                session.Stop("Wrong swf version");
            }
        }
    }
}
