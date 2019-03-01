using SkylightEmulator.HabboHotel.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendUserActiveBadgesComposerHandler : OutgoingHandler
    {
        public UserProfile Profile { get; }

        public SendUserActiveBadgesComposerHandler(UserProfile profile)
        {
            this.Profile = profile;
        }
    }
}
