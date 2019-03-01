using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class PickupObjectMessageEvent : IncomingPacket
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
