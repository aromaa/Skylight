using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.MUS
{
    public class MUSServerMessage
    {
        private List<byte> Data;

        public MUSServerMessage(string subject, MUSType type)
        {
            this.Data = new List<byte>();

            this.WriteInt32(0); //error code
            this.WriteInt32(0); //timestamp
            this.WriteEvenPaddedString(subject);
            this.WriteEvenPaddedString("System");

            this.WriteInt32(1); //RECEIVERS AMOUNT
            this.WriteEvenPaddedString("*"); //RECEIVER
            this.WriteShort((short)type);
        }

        public void WriteShort(short value)
        {
            this.Data.Add((byte)(value >> 8));
            this.Data.Add((byte)value);
        }

        public void WriteInt32(int value)
        {
            this.Data.Add((byte)(value >> 24));
            this.Data.Add((byte)(value >> 16));
            this.Data.Add((byte)(value >> 8));
            this.Data.Add((byte)value);
        }

        public void WriteEvenPaddedString(string value)
        {
            byte[] data = Skylight.GetDefaultEncoding().GetBytes(value);

            this.WriteInt32(data.Length);
            this.WriteBytes(data);

            if (data.Length % 2 != 0)
            {
                this.WriteByte(0);
            }
        }

        public void WriteBytes(byte[] data)
        {
            this.Data.AddRange(data);
        }

        public void WriteByte(byte byte_)
        {
            this.Data.Add(byte_);
        }

        public void WritePropList(Dictionary<string, MUSPropListItem> data)
        {
            this.WriteInt32(data.Count);

            foreach(KeyValuePair<string, MUSPropListItem> data_ in data)
            {
                this.WriteShort((short)MUSType.Symbol);
                this.WriteEvenPaddedString(data_.Key);

                this.WriteShort((short)data_.Value.Type);
                if (data_.Value.Type != MUSType.Integer)
                {
                    this.WriteInt32(data_.Value.Data.Length);
                }

                this.WriteBytes(data_.Value.Data);
                if (data_.Value.Data.Length % 2 != 0)
                {
                    this.WriteByte(0);
                }
            }
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[this.Data.Count + 6];
            data[0] = (byte)'r';
            data[1] = 0;
            data[2] = (byte)(this.Data.Count >> 24);
            data[3] = (byte)(this.Data.Count >> 16);
            data[4] = (byte)(this.Data.Count >> 8);
            data[5] = (byte)this.Data.Count;
            for(int i = 0; i < this.Data.Count; i++)
            {
                data[i + 6] = this.Data[i];
            }
            return data;
        }
    }
}
