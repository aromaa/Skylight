using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class StartTradeMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.StartTrade);
            message.AppendUInt(valueHolder.GetValue<uint>("UserOneID"));
            message.AppendInt32(1); //can trade
            message.AppendUInt(valueHolder.GetValue<uint>("UserTwoID"));
            message.AppendInt32(1); //can trade
            return message;
        }
    }
}
