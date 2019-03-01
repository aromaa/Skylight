using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class ReceiveWhisperComposerHandler : OutgoingHandler
    {
        public int SenderVirtualID { get; }
        public string Message { get; }
        public int ChatColor;

        public ReceiveWhisperComposerHandler(int senderVirtualId, string message, int chatColor)
        {
            this.SenderVirtualID = senderVirtualId;
            this.Message = message;
            this.ChatColor = chatColor;
        }
    }
}
