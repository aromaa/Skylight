using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class RequestDiscountEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendDiscount);
            message_.AppendInt32(100);
            message_.AppendInt32(6);
            message_.AppendInt32(1);
            message_.AppendInt32(1);
            message_.AppendInt32(2);
            message_.AppendInt32(40);
            message_.AppendInt32(99);
        }
    }
}
