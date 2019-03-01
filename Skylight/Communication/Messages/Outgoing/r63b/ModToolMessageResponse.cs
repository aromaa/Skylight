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

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class ModToolMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            GameClient session = valueHolder.GetValue<GameClient>("Session");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.ModTool);
            message.AppendInt32(Skylight.GetGame().GetModerationToolManager().GetSupportTickets().Count); //amount of tickets
            foreach (SupportTicket ticket in Skylight.GetGame().GetModerationToolManager().GetSupportTickets().Values)
            {
                ticket.Serialize(message);
            }

            message.AppendInt32(Skylight.GetGame().GetModerationToolManager().GetUserPresents().Count); //amount of user preset messages
            foreach (string present in Skylight.GetGame().GetModerationToolManager().GetUserPresents())
            {
                message.AppendString(present);
            }

            message.AppendInt32(Skylight.GetGame().GetModerationToolManager().GetIssues().Count); //amount of message categories
            foreach (KeyValuePair<string, List<ModerationIssue>> issueCategory in Skylight.GetGame().GetModerationToolManager().GetIssues())
            {
                message.AppendString(issueCategory.Key);
                message.AppendBoolean(false); //un-used
                message.AppendInt32(issueCategory.Value.Count);
                foreach (ModerationIssue issue in issueCategory.Value)
                {
                    message.AppendString(issue.Issue);
                    message.AppendString(issue.Solution);
                    message.AppendInt32(0); //ban lenght, 100000 == permament
                    message.AppendInt32(0); //mute lenght
                }
            }

            message.AppendBoolean(session.GetHabbo().HasPermission("acc_supporttool")); //tickets fuse
            message.AppendBoolean(session.GetHabbo().HasPermission("acc_chatlogs")); //chatlog fuse
            message.AppendBoolean(session.GetHabbo().HasPermission("cmd_alert")); //message / caution fuse
            message.AppendBoolean(session.GetHabbo().HasPermission("cmd_kick")); //kick fuse
            message.AppendBoolean(session.GetHabbo().HasPermission("cmd_ban")); //ban fuse
            message.AppendBoolean(session.GetHabbo().HasPermission("cmd_roomalert")); //broadcastshit fuse
            message.AppendBoolean(session.GetHabbo().HasPermission("acc_supporttool")); //other shit fuse

            message.AppendInt32(Skylight.GetGame().GetModerationToolManager().GetRoomPresents().Count); //amount of room preset messages
            foreach (string present in Skylight.GetGame().GetModerationToolManager().GetRoomPresents())
            {
                message.AppendString(present);
            }
            return message;
        }
    }
}
