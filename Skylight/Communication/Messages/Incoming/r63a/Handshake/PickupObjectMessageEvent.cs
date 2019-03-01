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
            message.PopWiredInt32();
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = Skylight.GetGame().GetRoomManager().GetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null)
                {
                    RoomItem item = room.RoomItemManager.GetRoomItem(message.PopWiredUInt());
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
                        session.GetHabbo().GetInventoryManager().AddItemToHand(item);
                        session.GetHabbo().GetInventoryManager().UpdateInventory(false);
                    }
                }
            }
        }
    }
}
