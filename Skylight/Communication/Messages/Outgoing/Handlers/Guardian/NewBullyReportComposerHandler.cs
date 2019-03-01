using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian
{
    public class NewBullyReportComposerHandler : OutgoingHandler
    {
        public int Timeout;

        public NewBullyReportComposerHandler(int timeout)
        {
            this.Timeout = timeout;
        }
    }
}
