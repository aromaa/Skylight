using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class MoveFloorItemMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                if (room != null && room.GaveRoomRights(session))
                {
                    string[] data = message.PopStringUntilBreak(null).Split(' ');

                    RoomItem item = room.RoomItemManager.TryGetFloorItem(uint.Parse(data[0]));
                    if (item != null)
                    {
                        int x = int.Parse(data[1]);
                        int y = int.Parse(data[2]);
                        int rot = int.Parse(data[3]);

                        if (room.RoomItemManager.MoveFloorItemOnRoom(session, item, x, y, rot))
                        {

                        }
                    }
                }
            }
        }
    }
}
