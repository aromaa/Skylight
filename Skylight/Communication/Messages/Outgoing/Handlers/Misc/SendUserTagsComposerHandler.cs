using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendUserTagsComposerHandler : OutgoingHandler
    {
        public uint UserID { get; }
        public ICollection<string> Tags { get; }

        public SendUserTagsComposerHandler(uint userId, ICollection<string> tags)
        {
            this.UserID = userId;
            this.Tags = tags;
        }
    }
}
