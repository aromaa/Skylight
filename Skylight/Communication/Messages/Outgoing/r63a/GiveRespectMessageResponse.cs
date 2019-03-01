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
    class GiveRespectMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.RespectGive);
            message_.AppendUInt(valueHolder.GetValue<uint>("UserID"));
            message_.AppendInt32(valueHolder.GetValue<int>("Total"));
            return message_;
        }
    }
}
