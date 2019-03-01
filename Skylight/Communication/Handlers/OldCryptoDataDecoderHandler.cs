using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Encryption;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages.OldCrypto;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Handlers
{
    public class OldCryptoDataDecoderHandler : DataHandler
    {
        private byte?[] LenghtBytes = null;
        private int? Lenght = null;
        private byte[] Packet = null;
        private int Pointer;
        private RC4 RC4;

        public static readonly Guid Identifier_ = Guid.Parse("d6044348-d972-4fd8-a5d5-f5e8a62a8bb1");

        public override Guid Identifier()
        {
            return OldCryptoDataDecoderHandler.Identifier_;
        }

        public override bool HandlePacket(GameClient session, ref byte[] packet)
        {
            try
            {
                for (int i = 0; i < packet.Length; i++)
                {
                    if (this.ReadByte(packet[i]))
                    {
                        if (this.RC4 != null)
                        {
                            this.Packet = this.RC4.Decipher(this.Packet);
                        }

                        OldCryptoClientMessage message = new OldCryptoClientMessage();
                        message.Init(Base64Encoding.DecodeUInt32(new byte[] { this.Packet[0], this.Packet[1] }), this.Packet);
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
            catch(Exception ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Exception: " + ex.ToString());
                stringBuilder.AppendLine("- - - DEBUG INFORMATION BELOW - - - ");
                stringBuilder.AppendLine("Lenght bytes:" + string.Join(",", this.LenghtBytes));
                stringBuilder.Append("Lenght bytes as UTF8: ");
                foreach(byte? byte_ in this.LenghtBytes)
                {
                    stringBuilder.Append(Encoding.UTF8.GetString(new byte[1] { (byte)byte_ }));
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Lenght: " + this.Lenght);
                if (this.Packet != null)
                {
                    stringBuilder.AppendLine("Bytes readed: " + string.Join(",", this.Packet));
                    stringBuilder.AppendLine("Bytes readed as UTF8: " + Encoding.UTF8.GetString(this.Packet));
                }
                stringBuilder.AppendLine("Pointer: " + this.Pointer);
                stringBuilder.AppendLine("Reading bytes: " + string.Join(",", packet));
                stringBuilder.AppendLine("Reading bytes as UTF8: " + Encoding.UTF8.GetString(packet));
                Logging.LogPacketException(stringBuilder.ToString());

                session.Stop("OldCryptoDataDecoderHandler failed");
                return false;
            }
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
                    if (this.RC4 == null)
                    {
                        this.LenghtBytes = new byte?[3] { null, null, null };
                    }
                    else
                    {
                        this.LenghtBytes = new byte?[6] { null, null, null, null, null, null };
                    }
                }

                int i = 0;
                for (; i < this.LenghtBytes.Length; i++ )
                {
                    if (this.LenghtBytes[i] == null)
                    {
                        this.LenghtBytes[i] = byte_;
                        break;
                    }
                }

                if (i == this.LenghtBytes.Length - 1)
                {
                    if (this.RC4 == null)
                    {
                        this.Lenght = Base64Encoding.DecodeInt32(this.LenghtBytes.Select(j => (byte)j).ToArray());
                    }
                    else
                    {
                        this.Lenght = Base64Encoding.DecodeInt32(this.RC4.Decipher(this.LenghtBytes.Select(j => (byte)j).ToArray(), false)) * 2;
                    }
                }

                return false;
            }

            return this.Pointer == this.Packet.Length;
        }

        public void SetRC4(RC4 rc4)
        {
            this.RC4 = rc4;
        }

        public override bool Equals(object obj)
        {
            if (obj is OldCryptoDataDecoderHandler)
            {
                return ((OldCryptoDataDecoderHandler)obj).Identifier() == this.Identifier();
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Identifier_.GetHashCode();
        }
    }
}
