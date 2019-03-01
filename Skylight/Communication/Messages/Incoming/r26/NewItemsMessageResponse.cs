using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Messages.Empty;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class NewItemsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            return new EmptyServerMessage();
        }
    }
}
