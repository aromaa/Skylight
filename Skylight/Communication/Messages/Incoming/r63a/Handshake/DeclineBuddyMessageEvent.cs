using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class DeclineBuddyMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetMessenger() != null)
            {
                if (message.PopWiredInt32() == 0) //1 = none, 0 = multiple
                {
                    int count = message.PopWiredInt32();
                    for(int i = 0; i < count; i++)
                    {
                        session.GetHabbo().GetMessenger().DeclineFriendRequest(message.PopWiredUInt());
                    }
                }
            }
        }
    }
}
