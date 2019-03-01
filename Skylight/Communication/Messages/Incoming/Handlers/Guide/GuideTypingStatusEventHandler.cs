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
    public class GuideTypingStatusEventHandler : IncomingPacket
    {
        protected bool Typing;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                Skylight.GetGame().GetGuideManager().GetGuideSessionByUserID(session.GetHabbo().ID)?.SetTyping(session, this.Typing);
            }
        }
    }
}
