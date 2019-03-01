using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.NewCrypto
{
    class NewCryptoServerMessage : ServerMessage
    {
        private uint ID;
        private List<byte> Data;
        private bool ByteHolder;
        private Revision Revision;

        public NewCryptoServerMessage(Revision revision)
        {
            this.Revision = revision;
        }

        public override void Init()
        {
            this.ByteHolder = true;
            this.Data = new List<byte>();
        }

        public override void Init(uint id)
        {
            this.ByteHolder = false;
            this.ID = id;
            this.Data = new List<byte>();

            this.AppendShort((int)id);
        }

        public override uint GetID()
        {
            if (this.ByteHolder)
            {
                throw new NotSupportedException();
            }
            else
            {
                return this.ID;
            }
        }

        public override int GetLenght()
        {
            return this.Data.Count;
        }

        public override Revision GetRevision()
        {
            return this.Revision;
        }

        public override void AppendInt32(int i)
        {
            this.AppendBytes(BitConverter.GetBytes(i), true);
        }

        public void AppendBytes(byte[] b, bool IsInt)
        {
            if (IsInt)
            {
                for (int i = (b.Length - 1); i > -1; i--)
                {
                    this.Data.Add(b[i]);
                }
            }
            else
            {
                this.Data.AddRange(b);
            }
        }

        public override void AppendUInt(uint u)
        {
            this.AppendInt32((int)u);
        }

        public override void AppendString(string s)
        {
            this.AppendString(s, null);
        }

        public override void AppendString(string s, byte? b)
        {
            if (s != null && s.Length > 0)
            {
                this.AppendShort(s.Length);
                this.AppendBytes(Skylight.GetDefaultEncoding().GetBytes(s), false);
            }
            else
            {
                this.AppendShort(0);
            }
        }

        public override void AppendShort(int i)
        {
            Int16 s = (Int16)i;
            this.AppendBytes(BitConverter.GetBytes(s), true);
        }

        public override void AppendBoolean(bool b)
        {
            this.AppendBytes(new byte[] { (byte)(b ? 1 : 0) }, false);
        }

        public override void AppendBytes(List<byte> bytes)
        {
            this.Data.AddRange(bytes);
        }

        public override void AppendBytes(byte[] bytes)
        {
            this.Data.AddRange(bytes);
        }

        public override byte[] GetBytes()
        {
            if (this.ByteHolder)
            {
                return this.Data.ToArray();
            }
            else
            {
                List<byte> Final = new List<byte>();
                Final.AddRange(BitConverter.GetBytes(this.Data.Count)); // packet len
                Final.Reverse();
                Final.AddRange(this.Data); // Add Packet
                return Final.ToArray();
            }
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
