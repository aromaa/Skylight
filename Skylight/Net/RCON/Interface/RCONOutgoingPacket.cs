using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Net.RCON.Interface
{
    public interface RCONOutgoingPacket
    {
        byte[] GetBytes();
    }
}
