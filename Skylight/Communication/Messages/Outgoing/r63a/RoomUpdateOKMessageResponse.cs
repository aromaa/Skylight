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
    class RoomUpdateOKMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomUpdateOK = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            roomUpdateOK.Init(r63aOutgoing.RoomUpdateOK);
            roomUpdateOK.AppendUInt(valueHolder.GetValue<uint>("RoomID"));
            return roomUpdateOK;
        }
    }
}
