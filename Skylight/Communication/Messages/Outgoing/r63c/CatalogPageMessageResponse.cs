using SkylightEmulator.Communication.Headers;
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
    class CatalogPageMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            CatalogPage page = valueHolder.GetValue<CatalogPage>("CatalogPage");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message.Init(r63cOutgoing.CatalogPage);
            message.AppendInt32(page.PageID);
            message.AppendString("NORMAL");

            switch (page.PageLayout)
            {
                case "frontpage":
                    {
                        message.AppendString("frontpage4");
                        message.AppendInt32(2);
                        message.AppendString(page.PageHeadline);
                        message.AppendString(page.PageTeaser);
                        message.AppendInt32(3);
                        message.AppendString(page.PageText1);
                        message.AppendString(page.PageText2);
                        message.AppendString(page.PageTeaser);
                    }
                    break;
                case "club_buy":
                    {
                        message.AppendString("vip_buy");
                        message.AppendInt32(2);
                        message.AppendString(page.PageHeadline);
                        message.AppendString(page.PageTeaser);
                        message.AppendInt32(0);
                    }
                    break;
                case "pets":
                    {
                        message.AppendString("pets");
                        message.AppendInt32(2);
                        message.AppendString(page.PageHeadline);
                        message.AppendString(page.PageTeaser);
                        message.AppendInt32(4);
                        message.AppendString(page.PageText1);
                        message.AppendString("");
                        message.AppendString("Pick a color:");
                        message.AppendString("Pick a race:");
                    }
                    break;
                case "spaces":
                    {
                        message.AppendString("spaces_new");
                        message.AppendInt32(1);
                        message.AppendString(page.PageHeadline);
                        message.AppendInt32(1);
                        message.AppendString(page.PageText1);
                    }
                    break;
                default:
                    {
                        message.AppendString(page.PageLayout);
                        message.AppendInt32(3);
                        message.AppendString(page.PageHeadline);
                        message.AppendString(page.PageTeaser);
                        message.AppendString(page.PageSpecial);
                        message.AppendInt32(3);
                        message.AppendString(page.PageText1);
                        message.AppendString(page.PageTextDetails);
                        message.AppendString(page.PageTextTeaser);
                    }
                    break;
            }

            message.AppendInt32(page.Items.Count); //items count
            foreach (CatalogItem item in page.Items.Values)
            {
                item.Serialize(message);
            }
            message.AppendInt32(0);
            message.AppendBoolean(false); //acceptSeasonCurrencyAsCredits
            return message;
        }
    }
}
