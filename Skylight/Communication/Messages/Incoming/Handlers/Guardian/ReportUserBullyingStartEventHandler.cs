using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Guardian;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guardian
{
    public class ReportUserBullyingStartEventHandler : IncomingPacket
    {
        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo() != null)
            {
                BullyReport report = Skylight.GetGame().GetGuideManager().GetBullyReportByReporterID(session.GetHabbo().ID);
                if (report != null)
                {
                    session.SendMessage(new BullyReportStartComposerHandler(BullyReportStartCode.OngoingCase, report));
                }
                else
                {
                    if (!Skylight.GetGame().GetGuideManager().GetHasBullyReportTimeout(session.GetHabbo().ID))
                    {
                        if (!Skylight.GetGame().GetGuideManager().GetHasBullyReportAbuseTimeout(session.GetHabbo().ID))
                        {
                            session.SendMessage(new BullyReportStartComposerHandler(BullyReportStartCode.Start));
                        }
                        else
                        {
                            session.SendMessage(new BullyReportStartComposerHandler(BullyReportStartCode.Blocked));
                        }
                    }
                    else
                    {
                        session.SendMessage(new BullyReportStartComposerHandler(BullyReportStartCode.TooRecent));
                    }
                }
            }
        }
    }
}
