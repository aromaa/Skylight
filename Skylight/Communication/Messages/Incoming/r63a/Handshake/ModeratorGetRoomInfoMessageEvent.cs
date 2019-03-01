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
    class ModeratorGetRoomInfoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_supporttool"))
            {
                uint roomId = message.PopWiredUInt();

                RoomData data = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);
                session.SendMessage(Skylight.GetGame().GetModerationToolManager().SerializeRoomInfo(data));
            }
        }
    }
}
