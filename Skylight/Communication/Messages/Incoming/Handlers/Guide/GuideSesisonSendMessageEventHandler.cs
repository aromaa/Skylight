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
    public class GuideSesisonSendMessageEventHandler : IncomingPacket
    {
        protected string Message;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                Skylight.GetGame().GetGuideManager().GetGuideSessionByUserID(session.GetHabbo().ID)?.SendMessage(session.GetHabbo().ID, this.Message);
            }
        }
    }
}
