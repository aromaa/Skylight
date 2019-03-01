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
    class RemoveFloorItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.RemoveFloorItem);
            message.AppendString(valueHolder.GetValue<uint>("ID").ToString());
            message.AppendInt32(0);
            message.AppendUInt(valueHolder.GetValue<uint>("UserID"));
            return message;
        }
    }
}
