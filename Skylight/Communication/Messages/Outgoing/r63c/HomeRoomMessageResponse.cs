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
    class HomeRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            uint roomId = valueHolder.GetValue<uint>("HomeRoom");

            ServerMessage HomeRoom = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            HomeRoom.Init(r63cOutgoing.HomeRoom);
            HomeRoom.AppendUInt(roomId);
            HomeRoom.AppendUInt(valueHolder.GetValueOrDefault<uint>("ForwardID", roomId));
            return HomeRoom;
        }
    }
}
