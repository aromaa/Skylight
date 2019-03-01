using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
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
        //public abstract OutgoingPackets GetOutgouings();
        public abstract bool HandleOutgoing(OutgoingPacketsEnum id, out OutgoingPacket packet);
        public abstract IncomingPacket GetIncoming(uint id);
        public abstract OutgoingPacket GetOutgoing(OutgoingPacketsEnum id);
        public abstract ServerMessage GetNewOutgoing(OutgoingHandler handler);
        public abstract void Clear();
    }
}
