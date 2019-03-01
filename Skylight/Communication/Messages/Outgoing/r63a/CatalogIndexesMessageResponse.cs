using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class CatalogIndexesMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            int rank = valueHolder.GetValue<int>("Rank");

            List<CatalogPage> mainPages = Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(-1, rank);

            ServerMessage indexes = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            indexes.Init(r63aOutgoing.CatalogIndexes);
            indexes.AppendInt32(1); //visible
            indexes.AppendInt32(0); //color
            indexes.AppendInt32(0); //image
            indexes.AppendInt32(-1); //page id
            indexes.AppendString(""); //name
            indexes.AppendInt32(mainPages.Count); //count sub indexes

            foreach (CatalogPage page in mainPages)
            {
                if (page.ParentID == -1)
                {
                    page.Serialize(indexes);

                    List<CatalogPage> childs = Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(page.PageID, rank);

                    indexes.AppendInt32(childs.Count);
                    foreach (CatalogPage page2 in childs)
                    {
                        page2.Serialize(indexes);
                        indexes.AppendInt32(0); //we dont expect to have more pages
                    }
                }
            }

            indexes.AppendBoolean(false); //new
            return indexes;
        }
    }
}
