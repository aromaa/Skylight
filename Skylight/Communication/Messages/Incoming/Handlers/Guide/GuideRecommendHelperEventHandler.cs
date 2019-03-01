using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guide
{
    public class GuideRecommendHelperEventHandler : IncomingPacket
    {
        protected bool Recommend;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                Skylight.GetGame().GetGuideManager().Recommend(session, this.Recommend);
            }
        }
    }
}
