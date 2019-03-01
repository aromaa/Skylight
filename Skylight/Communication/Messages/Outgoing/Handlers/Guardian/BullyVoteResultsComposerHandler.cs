using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian
{
    public class BullyVoteResultsComposerHandler : OutgoingHandler
    {
        public List<GuardianVote> Votes { get; }

        public BullyVoteResultsComposerHandler(List<GuardianVote> votes)
        {
            this.Votes = votes;
        }
    }
}
