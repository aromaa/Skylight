using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication
{
    public abstract class OutgoingHandlerPacket
    {
        public abstract ServerMessage Handle(OutgoingHandler handler);
    }
}
