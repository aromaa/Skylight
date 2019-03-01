using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Handlers
{
    public class PrivacyPolicyRequestHandler : DataHandler
    {
        public static readonly PrivacyPolicyRequestHandler Handler = new PrivacyPolicyRequestHandler();
        private static readonly byte[] XmlPolicy = Encoding.ASCII.GetBytes(CrossdomainPolicy.GetXmlPolicy());
        private readonly Guid Identifier_ = Guid.Parse("e4d3e08b-ca19-43ea-969c-6593c6bf539e");

        public override Guid Identifier()
        {
            return this.Identifier_;
        }

        public override bool HandlePacket(GameClient session, ref byte[] packet)
        {
            session.RemoveDataHandler(this.Identifier()); //we don't need this shit anymore

            if (packet[0] == 60) //it may be privacy policy request
            {
                string data = Encoding.ASCII.GetString(packet); //decode and figure it out
                if (data == "<policy-file-request/>\0") //yes! it is privacy policy request, send response
                {
                    session.SendData(PrivacyPolicyRequestHandler.XmlPolicy);
                    if (!MonoUtils.IsMonoRunning)
                    {
                        session.Stop("Policy privacy file request response");
                    }
                    else
                    {
                        session.ClosePending = true;
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PrivacyPolicyRequestHandler)
            {
                return ((PrivacyPolicyRequestHandler)obj).Identifier() == this.Identifier();
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
