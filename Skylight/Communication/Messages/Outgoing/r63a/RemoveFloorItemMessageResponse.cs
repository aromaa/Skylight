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
    class RemoveFloorItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage FloorItemRemoved = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            FloorItemRemoved.Init(r63aOutgoing.RemoveFloorItemFromRoom);
            FloorItemRemoved.AppendString(valueHolder.GetValue<uint>("ID").ToString());
            FloorItemRemoved.AppendInt32(0);
            return FloorItemRemoved;
        }
    }
}
