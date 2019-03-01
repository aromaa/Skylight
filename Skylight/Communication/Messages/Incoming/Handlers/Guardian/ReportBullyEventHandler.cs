using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Support;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guardian
{
    public class ReportBullyEventHandler : IncomingPacket
    {
        protected uint UserID;
        protected uint RoomID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                if (!Skylight.GetGame().GetGuideManager().GetHasBullyReportAbuseTimeout(session.GetHabbo().ID))
                {
                    if (Skylight.GetGame().GetGuideManager().GetBullyReportByReportedID(this.UserID) == null)
                    {
                        List<RoomMessage> chatlog = Skylight.GetGame().GetChatlogManager().GetRoomChatlog(this.RoomID).ToList();

                        if (chatlog?.Any(l => l.UserID == this.UserID) ?? false)
                        {
                            Skylight.GetGame().GetGuideManager().SendBullyReport(session, this.UserID, this.RoomID, chatlog);

                            session.SendMessage(new BullyReportCodeComposerHandler(BullyReportCode.Received));
                        }
                        else
                        {
                            session.SendMessage(new BullyReportCodeComposerHandler(BullyReportCode.NoChat));
                        }
                    }
                    else
                    {
                        session.SendMessage(new BullyReportCodeComposerHandler(BullyReportCode.AlreadyReported));
                    }
                }
                else
                {

                    session.SendMessage(new BullyReportCodeComposerHandler(BullyReportCode.Blocked));
                }
            }
        }
    }
}
