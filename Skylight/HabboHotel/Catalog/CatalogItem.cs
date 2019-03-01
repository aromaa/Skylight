using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Catalog
{
    public class CatalogItem
    {
        public uint Id;
        public int PageID;
        public string Name;
        public int CostCredits;
        public int CostActivityPoints;
        public int ActivityPointsType;
        public Tuple<Item, int>[] ProductData;

        public CatalogItem(uint id, string name, string items, int costCredits, int costActivityPoints, int activityPointsType, string amounts, int pageId)
        {
            this.Id = id;
            this.PageID = pageId;
            this.Name = name;
            this.CostCredits = costCredits;
            this.CostActivityPoints = costActivityPoints;
            this.ActivityPointsType = activityPointsType;
            
            string[] splitedItems = items.Split(',');
            string[] splittedAmounts = amounts.Split(',');

            this.ProductData = new Tuple<Item, int>[splitedItems.Length];
            for (int i = 0; i < splitedItems.Length; i++)
            {
                this.ProductData[i] = new Tuple<Item, int>(this.GetItem(uint.Parse(splitedItems[i])), int.Parse(splittedAmounts[i]));
            }
        }

        private Item GetItem(uint itemId)
        {
            return Skylight.GetGame().GetItemManager().TryGetItem(itemId);
        }

        public Tuple<Item, int>[] GetItems()
        {
            return this.ProductData;
        }

        public bool IsDeal
        {
            get
            {
                return this.ProductData.Length > 1;
            }
        }

        public void Serialize(ServerMessage message)
        {
            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                message.AppendUInt(this.Id);
                message.AppendString(this.Name);
                if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
                {
                    message.AppendBoolean(false);
                }

                message.AppendInt32(this.CostCredits);
                message.AppendInt32(this.CostActivityPoints);
                message.AppendInt32(this.ActivityPointsType);
                if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendBoolean(true); //allow gift
                }

                message.AppendInt32(this.ProductData.Length);
                if (this.IsDeal)
                {
                    foreach (Tuple<Item, int> data in this.GetItems())
                    {
                        if (data.Item1.Type == "s" || data.Item1.Type == "i")
                        {
                            message.AppendString(data.Item1.Type);
                            message.AppendInt32(data.Item1.SpriteID);
                            message.AppendString(""); //extra data
                            message.AppendInt32(data.Item2);
                            if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
                            {
                                message.AppendBoolean(false);
                                //message.AppendInt32(0);
                                //message.AppendInt32(0);
                            }
                            else
                            {
                                message.AppendInt32(-1);

                                if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                                {
                                    message.AppendBoolean(false); //isLTD
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Only normal items are supported for deals");
                        }
                    }
                }
                else
                {
                    Item item = this.ProductData[0].Item1;

                    message.AppendString(item.Type);
                    message.AppendInt32(item.SpriteID);
                    if (this.Name.Contains("wallpaper_single") || this.Name.Contains("floor_single") || this.Name.Contains("landscape_single"))
                    {
                        message.AppendString(this.Name.Split('_')[2]); //id
                    }
                    else
                    {
                        message.AppendString(""); //extra data, example dics
                    }
                    message.AppendInt32(this.ProductData[0].Item2);
                    if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
                    {
                        message.AppendBoolean(false);
                        //message.AppendInt32(0);
                        //message.AppendInt32(0);
                    }
                    else
                    {
                        message.AppendInt32(-1);

                        if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                        {
                            message.AppendBoolean(false); //isLTD
                        }
                    }
                }
                message.AppendInt32(0); //vip item
                if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendBoolean(false); //Buy multiple items, get cheaper
                }
            }
            else
            {
                Item item = this.ProductData[0].Item1;

                message.AppendString("p:" + this.Name, 9);
                message.AppendString("", 9);
                message.AppendString(this.CostCredits.ToString(), 9);
                message.AppendString("", 9);
                message.AppendString(item.IsFloorItem ? "s" : "i", 9);
                message.AppendString(item.ItemName, 9);
                message.AppendString("", 9);
                message.AppendString(item.IsFloorItem ? (item.Lenght + "," + item.Width) : "", 9);
                message.AppendString(this.Id.ToString(), 9);
                message.AppendString(item.IsFloorItem ? "0,0,0" : "", 13);
            }
        }
    }
}
