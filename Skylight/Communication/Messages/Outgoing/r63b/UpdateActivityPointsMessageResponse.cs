using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class UpdateActivityPointsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            Message.Init(r63bOutgoing.UpdateActivityPoints);
            Message.AppendInt32(valueHolder.GetValue<int>("Points"));
            Message.AppendInt32(0); //change, unused
            Message.AppendInt32(valueHolder.GetValue<int>("ID"));
            return Message;
        }
    }
}
