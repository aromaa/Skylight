using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class GetRoomItemsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession().RequestedRoomID > 0 && session.GetHabbo().GetRoomSession().LoadingRoom)
            {
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().RequestedRoomID);
                if (room != null)
                {
                    session.SendData(room.RoomGamemapManager.Model.GetPublicItems(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169), true);

                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    message_.Init(r26Outgoing.FloorItems);
                    message_.AppendInt32(room.RoomItemManager.FloorItems.Count);
                    foreach (RoomItem item in room.RoomItemManager.FloorItems.Values)
                    {
                        item.Serialize(message_);
                    }
                    session.SendMessage(message_);
                }
            }
        }
    }
}
