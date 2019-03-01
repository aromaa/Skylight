using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class SendMsgMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetMessenger() != null)
            {
                uint userId = message.PopWiredUInt();
                string chat = TextUtilies.FilterString(message.PopFixedString());

                if (userId > 0)
                {
                    session.GetHabbo().GetMessenger().SendChatMessage(userId, chat);
                }
                else
                {

                }
            }
        }
    }
}
