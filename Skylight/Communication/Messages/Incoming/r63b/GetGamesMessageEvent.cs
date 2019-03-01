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
    class GetGamesMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message_.Init(r63bOutgoing.Games);
            message_.AppendInt32(1); //count
            message_.AppendInt32(3); //id?
            message_.AppendString("basejump"); //name
            message_.AppendString("68bbd2"); //bacground color
            message_.AppendString(""); //text color
            message_.AppendString("http://localhost/habway/c_images/gamecenter_basejump/");
            session.SendMessage(message_);
        }
    }
}
