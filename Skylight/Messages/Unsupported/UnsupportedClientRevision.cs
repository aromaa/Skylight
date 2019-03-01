using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.Unsupported
{
    public class UnsupportedClientRevision : ClientMessage
    {
        public override void Init(uint id, byte[] data)
        {
            throw new NotSupportedException();
        }

        public override string PopFixedString()
        {
            throw new NotSupportedException();
        }

        public override int PopWiredInt32()
        {
            throw new NotSupportedException();
        }

        public override int PopFixedInt32()
        {
            throw new NotSupportedException();
        }

        public override uint PopWiredUInt()
        {
            throw new NotSupportedException();
        }

        public override bool PopWiredBoolean()
        {
            throw new NotSupportedException();
        }

        public override bool PopBase64Boolean()
        {
            throw new NotSupportedException();
        }

        public override int GetRemainingLength()
        {
            throw new NotSupportedException();
        }

        public override uint GetID()
        {
            throw new NotSupportedException();
        }

        public override string GetHeader()
        {
            throw new NotSupportedException();
        }

        public override string GetBody()
        {
            throw new NotSupportedException();
        }

        public override byte[] ReadBytes(int bytes)
        {
            throw new NotSupportedException();
        }

        public override string ReadBytesAsString(int bytes)
        {
            throw new NotSupportedException();
        }

        public override int PopBase64Int32()
        {
            throw new NotSupportedException();
        }

        public override string PopStringUntilBreak()
        {
            throw new NotSupportedException();
        }

        public override string PopStringUntilBreak(byte? b)
        {
            throw new NotSupportedException();
        }
    }
}
