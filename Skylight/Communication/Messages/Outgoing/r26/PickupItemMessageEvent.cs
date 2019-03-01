using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class PickupItemMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string[] data = message.PopStringUntilBreak(null).Split(' ');

            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                if (room != null && room.HaveOwnerRights(session))
                {
                    uint itemId = uint.Parse(data[2]);

                    RoomItem item = room.RoomItemManager.TryGetRoomItem(itemId);
                    if (item != null)
                    {
                        if (item.IsWallItem)
                        {
                            room.RoomItemManager.PickupWallItemFromRoom(session, item);
                        }
                        else
                        {
                            room.RoomItemManager.PickupFloorItemFromRoom(session, item);
                        }

                        session.GetHabbo().GetInventoryManager().AddRoomItemToHand(item);
                    }
                }
            }
        }
    }
}
