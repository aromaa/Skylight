using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication
{
    public abstract class PacketManager
    {
        public abstract void Initialize();
        public abstract bool HandleIncoming(uint id, out IncomingPacket packet);
        public abstract bool PacketExits(uint id);
    }
}
