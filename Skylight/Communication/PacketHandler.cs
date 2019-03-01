using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication
{
    public abstract class DataHandler
    {
        public abstract Guid Identifier();
        public abstract bool HandlePacket(GameClient session, ref byte[] packet); //return true if we shall continue, false if we should stop
    }
}
