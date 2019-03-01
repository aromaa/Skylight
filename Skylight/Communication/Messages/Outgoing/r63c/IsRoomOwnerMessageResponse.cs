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
    class IsRoomOwnerMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomOwner = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            roomOwner.Init(r63cOutgoing.IsRoomOwner);
            return roomOwner;
        }
    }
}
