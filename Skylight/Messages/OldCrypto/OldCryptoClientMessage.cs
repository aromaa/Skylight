using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.OldCrypto
{
    public class OldCryptoClientMessage : ClientMessage
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
            int len = Base64Encoding.DecodeInt32(this.ReadBytes(2));
            return this.ReadBytes(len);
        }

        public override int PopBase64Int32()
        {
            return Base64Encoding.DecodeInt32(this.ReadBytes(2));
        }

        public override int GetRemainingLength()
        {
            return this.Data.Length - this.Pointer;
        }

        public override bool PopWiredBoolean()
        {
            return this.GetRemainingLength() > 0 && this.Data[this.Pointer++] == 73;
        }

        public override bool PopBase64Boolean()
        {
            return this.GetRemainingLength() > 0 && this.Data[this.Pointer++] == 65;
        }

        public override byte[] ReadBytes(int Bytes)
        {
            if (Bytes > this.GetRemainingLength())
            {
                Bytes = this.GetRemainingLength();
            }

            byte[] data = new byte[Bytes];
            for (int i = 0; i < Bytes; i++)
            {
                data[i] = this.Data[this.Pointer++];
            }
            return data;
        }

        public override uint GetID()
        {
            return this.ID;
        }

        public override string GetHeader()
        {
            return Encoding.Default.GetString(this.Data.Take(2).ToArray());
        }

        public override string GetBody()
        {
            return Encoding.Default.GetString(this.Data.Skip(2).ToArray());
        }

        public byte[] ReadPlainBytes(int bytes)
        {
            if (bytes > this.GetRemainingLength())
            {
                bytes = this.GetRemainingLength();
            }

            byte[] data = new byte[bytes];
            int x = 0;
            int y = this.Pointer;
            while (x < bytes)
            {
                data[x] = this.Data[y];
                x++;
                y++;
            }
            return data;
        }

        public override int PopWiredInt32()
        {
            int i;
            if (this.GetRemainingLength() < 1)
            {
                i = 0;
            }
            else
            {
                byte[] Data = this.ReadPlainBytes(6);
                int TotalBytes = 0;
                int num2 = WireEncoding.DecodeInt32(Data, out TotalBytes);
                this.Pointer += TotalBytes;
                i = num2;
            }
            return i;
        }

        public override int PopFixedInt32()
        {
            int i = 0;
            string s = this.PopFixedString(Encoding.ASCII);
            int.TryParse(s, out i);
            return i;
        }

        public override uint PopWiredUInt()
        {
            return (uint)this.PopWiredInt32();
        }

        public void Skip(int amount)
        {
            this.Pointer += amount;
        }

        public override string ReadBytesAsString(int bytes)
        {
            return Skylight.GetDefaultEncoding().GetString(this.ReadBytes(bytes));
        }

        public byte ReadByte()
        {
            return this.Data[this.Pointer++];
        }

        public override string PopStringUntilBreak()
        {
            return this.PopStringUntilBreak(2);
        }

        public override string PopStringUntilBreak(byte? b)
        {
            if (b != null)
            {
                List<byte> bytes = new List<byte>();
                byte b_;
                while ((b_ = this.ReadByte()) != b)
                {
                    bytes.Add(b_);
                }

                return Skylight.GetDefaultEncoding().GetString(bytes.ToArray());
            }
            else
            {
                return Skylight.GetDefaultEncoding().GetString(this.ReadBytes(this.GetRemainingLength()));
            }
        }
    }
}
