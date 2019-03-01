using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class AddWallItemToRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomItem item = valueHolder.GetValue<RoomItem>("Item");

            ServerMessage AddWallItemToRoom = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            AddWallItemToRoom.Init(r63cOutgoing.AddWallItemToRoom);
            item.Serialize(AddWallItemToRoom);
            AddWallItemToRoom.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(item.UserID));
            return AddWallItemToRoom;
        }
    }
}
