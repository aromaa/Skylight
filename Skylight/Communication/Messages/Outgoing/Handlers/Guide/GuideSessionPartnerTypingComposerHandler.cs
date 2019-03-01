using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideSessionPartnerTypingComposerHandler : OutgoingHandler
    {
        public bool Typing { get; }

        public GuideSessionPartnerTypingComposerHandler(bool typing)
        {
            this.Typing = typing;
        }
    }
}
