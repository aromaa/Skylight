using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.Empty
{
    public class EmptyServerMessage : ServerMessage
    {
        public static readonly EmptyServerMessage instance = new EmptyServerMessage();

        public override void Init()
        {
            throw new NotSupportedException();
        }

        public override void Init(uint id)
        {
            throw new NotSupportedException();
        }

        public override uint GetID()
        {
            throw new NotSupportedException();
        }

        public override int GetLenght()
        {
            throw new NotSupportedException();
        }

        public override Revision GetRevision()
        {
            return Revision.None;
        }

        public override void AppendShort(int s)
        {
            throw new NotSupportedException();
        }

        public override void AppendInt32(int i)
        {
            throw new NotSupportedException();
        }

        public override void AppendUInt(uint u)
        {
            throw new NotSupportedException();
        }

        public override void AppendString(string s)
        {
            throw new NotSupportedException();
        }

        public override void AppendString(string s, byte? b)
        {
            throw new NotSupportedException();
        }

        public override void AppendBoolean(bool b)
        {
            throw new NotSupportedException();
        }

        public override void AppendBytes(List<byte> bytes)
        {
            throw new NotSupportedException();
        }

        public override void AppendBytes(byte[] bytes)
        {
            throw new NotSupportedException();
        }

        public override byte[] GetBytes()
        {
            return null;
        }

        public override string GetHeader()
        {
            return null;
        }

        public override string GetBody()
        {
            return null;
        }
    }
}
