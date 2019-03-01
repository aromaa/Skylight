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
    class RoomErrorOnEnterMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomErrorOnEnter = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            roomErrorOnEnter.Init(r63aOutgoing.RoomErrorOnEnter);
            roomErrorOnEnter.AppendInt32(valueHolder.GetValue<int>("ErrorCode"));
            return roomErrorOnEnter;
        }
    }
}
