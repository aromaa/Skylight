using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class AddWallItemToRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomItem item = valueHolder.GetValue<RoomItem>("Item");

            ServerMessage AddWallItemToRoom = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            AddWallItemToRoom.Init(r63aOutgoing.AddWallItemToRoom);
            item.Serialize(AddWallItemToRoom);
            return AddWallItemToRoom;
        }
    }
}
