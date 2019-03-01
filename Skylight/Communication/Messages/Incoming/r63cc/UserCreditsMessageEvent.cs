using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63cc
{
    class UserCreditsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                session.GetHabbo().UpdateCredits(false);
                session.GetHabbo().UpdateActivityPoints(-1, false);
                session.GetHabbo().SendOnlineUsersCount();
            }
        }
    }
}
