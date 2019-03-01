using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Navigator;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class RoomSearchMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.Navigator);
            message.AppendInt32(1);
            message.AppendString(valueHolder.GetValue<string>("Entry"));

            List<RoomData> rooms = valueHolder.GetValue<List<RoomData>>("Rooms");
            message.AppendInt32(rooms.Count);
            foreach (RoomData roomData in rooms)
            {
                roomData.Serialize(message, false);
            }

            PublicItem publicRoom = valueHolder.GetValue<PublicItem>("PublicRoom");
            if (publicRoom != null)
            {
                message.AppendBoolean(true);
                publicRoom.Serialize(message);
            }
            else
            {
                message.AppendBoolean(false);
            }
            return message;
        }
    }
}
