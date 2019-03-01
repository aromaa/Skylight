using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.HabboHotel.Items;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class BuyInfoMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            CatalogItem item = valueHolder.GetValue<CatalogItem>("Item");
            Tuple<Item, int>[] products = valueHolder.GetValue<Tuple<Item, int>[]>("Products");
            int finalCredits = valueHolder.GetValue<int>("Credits");
            int finalPixels = valueHolder.GetValue<int>("Pixels");

            ServerMessage BuyInfo = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            BuyInfo.Init(r63bOutgoing.BuyInfo);
            BuyInfo.AppendUInt(item.Id);
            BuyInfo.AppendString(item.Name);
            BuyInfo.AppendInt32(finalCredits);
            BuyInfo.AppendInt32(finalPixels);
            BuyInfo.AppendInt32(item.ActivityPointsType);
            BuyInfo.AppendBoolean(true);
            BuyInfo.AppendInt32(products.Length);
            if (item.IsDeal)
            {
                foreach (Tuple<Item, int> data in products)
                {
                    if (data.Item1.Type == "s" || data.Item1.Type == "i")
                    {
                        BuyInfo.AppendString(data.Item1.Type);
                        BuyInfo.AppendInt32(data.Item1.SpriteID);
                        BuyInfo.AppendString(""); //extra data
                        BuyInfo.AppendInt32(data.Item2);
                        BuyInfo.AppendInt32(-1);
                        BuyInfo.AppendBoolean(false); //isLTD
                    }
                    else
                    {
                        throw new Exception("Only normal items are supported for deals");
                    }
                }
            }
            else
            {
                Item item_ = products[0].Item1;

                BuyInfo.AppendString(item_.Type);
                BuyInfo.AppendInt32(item_.SpriteID);
                if (item.Name.Contains("wallpaper_single") || item.Name.Contains("floor_single") || item.Name.Contains("landscape_single"))
                {
                    BuyInfo.AppendString(item.Name.Split('_')[2]); //id
                }
                else
                {
                    BuyInfo.AppendString(""); //extra data, example dics
                }
                BuyInfo.AppendInt32(products[0].Item2);
                BuyInfo.AppendInt32(-1);
                BuyInfo.AppendBoolean(false); //isLTD
            }
            BuyInfo.AppendInt32(0); //club level
            BuyInfo.AppendBoolean(false);
            return BuyInfo;
        }
    }
}
