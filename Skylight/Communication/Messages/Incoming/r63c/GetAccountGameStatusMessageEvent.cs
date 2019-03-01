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
    class GetAccountGameStatusMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int gameId = message.PopWiredInt32();

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.AccountGameStatus);
            message_.AppendInt32(gameId);
            message_.AppendInt32(-1); //plays left, below zero means unlimited
            message_.AppendInt32(0); //some promo stuff, possible boolean
            session.SendMessage(message_);
        }
    }
}
