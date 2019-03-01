using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideToolComposerHandler : OutgoingHandler
    {
        public bool OnDuty { get; }
        public int GuidesOnDuty { get; }
        public int HelpersOnDuty { get; }
        public int GuardiansOnDuty { get; }

        public GuideToolComposerHandler(bool onDuty, int guidesOnDuty, int helpersOnDuty, int guardiansOnDuty)
        {
            this.OnDuty = onDuty;
            this.GuidesOnDuty = guidesOnDuty;
            this.HelpersOnDuty = helpersOnDuty;
            this.GuardiansOnDuty = guardiansOnDuty;
        }
    }
}
