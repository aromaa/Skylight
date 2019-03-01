using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class MoveWallItemMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                if (room != null)
                {
                    RoomItem item = room.RoomItemManager.TryGetWallItem(message.PopWiredUInt());
                    if (item != null)
                    {
                        if (room.GaveRoomRights(session))
                        {
                            string data = message.PopFixedString();
                            if (room.RoomItemManager.MoveWallItemOnRoom(session, item, new WallCoordinate(data)))
                            {
                                return;
                            }
                        }
                    }


                }
            }
        }
    }
}
