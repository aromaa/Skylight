using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class AddFloorItemToRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomItem item = valueHolder.GetValue<RoomItem>("Item");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.AddFloorItemToRoom);
            item.Serialize(message);
            message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(item.UserID));
            return message;
        }
    }
}
