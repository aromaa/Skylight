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
    class UseFurnitureMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            try
            {
                if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
                {
                    Room room = Skylight.GetGame().GetRoomManager().GetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                    if (room != null)
                    {
                        RoomItem item = room.RoomItemManager.GetRoomItem(message.PopWiredUInt());
                        if (item != null)
                        {
                            item.GetFurniInteractor().OnUse(session, item, message.PopWiredInt32(), room.HaveRights(session));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying use item! " + ex.ToString());
            }
        }
    }
}
