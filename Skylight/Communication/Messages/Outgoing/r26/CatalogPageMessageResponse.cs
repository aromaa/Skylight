using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class CatalogPageMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            CatalogPage page = valueHolder.GetValue<CatalogPage>("CatalogPage");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message.Init(r26Outgoing.CatalogPage);
            message.AppendString("i:" + page.PageID.ToString(), 13);
            message.AppendString("n:" + page.Caption, 13);
            message.AppendString("g:" + page.PageHeadline, 13);
            message.AppendString("h:" + page.PageText1, 13);
            message.AppendString("w:" + page.PageText2, 13);

            switch (page.PageLayout)
            {
                case "frontpage":
                    {
                        message.AppendString("l:ctlg_purse", 13);
                    }
                    break;
                case "default_3x3":
                    {
                        message.AppendString("l:ctlg_layout2", 13);
                    }
                    break;
                default:
                    {
                        message.AppendString("l:" + page.PageLayout, 13);
                    }
                    break;
            }

            foreach(CatalogItem item in page.Items.Values)
            {
                item.Serialize(message);
            }

            return message;
        }
    }
}
