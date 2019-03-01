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
    class PingMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            session.LastPong.Restart();

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.Ping);
            message_.AppendInt32(message.PopWiredInt32());
            session.SendMessage(message_);
        }
    }
}
