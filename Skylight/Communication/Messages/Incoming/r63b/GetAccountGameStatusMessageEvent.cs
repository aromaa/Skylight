using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetAccountGameStatusMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int gameId = message.PopWiredInt32();

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message_.Init(r63bOutgoing.AccountGameStatus);
            message_.AppendInt32(gameId);
            message_.AppendInt32(-1); //plays left, below zero means unlimited
            message_.AppendInt32(0); //spome promo stuff, possible boolean
            session.SendMessage(message_);
        }
    }
}
