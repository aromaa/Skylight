using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication
{
    public abstract class OutgoingHandler
    {
        public ServerMessage Handle(OutgoingHandlerPacket packet)
        {
            return packet.Handle(this);
        }
    }
}
