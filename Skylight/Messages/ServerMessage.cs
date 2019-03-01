using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages
{
    public abstract class ServerMessage
    {
        public abstract void Init(uint id);
        public abstract uint GetID();
        public abstract int GetLenght();
        public abstract void AppendInt32(int i);
        public abstract void AppendUInt(uint u);
        public abstract void AppendString(string s);
        public abstract void AppendStringWithBreak(string s);
        public abstract void AppendBoolean(bool b);
        public abstract byte[] GetBytes();
    }
}
