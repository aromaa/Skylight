using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerFollowUserComposerHandler : OutgoingHandler
    {
        public uint RoomID { get; }

        public MessengerFollowUserComposerHandler(uint roomId)
        {
            this.RoomID = roomId;
        }
    }
}
