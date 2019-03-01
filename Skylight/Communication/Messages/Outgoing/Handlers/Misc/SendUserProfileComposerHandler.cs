using SkylightEmulator.HabboHotel.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendUserProfileComposerHandler : OutgoingHandler
    {
        public UserProfile Profile { get; }
        public bool FriendsWith { get; }
        public bool RequestSended { get; }

        public SendUserProfileComposerHandler(UserProfile profile, bool friendsWith, bool requestSended)
        {
            this.Profile = profile;
            this.FriendsWith = friendsWith;
            this.RequestSended = requestSended;
        }
    }
}
