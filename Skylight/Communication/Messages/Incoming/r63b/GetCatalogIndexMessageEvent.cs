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

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetCatalogIndexMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                List<CatalogPage> mainPages = Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(-1, session.GetHabbo().Rank);

                ServerMessage indexes = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                indexes.Init(r63bOutgoing.CatalogIndex);
                indexes.AppendBoolean(true); //visible
                indexes.AppendInt32(0); //color
                indexes.AppendInt32(0); //image
                indexes.AppendInt32(-1); //page id
                indexes.AppendString("root"); //name
                indexes.AppendString("");
                indexes.AppendInt32(mainPages.Count); //count sub indexes

                foreach (CatalogPage page in mainPages)
                {
                    if (page.ParentID == -1)
                    {
                        page.Serialize(indexes);

                        List<CatalogPage> childs = Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(page.PageID, session.GetHabbo().Rank);

                        indexes.AppendInt32(childs.Count);
                        foreach (CatalogPage page2 in childs)
                        {
                            page2.Serialize(indexes);
                            indexes.AppendInt32(0); //we dont expect to have more pages
                        }
                    }
                }

                indexes.AppendBoolean(false); //new
                session.SendMessage(indexes);
            }
        }
    }
}
