using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.MUS
{
    public class MUSClientMessage
    {
        private byte[] Data;
        private int Pointer;

        public MUSClientMessage(byte[] data)
        {
            if (data == null)
            {
                data = new byte[0];
            }
            
            this.Data = data;
            this.Pointer = 0;
        }

        public int ReadInt32()
        {
            return (this.Data[this.Pointer++] << 24 | this.Data[this.Pointer++] << 16 | this.Data[this.Pointer++] << 8 | this.Data[this.Pointer++]);
        }

        public short ReadShort()
        {
            return (short)(this.Data[this.Pointer++] << 8 | this.Data[this.Pointer++]);
        }

        public string ReadEvenPaddedString()
        {
            int lenght = this.ReadInt32();
            if (lenght <= 0)
            {
                return "";
            }
            else
            {
                byte[] bytes = this.ReadBytes(lenght);

                if (lenght % 2 != 0)
                {
                    this.ReadByte();
                }

                return Skylight.GetDefaultEncoding().GetString(bytes);
            }
        }

        public int GetRemainingLength()
        {
            return this.Data.Length - this.Pointer;
        }

        public byte ReadByte()
        {
            return this.Data[this.Pointer++];
        }

        public byte[] ReadBytes(int count)
        {
            if (count > this.GetRemainingLength())
            {
                count = this.GetRemainingLength();
            }

            byte[] data = new byte[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = this.Data[this.Pointer++];
            }
            return data;
        }

        public Dictionary<string, MUSPropListItem> ReadPropList()
        {
            Dictionary<string, MUSPropListItem> props = new Dictionary<string, MUSPropListItem>();

            int lenght = this.ReadInt32();

            for(int i = 0; i < lenght; i++)
            {
                this.ReadShort(); //Symbol type, allways string

                string key = this.ReadEvenPaddedString();

                MUSType dataType = (MUSType)this.ReadShort();

                byte[] data = this.ReadBytes(dataType == MUSType.Integer ? 4 : this.ReadInt32());
                if (data.Length % 2 != 0)
                {
                    this.ReadByte();
                }

                props.Add(key, new MUSPropListItem(dataType, data));
            }

            return props;
        }
    }
}
