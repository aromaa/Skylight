using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian
{
    public class BullyReportResultsComposerHandler : OutgoingHandler
    {
        public GuardianVote Verdict { get; }
        public GuardianVote YourVote { get; }
        public List<GuardianVote> Votes { get; }

        public BullyReportResultsComposerHandler(GuardianVote verdict, GuardianVote yourVote, List<GuardianVote> votes)
        {
            this.Verdict = verdict;
            this.YourVote = yourVote;
            this.Votes = votes;
        }
    }
}
