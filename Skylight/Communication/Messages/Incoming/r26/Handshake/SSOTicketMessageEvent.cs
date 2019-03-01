using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class SSOTicketMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo() == null)
            {
                session.LogIn(message.PopFixedString());

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message_.Init(r26Outgoing.SendFurniAliases);
                message_.AppendBoolean(false);
                session.SendMessage(message_);

                ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message_2.Init(r26Outgoing.SpriteIndex);
                session.SendMessage(message_2);
            }
        }
    }
}
