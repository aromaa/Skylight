using SkylightEmulator.Communication.Handlers;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Cypto;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class SecretKeyMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            //if (session.SecurityNumber[0] == 1)
            //{
            //    string secretKey = message.PopFixedString();

            //    BigInteger sharedKey = HabboCrypto.CalculateDiffieHellmanSharedKey(secretKey);
            //    if (sharedKey != 0)
            //    {
            //        session.SecurityNumber[0] = 2;
            //        session.ARC4 = new ARC4(sharedKey.getBytes());

            //        ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            //        Message.Init(r63aOutgoing.SecretKey);
            //        Message.AppendString(HabboCrypto.DiffieHellman.PublicKey.ToString());
            //        session.SendMessage(Message);
            //    }
            //    else
            //    {
            //        session.Stop("Crypto error");
            //    }
            //}
            //else
            //{
            //    session.Stop("Crypto error");
            //}
        }
    }
}
