using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideCloseReasonCodeComposerHandler : OutgoingHandler
    {
        public GuideCloseReasonCode Code;

        public GuideCloseReasonCodeComposerHandler(GuideCloseReasonCode code)
        {
            this.Code = code;
        }
    }
}
