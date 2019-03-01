using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class StartTradeMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                FlatCat flatCat = Skylight.GetGame().GetNavigatorManager().GetFlatCat(room.RoomData.Category);
                if (flatCat != null && flatCat.CanTrade)
                {
                    int virtualId = message.PopWiredInt32();
                    RoomUnitUser user = room.RoomUserManager.GetUserByVirtualID(virtualId);
                    if (user != null && user.Session.GetHabbo().GetUserSettings().AcceptTrading)
                    {
                        room.StartTrade(session.GetHabbo().GetRoomSession().GetRoomUser(), user);
                    }
                }
                else
                {
                    session.SendNotif("Trading is disabled in this room!");
                }
            }
        }
    }
}
