using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class InitCryptoMessageMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (ServerConfiguration.EnableCrypto)
            {
                int cryptoType = message.PopWiredInt32();
                if (cryptoType == 0 && ServerConfiguration.CryptoType == 0) //banner data requested & server using this type encryption
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                    message_.Init(r63bOutgoing.Crypto);
                    message_.AppendString(Skylight.GetPublicToken());
                    message_.AppendBoolean(true);
                    session.SendMessage(message_);
                }
                else
                {
                    session.Stop("Invalid crypto type");
                }
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                message_.Init(r63bOutgoing.Crypto);
                message_.AppendString(Skylight.GetPublicToken());
                message_.AppendBoolean(false);
                session.SendMessage(message_);
            }
        }
    }
}
