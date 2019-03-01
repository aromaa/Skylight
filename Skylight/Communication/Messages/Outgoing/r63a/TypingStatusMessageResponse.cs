using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class TypingStatusMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.TypingStatus);
            message_.AppendInt32(valueHolder.GetValue<int>("VirtualID"));
            message_.AppendBoolean(valueHolder.GetValue<bool>("Typing"));
            return message_;
        }
    }
}
