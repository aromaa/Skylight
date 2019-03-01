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
    class IsRoomOwnerMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            roomOwner.Init(r63bOutgoing.IsRoomOwner);
            return roomOwner;
        }
    }
}
