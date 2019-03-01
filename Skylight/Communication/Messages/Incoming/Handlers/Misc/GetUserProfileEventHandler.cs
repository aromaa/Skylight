using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class GetUserProfileEventHandler : IncomingPacket
    {
        protected uint UserID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.SendMessage(new SendUserProfileComposerHandler(Skylight.GetGame().GetUserProfileManager().GetProfile(this.UserID), session.GetHabbo().GetMessenger().IsFriendWith(this.UserID), session.GetHabbo().GetMessenger().HasSendedFriendRequestTo(this.UserID)));
        }
    }
}
