using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class GetCatalogIndexesMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                ServerMessage indexes = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                indexes.Init(r26Outgoing.CatalogIndexes);
                
                foreach (CatalogPage page in Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(-1, session.GetHabbo().Rank))
                {
                    page.Serialize(indexes);

                    foreach (CatalogPage page2 in Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(page.PageID, session.GetHabbo().Rank))
                    {
                        page2.Serialize(indexes);
                    }
                }

                session.SendMessage(indexes);
            }
        }
    }
}
