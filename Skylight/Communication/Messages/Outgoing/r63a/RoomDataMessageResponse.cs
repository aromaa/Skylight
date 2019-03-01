using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class RoomDataMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomData roomData = valueHolder.GetValue<RoomData>("RoomData");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.RoomData);
            message.AppendBoolean(valueHolder.GetValueOrDefault<bool>("Entry"));
            roomData.Serialize(message, false);
            message.AppendBoolean(valueHolder.GetValueOrDefault<bool>("Forward"));
            message.AppendBoolean(roomData.IsStaffPick); //is staff pick
            return message;
        }
    }
}
