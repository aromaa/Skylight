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
    class HomeRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage HomeRoom = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            HomeRoom.Init(r63bOutgoing.HomeRoom);
            HomeRoom.AppendUInt(valueHolder.GetValue<uint>("HomeRoom"));
            HomeRoom.AppendUInt(valueHolder.GetValue<uint>("NewbieRoom"));
            return HomeRoom;
        }
    }
}
