using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class TradeStartComposer<T> : OutgoingHandlerPacket where T : TradeStartComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.TradeStart);
            foreach(TradeUser trader in handler.Traders)
            {
                message.AppendUInt(trader.UserID);
                message.AppendInt32(1); //can trade
            }
            return message;
        }
    }
}
