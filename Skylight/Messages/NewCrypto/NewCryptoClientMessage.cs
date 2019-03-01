using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.NewCrypto
{
    class NewCryptoClientMessage : ClientMessage
    {
        private uint ID;
        private byte[] Data;
        private int Pointer;

        public override void Init(uint id, byte[] data)
        {
            if (data == null)
            {
                data = new byte[0];
            }

            this.ID = id;
            this.Data = data;
            this.Pointer = 0;
        }

        public override string PopFixedString()
        {
            return this.PopFixedString(Skylight.GetDefaultEncoding());
        }

        public string PopFixedString(Encoding Encoding)
        {
            return Encoding.GetString(this.ReadFixedValue()).Replace(Convert.ToChar(1), ' ');
        }

        public byte[] ReadFixedValue()
        {
            int len = HabboEncoding.DecodeInt16(this.ReadBytes(2));
            return this.ReadBytes(len);
        }

        public override byte[] ReadBytes(int bytes)
        {
            if (bytes > this.GetRemainingLength())
            {
                bytes = this.GetRemainingLength();
            }

            byte[] data = new byte[bytes];
            for (int i = 0; i < bytes; i++)
            {
                data[i] = this.Data[this.Pointer++];
            }
            return data;
        }

        public override int PopWiredInt32()
        {
            if (this.GetRemainingLength() < 1)
            {
                return 0;
            }
            byte[] Data = this.ReadBytes(4);

            return HabboEncoding.DecodeInt32(Data);
        }

        public override int PopFixedInt32()
        {
            Int32 i = 0;
            Int32.TryParse(this.PopFixedString(Encoding.ASCII), out i);
            return i;
        }

        public override uint PopWiredUInt()
        {
            return (uint)this.PopWiredInt32();
        }

        public override bool PopWiredBoolean()
        {
            if (this.GetRemainingLength() > 0 && this.Data[this.Pointer++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        public override bool PopBase64Boolean()
        {
            throw new NotImplementedException();
        }

        public override int GetRemainingLength()
        {
            return this.Data.Length - this.Pointer;
        }

        public override uint GetID()
        {
            return this.ID;
        }

        public override string GetHeader()
        {
            return Encoding.Default.GetString(Base64Encoding.Encodeuint(this.ID, 2));
        }

        public override string GetBody()
        {
            return Encoding.Default.GetString(this.Data);
        }

        public void Skip(int amount)
        {
            this.Pointer += amount;
        }

        public override string ReadBytesAsString(int bytes)
        {
            return Skylight.GetDefaultEncoding().GetString(this.ReadBytes(bytes));
        }

        public override int PopBase64Int32()
        {
            throw new NotImplementedException();
        }

        public override string PopStringUntilBreak()
        {
            throw new NotImplementedException();
        }

        public override string PopStringUntilBreak(byte? b)
        {
            throw new NotImplementedException();
        }
    }
}
