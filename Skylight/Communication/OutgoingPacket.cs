using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication
{
    public interface OutgoingPacket
    {
        ServerMessage Handle(ValueHolder valueHolder = null);
    }
}
