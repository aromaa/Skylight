using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class MUSUtils
    {
        public static int BytesToInt(byte[] data)
        {
            return (data[3]) | (data[2] << 8) | (data[1] << 16) | (data[0] << 24);
        }

        public static byte[] IntToBytes(int i)
        {
            byte[] data = new byte[4];
            data[0] = (byte)(i >> 24);
            data[1] = (byte)(i >> 16);
            data[2] = (byte)(i >> 8);
            data[3] = (byte)i;
            return data;
        }
    }
}
