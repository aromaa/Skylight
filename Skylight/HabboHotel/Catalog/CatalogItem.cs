using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
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
        public List<uint> Items;
        public int CostCredits;
        public int CostActivityPoints;
        public int ActivityPointsType;
        public int Amount;

        public CatalogItem(uint id, string name, string items, int costCredits, int costActivityPoints, int activityPointsType, int amount, int pageId)
        {
            this.Id = id;
            this.PageID = pageId;
            this.Name = name;

            this.Items = new List<uint>();
            string[] splitedItems = items.Split(',');
            foreach(string item in splitedItems)
            {
                this.Items.Add(uint.Parse(item));
            }

            this.CostCredits = costCredits;
            this.CostActivityPoints = costActivityPoints;
            this.ActivityPointsType = activityPointsType;
            this.Amount = amount;
        }

        public Item GetItem()
        {
            return Skylight.GetGame().GetItemManager().GetItem(this.Items[0]);
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendUInt(this.Id);
            message.AppendStringWithBreak(this.Name);
            message.AppendInt32(this.CostCredits);
            message.AppendInt32(this.CostActivityPoints);
            message.AppendInt32(this.ActivityPointsType);
            message.AppendInt32(1);
            message.AppendStringWithBreak(this.GetItem().Type);
            message.AppendInt32(this.GetItem().SpriteID);
            message.AppendStringWithBreak(""); //extra data, example dics
            message.AppendInt32(this.Amount);
            message.AppendInt32(-1);
            message.AppendInt32(0); //vip item
        }
    }
}
