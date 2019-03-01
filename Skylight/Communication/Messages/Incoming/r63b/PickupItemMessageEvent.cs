using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class PickupItemMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                if (room != null && room.HaveOwnerRights(session))
                {
                    int itemType = message.PopWiredInt32();
                    uint itemId = message.PopWiredUInt();

                    RoomItem item = itemType == 2 ? room.RoomItemManager.TryGetFloorItem(itemId) : room.RoomItemManager.TryGetWallItem(itemId);
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
                        //session.GetHabbo().GetInventoryManager().UpdateInventoryItems(false);
                    }
                }
            }
        }
    }
}
