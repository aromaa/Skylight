using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guide
{
    public class GetGuideToolEventHandler : IncomingPacket
    {
        protected bool OnDuty;
        protected bool TourRequests;
        protected bool HelpRequests;
        protected bool BullyReports;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null && (session.GetHabbo().CanGiveTour || session.GetHabbo().IsHelper || session.GetHabbo().IsGuardian))
            {
                if (this.OnDuty)
                {
                    GuideRequestType type = GuideRequestType.None;
                    if (session.GetHabbo().CanGiveTour && this.TourRequests)
                    {
                        type |= GuideRequestType.Tour;
                    }

                    if (session.GetHabbo().IsHelper && this.HelpRequests)
                    {
                        type |= GuideRequestType.Help;
                    }

                    if (session.GetHabbo().IsGuardian && this.BullyReports)
                    {
                        type |= GuideRequestType.Bully;
                    }

                    Skylight.GetGame().GetGuideManager().SetOnDuty(session, type);
                }
                else
                {
                    Skylight.GetGame().GetGuideManager().RemoveFromDuty(session);
                }
            }
        }
    }
}
