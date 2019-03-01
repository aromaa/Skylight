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

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class SecretKeyMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string secretKey = message.PopFixedString();

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            Message.Init(r63bOutgoing.SecretKey);
            Message.AppendString(Skylight.GetHabboCrypto().PublicKey.ToString());
            session.SendMessage(Message);
        }
    }
}
