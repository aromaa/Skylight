using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerReceivedRoomInviteComposerHandler : OutgoingHandler
    {
        public uint UserID { get; }
        public string Message { get; }

        public MessengerReceivedRoomInviteComposerHandler(uint userId, string message)
        {
            this.UserID = userId;
            this.Message = message;
        }
    }
}
