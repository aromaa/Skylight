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
    class ModeratorPickIssueMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int amount = message.PopWiredInt32(); //amount
            for (int i = 0; i < amount; i++)
            {
                uint ticketId = message.PopWiredUInt(); //ticket id
                SupportTicket ticket = Skylight.GetGame().GetModerationToolManager().TryGetSupportTicket(ticketId);
                if (ticket != null && ticket.Status == SupportTicketStatus.Open)
                {
                    ticket.Pick(session, true);
                    Skylight.GetGame().GetModerationToolManager().SerializeSupportTicketToMods(ticket);
                }
            }

            //message.PopWiredBoolean(); //retry enabled
            //message.PopWiredInt32(); //retry count
        }
    }
}
