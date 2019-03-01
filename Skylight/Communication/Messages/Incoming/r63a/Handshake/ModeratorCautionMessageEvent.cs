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
    class ModeratorCautionMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("cmd_alert"))
            {
                uint userId = message.PopWiredUInt();
                string reason = message.PopFixedString();

                GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
                if (target != null)
                {
                    if (target.GetHabbo().Rank < session.GetHabbo().Rank)
                    {
                        Skylight.GetGame().GetCautionManager().GiveCauction(session, target, reason);
                    }
                    else
                    {
                        session.SendNotif("You are not allowed to give caution for this user.");
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
