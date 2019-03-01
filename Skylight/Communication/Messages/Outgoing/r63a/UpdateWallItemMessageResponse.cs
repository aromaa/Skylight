using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class UpdateWallItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomItem item = valueHolder.GetValue<RoomItem>("Item");

            ServerMessage updateRoomWallItem = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            updateRoomWallItem.Init(r63aOutgoing.UpdateRoomWallItem);
            item.Serialize(updateRoomWallItem);
            return updateRoomWallItem;
        }
    }
}
