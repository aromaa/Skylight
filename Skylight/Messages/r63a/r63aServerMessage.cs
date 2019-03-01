using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.r63a
{
    public class r63aServerMessage : ServerMessage
    {
        private uint ID;
        private List<byte> Data;

        public override uint GetID()
        {
            return this.ID;
        }

        public override int GetLenght()
        {
            return this.Data.Count;
        }

        public override void Init(uint id)
        {
            this.ID = id;
            this.Data = new List<byte>();
        }

        public void AppendBytes(byte[] data)
        {
            if (data != null && data.Length != 0)
            {
                this.Data.AddRange(data);
            }
        }

        public void AppendByte(byte b)
        {
            this.Data.Add(b);
        }

        public override void AppendInt32(int i)
        {
            this.AppendBytes(WireEncoding.EncodeInt32(i));
        }

        public void AppendString(string s, Encoding Encoding)
        {
            if (s != null && s.Length != 0)
            {
                this.AppendBytes(Encoding.GetBytes(s));
            }
        }

        public override void AppendUInt(uint u)
        {
            this.AppendInt32((int)u);
        }

        public override void AppendString(string s)
        {
            this.AppendString(s, Skylight.GetDefaultEncoding());
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

        public override void AppendStringWithBreak(string s)
        {
            this.AppendStringWithBreak(s, 2);
        }

        public void AppendStringWithBreak(string s, byte BreakChar)
        {
            this.AppendString(s);
            this.AppendByte(BreakChar);
        }

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
    }
}
