using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideStartSessionComposerHandler : OutgoingHandler
    {
        public uint RequesterID { get; }
        public string RequesterUsername { get; }
        public string RequesterLook { get; }

        public uint HelperID { get; }
        public string HelperUsername { get; }
        public string HelperLook { get; }

        public GuideStartSessionComposerHandler(uint requesterId, string requesterUsername, string requesterLook, uint helperId, string helperUsername, string helperLook)
        {
            this.RequesterID = requesterId;
            this.RequesterUsername = requesterUsername;
            this.RequesterLook = requesterLook;

            this.HelperID = helperId;
            this.HelperUsername = helperUsername;
            this.HelperLook = helperLook;
        }
    }
}
