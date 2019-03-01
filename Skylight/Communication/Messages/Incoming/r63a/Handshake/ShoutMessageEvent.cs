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
    class ShoutMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                RoomUser roomUser = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                if (roomUser != null)
                {
                    roomUser.Speak(TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(message.PopFixedString())), true);
                }
            }
        }
    }
}
