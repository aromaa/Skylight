using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class GerUserWearingBadgesEventHandler : IncomingPacket
    {
        protected uint UserID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.SendMessage(new SendUserActiveBadgesComposerHandler(Skylight.GetGame().GetUserProfileManager().GetProfile(this.UserID)));
        }
    }
}
