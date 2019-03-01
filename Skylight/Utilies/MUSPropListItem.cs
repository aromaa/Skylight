using SkylightEmulator.Messages.MUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class MUSPropListItem
    {
        public MUSType Type { get; private set; }
        public byte[] Data { get; private set; }

        public MUSPropListItem(MUSType type, byte[] data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}
