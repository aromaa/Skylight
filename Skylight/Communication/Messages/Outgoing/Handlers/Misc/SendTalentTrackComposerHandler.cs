using SkylightEmulator.HabboHotel.Talent;
using SkylightEmulator.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendTalentTrackComposerHandler : OutgoingHandler
    {
        public TalentTrack Track;
        public Habbo Habbo;

        public SendTalentTrackComposerHandler(TalentTrack track, Habbo habbo)
        {
            this.Track = track;
            this.Habbo = habbo;
        }
    }
}
