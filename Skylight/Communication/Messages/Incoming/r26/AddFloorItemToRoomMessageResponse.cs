using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class AddFloorItemToRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message.Init(r26Outgoing.PlaceFloorItem);
            valueHolder.GetValue<RoomItem>("Item").Serialize(message);
            return message;
        }
    }
}
