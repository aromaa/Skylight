using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages.NewCrypto;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Handlers
{
    class NewCryptoDataDecoderHandler : DataHandler
    {
        private byte?[] LenghtBytes = null;
        private int? Lenght = null;
        private byte[] Packet = null;
        private int Pointer;

        private readonly Guid Identifier_ = Guid.Parse("d6044348-d972-4fd8-a5d5-f5e8a62a8bb1");

        public override Guid Identifier()
        {
            return this.Identifier_;
        }

        public override bool HandlePacket(GameClient session, ref byte[] packet)
        {
            for (int i = 0; i < packet.Length; i++)
            {
                if (this.ReadByte(packet[i]))
                {
                    NewCryptoClientMessage message = new NewCryptoClientMessage();
                    message.Init((uint)HabboEncoding.DecodeInt16(new byte[] { this.Packet[0], this.Packet[1] }), this.Packet);
                    message.Skip(2);
                    session.HandlePacket(message);

                    Array.Clear(this.Packet, 0, this.Packet.Length);
                    this.Packet = null;
                    this.Lenght = null;
                    this.LenghtBytes = null;
                    this.Pointer = 0;

                    if (session.Disconnected)
                    {
                        return false; //packet disconnected the user! :D
                    }
                }
            }

            return true;
        }

        public bool ReadByte(byte byte_) //returns if packet is fully readed
        {
            if (this.Lenght != null)
            {
                if (this.Packet == null)
                {
                    this.Packet = new byte[(int)this.Lenght];
                }

                this.Packet[this.Pointer++] = byte_;
            }
            else
            {
                if (this.LenghtBytes == null)
                {
                    this.LenghtBytes = new byte?[4] { null, null, null, null };
                }

                int i = 0;
                for (; i < this.LenghtBytes.Length; i++)
                {
                    if (this.LenghtBytes[i] == null)
                    {
                        this.LenghtBytes[i] = byte_;
                        break;
                    }
                }

                if (i == 3)
                {
                    this.Lenght = HabboEncoding.DecodeInt32(new byte[] { (byte)this.LenghtBytes[0], (byte)this.LenghtBytes[1], (byte)this.LenghtBytes[2], (byte)this.LenghtBytes[3] });
                }

                return false;
            }

            return this.Pointer == this.Packet.Length;
        }

        public override bool Equals(object obj)
        {
            if (obj is NewCryptoDataDecoderHandler)
            {
                return ((NewCryptoDataDecoderHandler)obj).Identifier() == this.Identifier();
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Identifier_.GetHashCode();
        }
    }
}
