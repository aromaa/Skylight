using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class TradeAcceptedComposer<T> : OutgoingHandlerPacket where T : TradeAcceptedComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.TradeAccepted);
            message.AppendUInt(handler.UserID);
            message.AppendInt32(handler.Accepted ? 1 : 0);
            return message;
        }
    }
}
