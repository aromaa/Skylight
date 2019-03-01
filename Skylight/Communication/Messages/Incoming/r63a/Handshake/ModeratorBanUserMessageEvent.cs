using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ModeratorBanUserMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("cmd_ban"))
            {
                uint userId = message.PopWiredUInt();
                string reason = message.PopFixedString();
                int lenght = message.PopWiredInt32() * 3600;
                if (lenght == 360000000) //permament
                {
                    lenght = -1;
                }

                GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
                if (target != null)
                {
                    if (target.GetHabbo().Rank < session.GetHabbo().Rank)
                    {
                        Skylight.GetGame().GetBanManager().BanUser(session, target, BanType.User, userId.ToString(), reason, lenght.ToString());
                    }
                    else
                    {
                        session.SendNotif("You are not allowed to ban that user.");
                    }
                }
                else
                {
                    session.SendNotif("User not found.");
                }
            }
        }
    }
}
