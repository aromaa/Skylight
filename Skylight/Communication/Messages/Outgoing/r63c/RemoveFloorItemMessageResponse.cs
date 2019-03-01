using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class RemoveFloorItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage FloorItemRemoved = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            FloorItemRemoved.Init(r63cOutgoing.RemoveFloorItem);
            FloorItemRemoved.AppendString(valueHolder.GetValue<uint>("ID").ToString());
            FloorItemRemoved.AppendBoolean(false);
            FloorItemRemoved.AppendUInt(valueHolder.GetValue<uint>("UserID"));
            FloorItemRemoved.AppendInt32(0);
            return FloorItemRemoved;
        }
    }
}
