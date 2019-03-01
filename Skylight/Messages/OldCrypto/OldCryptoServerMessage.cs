using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.OldCrypto
{
    public class OldCryptoServerMessage : ServerMessage
    {
        private uint ID;
        private List<byte> Data;
        private Revision Revision;

        public OldCryptoServerMessage(Revision revision)
        {
            this.Revision = revision;
        }

        public override uint GetID()
        {
            return this.ID;
        }

        public override int GetLenght()
        {
            return this.Data.Count;
        }

        public override Revision GetRevision()
        {
            return this.Revision;
        }

        public override void Init(uint id)
        {
            this.ID = id;
            this.Data = new List<byte>();
        }

        public override void AppendBytes(byte[] data)
        {
            this.Data.AddRange(data);
        }

        public void AppendByte(byte b)
        {
            this.Data.Add(b);
        }

        public override void AppendInt32(int i)
        {
            this.AppendBytes(WireEncoding.EncodeInt32(i));
        }

        public void AppendString(string s, byte? b, Encoding Encoding)
        {
            if (s != null && s.Length != 0)
            {
                this.AppendBytes(Encoding.GetBytes(s));
            }

            if (b != null)
            {
                this.AppendByte((byte)b); //break string
            }
        }

        public override void AppendUInt(uint u)
        {
            this.AppendInt32((int)u);
        }

        public override void AppendString(string s)
        {
            this.AppendString(s, 2);
        }

        public override void AppendString(string s, byte? b)
        {
            this.AppendString(s, b, Skylight.GetDefaultEncoding());
        }

        public override byte[] GetBytes()
        {
            byte[] Data = new byte[this.GetLenght() + 3];
            byte[] Header = Base64Encoding.Encodeuint(this.ID, 2);
            Data[0] = Header[0];
            Data[1] = Header[1];
            for (int i = 0; i < this.GetLenght(); i++)
            {
                Data[i + 2] = this.Data[i];
            }
            Data[Data.Length - 1] = 1;
            return Data;
        }

        //public override void AppendStringWithBreak(string s)
        //{
        //    this.AppendStringWithBreak(s, 2);
        //}

        //public void AppendStringWithBreak(string s, byte BreakChar)
        //{
        //    this.AppendString(s);
        //    this.AppendByte(BreakChar);
        //}

        public override void AppendBoolean(bool Bool)
        {
            if (Bool)
            {
                this.Data.Add(73);
            }
            else
            {
                this.Data.Add(72);
            }
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void AppendBytes(List<byte> bytes)
        {
            this.Data.AddRange(bytes);
        }

        public override void AppendShort(int s)
        {
            throw new NotImplementedException();
        }

        public override string GetHeader()
        {
            return Encoding.Default.GetString(Base64Encoding.Encodeuint(this.ID, 2));
        }

        public override string GetBody()
        {
            return Encoding.Default.GetString(this.Data.ToArray());
        }
    }
}
