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

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetUserClubMembershipMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string clubType = message.PopFixedString();

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.SendClubMembership);
            Message.AppendStringWithBreak(clubType.ToLower());
            Message.AppendInt32(0); //club days left
            Message.AppendInt32(0); //un used
            Message.AppendInt32(1); //club months left
            Message.AppendInt32(0); //response type
            Message.AppendBoolean(false); //unknown bool
            Message.AppendBoolean(false); //is vip
            Message.AppendInt32(0); //hc club gifts(?)
            Message.AppendInt32(0); //vip club gifts(?)
            Message.AppendBoolean(false); //show promo
            Message.AppendInt32(10); //normal price
            Message.AppendInt32(0); //new price
            session.SendMessage(Message);
        }
    }
}
