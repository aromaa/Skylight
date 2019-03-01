using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63cc
{
    class GetGamesMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            message_.Init(r63ccOutgoing.Games);
            message_.AppendInt32(1); //count
            message_.AppendInt32(3); //id?
            message_.AppendString("basejump"); //name
            message_.AppendString("68bbd2"); //bacground color
            message_.AppendString(""); //text color
            message_.AppendString("http://localhost/habway/c_images/gamecenter_basejump/");
            message_.AppendString(""); //idk
            session.SendMessage(message_);
        }
    }
}
