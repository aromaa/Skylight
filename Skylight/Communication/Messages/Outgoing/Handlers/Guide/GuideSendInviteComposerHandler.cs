using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideSendInviteComposerHandler : OutgoingHandler
    {
        public uint RoomID { get; }
        public string RoomName { get; }

        public GuideSendInviteComposerHandler(uint roomId, string roomName)
        {
            this.RoomID = roomId;
            this.RoomName = roomName;
        }
    }
}
