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

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class CatalogIndexesMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            int rank = valueHolder.GetValue<int>("Rank");

            List<CatalogPage> mainPages = Skylight.GetGame().GetCatalogManager().GetParentPagesWithRankedSystem(-1, rank);

            ServerMessage indexes = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            indexes.Init(r63cOutgoing.CatalogIndexes);
            indexes.AppendBoolean(true); //visible
            indexes.AppendInt32(0); //icon
            indexes.AppendInt32(-1); //page id
            indexes.AppendString(""); //name
            indexes.AppendString("");
            indexes.AppendInt32(0);
            indexes.AppendInt32(mainPages.Count); //count sub indexes

            foreach (CatalogPage page in mainPages)
            {
                page.Serialize(indexes);
            }

            indexes.AppendBoolean(false); //new
            indexes.AppendString("NORMAL");
            return indexes;
        }
    }
}
