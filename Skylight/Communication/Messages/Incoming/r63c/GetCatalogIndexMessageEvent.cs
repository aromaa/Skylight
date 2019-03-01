using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class GetCatalogIndexMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.CatalogIndexes).Handle(new ValueHolder("Rank", session.GetHabbo().Rank)));
            }
        }
    }
}
