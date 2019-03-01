using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class LoginMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() == null)
            {
                string username = message.PopFixedString();
                string password = message.PopFixedString();
                uint userId = message.PopWiredUInt();
                int time = message.PopWiredInt32();

                session.LogIn(username, password, userId, time);
            }
        }
    }
}
