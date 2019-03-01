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

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class GetUserClubMembershipMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage ClubMembership = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            ClubMembership.Init(r26Outgoing.SendClubMembership);
            ClubMembership.AppendString("club_habbo");
            ClubMembership.AppendInt32(1); //end day
            ClubMembership.AppendInt32(0); //passed months
            ClubMembership.AppendInt32(0); //end month
            ClubMembership.AppendBoolean(true);
            session.SendMessage(ClubMembership);
        }
    }
}
