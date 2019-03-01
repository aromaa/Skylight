using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63cc
{
    class UpdateActivityPointsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            Message.Init(r63ccOutgoing.UpdateActivityPoints);
            Message.AppendInt32(valueHolder.GetValue<int>("Points"));
            Message.AppendInt32(0); //change, unused
            Message.AppendInt32(valueHolder.GetValue<int>("ID"));
            return Message;
        }
    }
}
