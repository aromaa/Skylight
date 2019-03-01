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
    class ModeratorReleaseIssueMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_supporttool"))
            {
                int amount = message.PopWiredInt32();
                for(int i = 0; i < amount; i++)
                {
                    uint ticketId = message.PopWiredUInt();
                    SupportTicket ticket = Skylight.GetGame().GetModerationToolManager().TryGetSupportTicket(ticketId);
                    if (ticket != null && ticket.Status == SupportTicketStatus.Picked && ticket.PickerID == session.GetHabbo().ID)
                    {
                        ticket.Release(true);
                        Skylight.GetGame().GetModerationToolManager().SerializeSupportTicketToMods(ticket);
                    }
                }
            }
        }
    }
}
