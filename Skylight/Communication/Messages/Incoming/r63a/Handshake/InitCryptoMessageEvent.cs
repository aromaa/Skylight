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
    class InitCryptoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            //if (session.SecurityNumber[0] == 0) //should be called once so this shouldn meter
            //{
            if (ServerConfiguration.EnableCrypto)
            {
                int cryptoType = message.PopWiredInt32();
                if (cryptoType == 0 && ServerConfiguration.CryptoType == 0) //banner data requested & server using this type encryption
                {
                    //session.SecurityNumber[0] = 1;
                    //session.SecurityNumber[1] = 1;

                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.Crypto);
                    message_.AppendString(Skylight.GetPublicToken());
                    message_.AppendInt32(0); //not used
                    session.SendMessage(message_);
                }
                else
                {
                    session.Stop("Invalid crypto type");
                }
            }
            else
            {
                if (!ServerConfiguration.RequireMachineID)
                {
                    IncomingPacket incoming;
                    if (BasicUtilies.GetRevisionPacketManager(Revision.RELEASE63_35255_34886_201108111108).HandleIncoming(r63aIncoming.GetSessionParameters, out incoming))
                    {
                        incoming.Handle(session, null);
                    }
                }
                else //REQUIRED EDITED HABBO.SWF
                {
                    //session.SecurityNumber[0] = 1;
                    //session.SecurityNumber[1] = 1;

                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.Crypto);
                    message_.AppendBoolean(false);
                    //message_.AppendString("");
                    //message_.AppendInt32(0); //not used
                    session.SendMessage(message_);
                }
            }
            //}
            //else
            //{
            //    session.Stop("Crypto error");
            //}
        }
    }
}
