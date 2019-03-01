using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.NewCrypto;
using SkylightEmulator.Messages.OldCrypto;
using SkylightEmulator.Net;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Handlers
{
    class DetectRevisionHandler : DataHandler
    {
        private static readonly byte[] XmlPolicy = Encoding.ASCII.GetBytes(CrossdomainPolicy.GetXmlPolicy());

        public static readonly Guid Identifier_ = Guid.Parse("dd0d98d8-1bd7-4e20-9285-7105c3ea86ce");
        private int State;

        public override Guid Identifier()
        {
            return DetectRevisionHandler.Identifier_;
        }

        public override bool HandlePacket(GameClient session, ref byte[] packet)
        {
            try
            {
                ClientMessage message = this.TryParseAsOldCrypto(packet);
                if (message is OldCryptoClientMessage)
                {
                    if (this.State == 0 && message.GetID() == r63aIncoming.InitCryptoMessage) //same on r26 and r63a
                    {
                        IncomingPacket packet_ = null;
                        if (BasicUtilies.GetRevisionPacketManager(Revision.RELEASE63_35255_34886_201108111108).HandleIncoming(message.GetID(), out packet_))
                        {
                            packet_.Handle(session, message);
                            this.State = 1;
                        }
                    }
                    else if (this.State == 1 && message.GetID() == r26Outgoing.SecretKey && ServerConfiguration.RequireMachineID)
                    {
                        session.RemoveDataHandler(this.Identifier());
                        session.Revision = Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169;
                    }
                    else if (this.State == 1 && message.GetID() == 204 /* NO CLUE WTF IS THIS */ && !ServerConfiguration.RequireMachineID)
                    {
                        session.RemoveDataHandler(this.Identifier());
                        session.Revision = Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169;
                    }
                    else if (this.State == 1 && message.GetID() == r63aIncoming.Variables && ServerConfiguration.RequireMachineID)
                    {
                        session.RemoveDataHandler(this.Identifier());
                        session.Revision = Revision.RELEASE63_35255_34886_201108111108;
                    }
                    else if (this.State == 1 && message.GetID() == r63aIncoming.SSOTicket && !ServerConfiguration.RequireMachineID)
                    {
                        session.RemoveDataHandler(this.Identifier());
                        session.Revision = Revision.RELEASE63_35255_34886_201108111108;
                    }
                    else
                    {
                        session.Stop("Old crypto, unable to find revision #1");
                    }
                }
                else
                {
                    message = this.TryParseAsNewCrypto(packet);
                    if (message is NewCryptoClientMessage)
                    {
                        if (message.GetID() == r63bIncoming.VersionCheck)
                        {
                            session.RemoveDataHandler(this.Identifier());

                            string version = message.PopFixedString();
                            if (version == "RELEASE63-201211141113-913728051")
                            {
                                session.Revision = Revision.RELEASE63_201211141113_913728051;
                            }
                            else if (version == "PRODUCTION-201601012205-226667486")
                            {
                                session.Revision = Revision.PRODUCTION_201601012205_226667486;
                            }
                            else if (version == "PRODUCTION-201611291003-338511768")
                            {
                                session.Revision = Revision.PRODUCTION_201611291003_338511768;
                            }
                            else
                            {
                                session.Stop("New crypto, revision not supported");
                            }
                        }
                        else
                        {
                            session.Stop("New crypto, unable to find revision");
                        }
                    }
                    else
                    {
                        if (Skylight.ExternalFlashPolicyFileRequestPortEnabled) //If the emulator just booted the policy file may have fallen here, lets just send it
                        {
                            if (packet[0] == 60)
                            {
                                string data = Encoding.ASCII.GetString(packet); //decode and figure it out
                                if (data == "<policy-file-request/>\0") //yes! it is privacy policy request, send response
                                {
                                    session.SendData(DetectRevisionHandler.XmlPolicy);
                                    if (!MonoUtils.IsMonoRunning)
                                    {
                                        session.Stop("Policy privacy file request response");
                                    }
                                    else
                                    {
                                        session.ClosePending = true;
                                    }
                                }
                            }
                        }

                        session.Stop("Unable to revision");
                    }
                }

                return false;
            }
            finally
            {
                if (!session.Disconnected && session.GetDataHandler(DetectRevisionHandler.Identifier_) == null)
                {
                    session.EnableDecodeHandlers();
                    session.HandleData(packet);
                }
            }
        }

        private ClientMessage TryParseAsOldCrypto(byte[] packet)
        {
            try
            {
                int i = 0;
                while (i < packet.Length)
                {
                    int lenght = Base64Encoding.DecodeInt32(new byte[] { (byte)packet[i++], (byte)packet[i++], (byte)packet[i++] });
                    if (lenght > 0)
                    {
                        uint id = Base64Encoding.DecodeUInt32(new byte[] { (byte)packet[i++], (byte)packet[i++] });
                        if (id > 0)
                        {
                            byte[] bytes = new byte[lenght - 2];
                            for (int j = 0; j < bytes.Length; j++)
                            {
                                bytes[j] = packet[i++];
                            }

                            OldCryptoClientMessage crypto = new OldCryptoClientMessage();
                            crypto.Init(id, bytes);
                            return crypto;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private ClientMessage TryParseAsNewCrypto(byte[] packet)
        {
            try
            {
                int i = 0;
                while (i < packet.Length)
                {
                    int lenght = HabboEncoding.DecodeInt32(new byte[] { (byte)packet[i++], (byte)packet[i++], (byte)packet[i++], (byte)packet[i++] });
                    if (lenght > 0)
                    {
                        uint id = (uint)HabboEncoding.DecodeInt16(new byte[] { (byte)packet[i++], (byte)packet[i++] });
                        if (id > 0)
                        {
                            byte[] bytes = new byte[lenght - 2];
                            for (int j = 0; j < bytes.Length; j++)
                            {
                                bytes[j] = packet[i++];
                            }

                            NewCryptoClientMessage crypto = new NewCryptoClientMessage();
                            crypto.Init(id, bytes);
                            return crypto;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is DetectRevisionHandler)
            {
                return ((DetectRevisionHandler)obj).Identifier() == this.Identifier();
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
