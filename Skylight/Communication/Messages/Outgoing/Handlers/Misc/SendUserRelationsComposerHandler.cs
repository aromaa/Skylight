using SkylightEmulator.HabboHotel.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendUserRelationsComposerHandler : OutgoingHandler
    {
        public UserProfile Profile { get; }

        public SendUserRelationsComposerHandler(UserProfile profile)
        {
            this.Profile = profile;
        }
    }
}
