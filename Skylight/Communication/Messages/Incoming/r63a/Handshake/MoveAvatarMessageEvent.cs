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
    class MoveAvatarMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                RoomUser user = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                if (user != null)
                {
                    int x = message.PopWiredInt32();
                    int y = message.PopWiredInt32();
                    if (user.GetX != x || user.GetY != y)
                    {
                        user.MoveTo(x, y);
                    }
                }
            }
        }
    }
}
