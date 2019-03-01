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
    class UpdateActivityPointsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.UpdateActivityPoints);
            Message.AppendInt32(valueHolder.GetValue<int>("Points"));
            Message.AppendInt32(0); //change, unused
            Message.AppendInt32(valueHolder.GetValue<int>("ID"));
            return Message;
        }
    }
}
