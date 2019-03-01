using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideSendToRoomComposerHandler : OutgoingHandler
    {
        public uint RoomID { get; }

        public GuideSendToRoomComposerHandler(uint roomId)
        {
            this.RoomID = roomId;
        }
    }
}
