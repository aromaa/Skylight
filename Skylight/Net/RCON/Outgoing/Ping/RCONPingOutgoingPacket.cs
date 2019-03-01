using SkylightEmulator.Net.RCON.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Net.RCON.Outgoing.Ping
{
    class RCONPingOutgoingPacket : RCONOutgoingPacket
    {
        public byte[] GetBytes()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    writer.Write((short)-1); //header
                }

                return memoryStream.ToArray();
            }
        }
    }
}
