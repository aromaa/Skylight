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
    class SetFootballGateDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint itemId = message.PopWiredUInt();
                RoomItem item = room.RoomItemManager.TryGetFloorItem(itemId);
                if (item != null && item is RoomItemFootballGate)
                {
                    RoomItemFootballGate gate = (RoomItemFootballGate)item;

                    string gender = message.PopFixedString();
                    string look = message.PopFixedString();
                    if (gender == "M" || gender == "F")
                    {
                        gate.SetFigure(gender, TextUtilies.FilterString(look));
                    }
                }
            }
        }
    }
}
