using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian
{
    public class BullyReportCodeComposerHandler : OutgoingHandler
    {
        public BullyReportCode Code;

        public BullyReportCodeComposerHandler(BullyReportCode code)
        {
            this.Code = code;
        }
    }
}
