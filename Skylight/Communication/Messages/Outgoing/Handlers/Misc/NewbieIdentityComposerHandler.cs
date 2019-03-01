using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class NewbieIdentityComposerHandler : OutgoingHandler
    {
        public bool IsNewbie { get; }

        public NewbieIdentityComposerHandler(bool isNewbie)
        {
            this.IsNewbie = isNewbie;
        }
    }
}
