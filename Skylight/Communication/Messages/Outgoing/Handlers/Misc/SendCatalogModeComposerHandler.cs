using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendCatalogModeComposerHandler : OutgoingHandler
    {
        public int Mode { get; }

        public SendCatalogModeComposerHandler(int mode)
        {
            this.Mode = mode;
        }
    }
}
