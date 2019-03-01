using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Guardian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian
{
    public class BullyReportStartComposerHandler : OutgoingHandler
    {
        public BullyReportStartCode Code { get; }
        public BullyReport Report { get; }

        public BullyReportStartComposerHandler(BullyReportStartCode code, BullyReport report = null)
        {
            this.Code = code;
            this.Report = report;
        }
    }
}
