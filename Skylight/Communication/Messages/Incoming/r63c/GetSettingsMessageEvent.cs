using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class GetSettingsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.SendMySettings);
            message_.AppendInt32(100);
            message_.AppendInt32(100);
            message_.AppendInt32(100);
            message_.AppendBoolean(false);
            message_.AppendBoolean(false);
            message_.AppendBoolean(false);
            message_.AppendInt32(1);
            message_.AppendInt32(0); //chat color
            session.SendMessage(message_);
        }
    }
}
