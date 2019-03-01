using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideSessionReceiveMessageComposerHandler : OutgoingHandler
    {
        public uint SenderUserID { get; }
        public string Message { get; }

        public GuideSessionReceiveMessageComposerHandler(uint senderUserId, string message)
        {
            this.SenderUserID = senderUserId;
            this.Message = message;
        }
    }
}
