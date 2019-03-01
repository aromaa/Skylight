using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.Unsupported
{
    public class UnsupportedServerRevision : ServerMessage
    {
        public override void Init(uint id)
        {
            throw new NotImplementedException();
        }

        public override uint GetID()
        {
            throw new NotImplementedException();
        }

        public override int GetLenght()
        {
            throw new NotImplementedException();
        }

        public override void AppendInt32(int i)
        {
            throw new NotImplementedException();
        }

        public override void AppendUInt(uint u)
        {
            throw new NotImplementedException();
        }

        public override void AppendBoolean(bool b)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void AppendBytes(List<byte> bytes)
        {
            throw new NotImplementedException();
        }

        public override void AppendBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override Revision GetRevision()
        {
            throw new NotImplementedException();
        }

        public override void AppendString(string s)
        {
            throw new NotImplementedException();
        }

        public override void AppendString(string s, byte? b)
        {
            throw new NotImplementedException();
        }

        public override void AppendShort(int s)
        {
            throw new NotImplementedException();
        }

        public override string GetHeader()
        {
            throw new NotImplementedException();
        }

        public override string GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
