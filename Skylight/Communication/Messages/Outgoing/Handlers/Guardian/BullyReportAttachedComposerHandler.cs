using SkylightEmulator.HabboHotel.Guardian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian
{
    public class BullyReportAttachedComposerHandler : OutgoingHandler
    {
        public BullyReport Report { get; }

        public BullyReportAttachedComposerHandler(BullyReport report)
        {
            this.Report = report;
        }
    }
}
