using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class CatalogPageMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            CatalogPage page = valueHolder.GetValue<CatalogPage>("CatalogPage");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.CatalogPage);
            message.AppendInt32(page.PageID);

            switch (page.PageLayout)
            {
                case "frontpage":
                    {
                        message.AppendString("frontpage3");
                        message.AppendInt32(2);
                        message.AppendString(page.PageHeadline);
                        message.AppendString(page.PageTeaser);
                        message.AppendInt32(11);
                        message.AppendString(page.PageText1.Split(Convert.ToChar(10))[0]);
                        message.AppendString(page.PageText1.Split(Convert.ToChar(10)).Length > 1 ? page.PageText1.Split(Convert.ToChar(10))[1] : "");
                        message.AppendString(page.PageLinkDescription);
                        message.AppendString(page.PageText2.Split(Convert.ToChar(10))[0]);
                        message.AppendString(TextUtilies.MergeArrayToString(page.PageText2.Split(Convert.ToChar(10)), 1, false));
                        message.AppendString(page.PageTextDetails);
                        message.AppendString(page.PageLinkPagename);
                        message.AppendString("#FAF8CC");
                        message.AppendString("#FAF8CC");
                        message.AppendString("Read More >");
                        message.AppendString("magic.credits");
                    }
                    break;
                case "club_buy":
                    {
                        message.AppendString("club_buy");
                        message.AppendInt32(1);
                        message.AppendString("habboclub_1");
                        message.AppendInt32(1);
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
            message.AppendInt32(-1);
            message.AppendBoolean(false); //new?
            return message;
        }
    }
}
