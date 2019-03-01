using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Inventory
{
    public class InventoryItem
    {
        public uint ID;
        public uint BaseItem;
        public string ExtraData;

        public InventoryItem(uint id, uint baseItem, string extraData)
        {
            this.ID = id;
            this.BaseItem = baseItem;
            this.ExtraData = extraData;
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendUInt(this.ID);
            message.AppendStringWithBreak(this.GetItem().Type.ToUpper());
            message.AppendUInt(this.ID);
            message.AppendInt32(this.GetItem().SpriteID);
            message.AppendInt32(1); // default item
            message.AppendInt32(0);
            message.AppendStringWithBreak(this.ExtraData);
            message.AppendBoolean(false); //allow recycle
            message.AppendBoolean(false); //allow trade
            message.AppendBoolean(true); //inventory stack
            message.AppendBoolean(false); //allow marketplace
            message.AppendInt32(-1);
            if (this.GetItem().IsFloorItem)
            {
                message.AppendStringWithBreak("");
                message.AppendInt32(0);
            }
        }

        public Item GetItem()
        {
            return Skylight.GetGame().GetItemManager().GetItem(this.BaseItem);
        }
    }
}
