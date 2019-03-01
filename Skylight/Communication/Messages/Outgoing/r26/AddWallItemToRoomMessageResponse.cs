using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class AddWallItemToRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomItem item = valueHolder.GetValue<RoomItem>("Item");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message.Init(r26Outgoing.AddWallItemToRoom);
            message.AppendString(item.ID.ToString(), 9);
            message.AppendString(item.BaseItem.ItemName, 9);
            message.AppendString(" ", 9);
            message.AppendString(item.WallCoordinate.ToString(), 9);
            message.AppendString(item.ExtraData);
            return message;
        }
    }
}
