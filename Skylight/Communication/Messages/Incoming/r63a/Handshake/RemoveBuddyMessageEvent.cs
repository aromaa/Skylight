using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RemoveBuddyMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetMessenger() != null)
            {
                int count = message.PopWiredInt32();
                for (int i = 0; i < count; i++)
                {
                    session.GetHabbo().GetMessenger().DeleteFriend(message.PopWiredUInt());
                }
            }
        }
    }
}
