using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ModeratorCloseIssueMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_supporttool"))
            {
                int status = message.PopWiredInt32();
                int amount = message.PopWiredInt32();
                for (int i = 0; i < amount; i++)
                {
                    uint ticketId = message.PopWiredUInt();

                    SupportTicket ticket = Skylight.GetGame().GetModerationToolManager().TryGetSupportTicket(ticketId);
                    if (ticket != null && ticket.Status == SupportTicketStatus.Picked && ticket.PickerID == session.GetHabbo().ID)
                    {
                        switch (status)
                        {
                            case 1: //invalid
                                {
                                    ticket.Close(SupportTicketStatus.Invalid, true);
                                    break;
                                }
                            case 2: //abusive
                                {
                                    ticket.Close(SupportTicketStatus.Abusive, true);
                                    break;
                                }
                            case 3: //resolved
                                {
                                    ticket.Close(SupportTicketStatus.Resolved, true);
                                    break;
                                }
                            default:
                                {
                                    session.SendNotif("Invalid close status! " + status);
                                    return;
                                }
                        }
                        Skylight.GetGame().GetModerationToolManager().SerializeSupportTicketToMods(ticket);

                        GameClient sender = Skylight.GetGame().GetGameClientManager().GetGameClientById(ticket.SenderID);
                        if (sender != null)
                        {
                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.SupportTicketResult);
                            message_.AppendInt32(status);
                            sender.SendMessage(message_);
                        }
                    }
                }
            }
        }
    }
}
