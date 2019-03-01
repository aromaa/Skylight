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
    class TypingStatusMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.TypingStatus);
            message.AppendInt32(valueHolder.GetValue<int>("VirtualID"));
            message.AppendInt32(valueHolder.GetValue<bool>("Typing") ? 1 : 0);
            return message;
        }
    }
}
