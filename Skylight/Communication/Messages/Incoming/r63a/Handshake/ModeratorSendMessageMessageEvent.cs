using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ModeratorSendMessageMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_supporttool"))
            {
                uint userId = message.PopWiredUInt();
                string message_ = message.PopFixedString();
                //string unknown = message.PopFixedString();

                GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
                if (target != null)
                {
                    target.SendNotif(message_);
                }
            }
        }
    }
}
