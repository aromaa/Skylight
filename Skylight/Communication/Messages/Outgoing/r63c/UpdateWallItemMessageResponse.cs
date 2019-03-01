using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class UpdateWallItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomItem item = valueHolder.GetValue<RoomItem>("Item");

            ServerMessage updateRoomWallItem = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            updateRoomWallItem.Init(r63cOutgoing.UpdateWallItem);
            item.Serialize(updateRoomWallItem);
            return updateRoomWallItem;
        }
    }
}
