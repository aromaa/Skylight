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
    class UnseenItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.NewItemAdded);
            message.AppendInt32(1);
            message.AppendInt32(valueHolder.GetValue<int>("Type"));
            message.AppendInt32(1);
            message.AppendInt32(valueHolder.GetValue<int>("ID"));
            return message;
        }
    }
}
