using SkylightEmulator.HabboHotel.GameClients;
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
    class SavePostItDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint itemId = message.PopWiredUInt();
                RoomItem item = room.RoomItemManager.TryGetRoomItem(itemId);
                if (item != null)
                {
                    string data = message.PopFixedString();
                    string[] dataSpllited = data.Split(new char[] { ' '}, 2);

                    string message_ = TextUtilies.FilterString(dataSpllited[1], false, true);
                    if (room.GaveRoomRights(session) || data.StartsWith(item.ExtraData))
                    {
                        string color = dataSpllited[0];
                        if (color == "FFFF33" || color == "FF9CFF" || color == "9CCEFF" || color == "9CFF9C")
                        {
                            item.ExtraData = color + " " + message_;
                            item.UpdateState(true, true);
                        }
                    }
                }
            }
        }
    }
}
