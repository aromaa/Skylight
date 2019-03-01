using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Guide;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guide
{
    public class HandleGuideRequestEventHandler : IncomingPacket
    {
        protected bool Accepted;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
        {
                if (this.Accepted)
                {
                    Skylight.GetGame().GetGuideManager().Accept(session);
                }
                else
                {
                    Skylight.GetGame().GetGuideManager().Decline(session);
                }
            }
        }
    }
}
