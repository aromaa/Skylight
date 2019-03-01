using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages
{
    public abstract class ClientMessage
    {
        public abstract void Init(uint id, byte[] data);
        public abstract string PopFixedString();
        public abstract string PopStringUntilBreak();
        public abstract string PopStringUntilBreak(byte? b);
        public abstract int PopWiredInt32();
        public abstract int PopFixedInt32();
        public abstract int PopBase64Int32();
        public abstract uint PopWiredUInt();
        public abstract bool PopWiredBoolean();
        public abstract bool PopBase64Boolean();
        public abstract byte[] ReadBytes(int bytes);
        public abstract string ReadBytesAsString(int bytes);
        public abstract int GetRemainingLength();
        public abstract uint GetID();
        public abstract string GetHeader();
        public abstract string GetBody();
    }
}
