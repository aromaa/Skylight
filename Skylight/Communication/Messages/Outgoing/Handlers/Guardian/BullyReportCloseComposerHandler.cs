using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian
{
    public class BullyReportCloseComposerHandler : OutgoingHandler
    {
        public bool Valid { get; }

        public BullyReportCloseComposerHandler(bool valid)
        {
            this.Valid = valid;
        }
    }
}
