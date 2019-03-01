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
    class RemoveStatusMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            RoomUnitUser user = session.GetHabbo().GetRoomSession().GetRoomUser();
            if (user != null)
            {
                string type = message.PopStringUntilBreak(null);
                if (type == "CarryItem")
                {
                    user.SetHanditem(0);
                }
                else if (type == "Dance")
                {
                    user.SetDance(0);
                }
            }
        }
    }
}
