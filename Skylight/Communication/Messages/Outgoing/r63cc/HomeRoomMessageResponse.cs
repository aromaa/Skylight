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
    class HomeRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage HomeRoom = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            HomeRoom.Init(r63ccOutgoing.HomeRoom);
            HomeRoom.AppendUInt(valueHolder.GetValue<uint>("HomeRoom"));
            HomeRoom.AppendUInt(valueHolder.GetValue<uint>("NewbieRoom"));
            return HomeRoom;
        }
    }
}
