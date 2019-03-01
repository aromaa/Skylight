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
    class CallForHelpMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (!Skylight.GetGame().GetModerationToolManager().HaveOpenSupportTickets(session.GetHabbo().ID))
            {
                string issue = TextUtilies.FilterString(message.PopFixedString());
                message.PopWiredInt32(); //always 1
                int topic = message.PopWiredInt32();
                uint reportedId = message.PopWiredUInt();

                Skylight.GetGame().GetModerationToolManager().CallForHelp(session, issue, topic, reportedId);

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.CallForHelpResult);
                message_.AppendBoolean(false);
                session.SendMessage(message_);
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.CallForHelpResult);
                message_.AppendBoolean(true);
                session.SendMessage(message_);
            }
        }
    }
}
