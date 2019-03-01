using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class StartTradeMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.StartTrade);
            message.AppendUInt(valueHolder.GetValue<uint>("UserOneID"));
            message.AppendInt32(1); //Can trade
            message.AppendUInt(valueHolder.GetValue<uint>("UserTwoID"));
            message.AppendInt32(1); //Can trade
            return message;
        }
    }
}
