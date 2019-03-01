using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items
{
    public class Item
    {
        public uint ID { get; private set; }
        public int SpriteID { get; private set; }
        public string PublicName { get; private set; }
        public string ItemName { get; private set; }
        public string Type { get; private set; }
        public int Width { get; private set; }
        public int Lenght { get; private set; }
        public double Height { get; private set; }
        public bool Stackable { get; private set; }
        public bool Walkable { get; private set; }
        public bool IsSeat { get; private set; }
        public bool AllowRecycle { get; private set; }
        public bool AllowTrade { get; private set; }
        public bool AllowMarketplaceSell { get; private set; }
        public bool AllowGift { get; private set; }
        public bool AllowInventoryStack { get; private set; }
        public string InteractionType { get; private set; }
        public int InteractionModeCounts { get; private set; }
        public int[] VendingIds { get; private set; }
        public double[] HeightAdjustable { get; private set; }
        public int EffectM { get; private set; }
        public int EffectF { get; private set; }
        public bool HeightOverride { get; private set; }

        public Item(DataRow dataRow) : this((uint)dataRow["Id"], (int)dataRow["sprite_id"], (string)dataRow["public_name"], (string)dataRow["item_name"], (string)dataRow["type"], (int)dataRow["width"], (int)dataRow["length"], (double)dataRow["stack_height"], TextUtilies.StringToBool(dataRow["can_stack"].ToString()), TextUtilies.StringToBool(dataRow["is_walkable"].ToString()), TextUtilies.StringToBool(dataRow["can_sit"].ToString()), TextUtilies.StringToBool(dataRow["allow_recycle"].ToString()), TextUtilies.StringToBool(dataRow["allow_trade"].ToString()), TextUtilies.StringToBool(dataRow["allow_marketplace_sell"].ToString()), TextUtilies.StringToBool(dataRow["allow_gift"].ToString()), TextUtilies.StringToBool(dataRow["allow_inventory_stack"].ToString()), (string)dataRow["interaction_type"], (int)dataRow["interaction_modes_count"], (string)dataRow["vending_ids"], (string)dataRow["height_adjustable"], (int)dataRow["effectM"], (int)dataRow["effectF"], TextUtilies.StringToBool((string)dataRow["HeightOverride"]))
        {
        }

        public Item(uint id, int spriteId, string publicName, string itemName, string type, int width, int lenght, double height, bool stackable, bool walkable, bool isSeat, bool allowRecycle, bool allowTrade, bool allowMarketplaceSell, bool allowGift, bool allowInventoryStack, string interactionType, int interactionModeCounts, string vendingIds, string heightAdjustable, int effectM, int effectF, bool heightOverride)
        {
            this.SetValues(id, spriteId, publicName, itemName, type, width, lenght, height, stackable, walkable, isSeat, allowRecycle, allowTrade, allowMarketplaceSell, allowGift, allowInventoryStack, interactionType, interactionModeCounts, vendingIds, heightAdjustable, effectM, effectF, heightOverride);
        }

        public void SetValues(uint id, int spriteId, string publicName, string itemName, string type, int width, int lenght, double height, bool stackable, bool walkable, bool isSeat, bool allowRecycle, bool allowTrade, bool allowMarketplaceSell, bool allowGift, bool allowInventoryStack, string interactionType, int interactionModeCounts, string vendingIds, string heightAdjustable, int effectM, int effectF, bool heightOverride)
        {
            this.ID = id;
            this.SpriteID = spriteId;
            this.PublicName = publicName;
            this.ItemName = itemName;
            this.Type = type;
            this.Width = width;
            this.Lenght = lenght;
            this.Height = height;
            this.Stackable = stackable;
            this.Walkable = walkable;
            this.IsSeat = isSeat;
            this.AllowRecycle = allowRecycle;
            this.AllowTrade = allowTrade;
            this.AllowMarketplaceSell = allowMarketplaceSell;
            this.AllowGift = allowGift;
            this.AllowInventoryStack = allowInventoryStack;
            this.InteractionType = interactionType;
            this.InteractionModeCounts = interactionModeCounts;
            this.EffectM = effectF;
            this.EffectF = effectF;
            this.HeightOverride = heightOverride;

            if (!string.IsNullOrWhiteSpace(vendingIds) && vendingIds != "0")
            {
                this.VendingIds = vendingIds.Split(',').Select(i => int.Parse(i)).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(heightAdjustable) && heightAdjustable.Contains(",")) //We want atleast two items
            {
                this.HeightAdjustable = heightAdjustable.Split(',').Select(i => double.Parse(i, CultularUtils.NumberFormatInfo)).ToArray();
            }
        }

        public void SetValues(DataRow dataRow)
        {
            this.SetValues((uint)dataRow["Id"], (int)dataRow["sprite_id"], (string)dataRow["public_name"], (string)dataRow["item_name"], (string)dataRow["type"], (int)dataRow["width"], (int)dataRow["length"], (double)dataRow["stack_height"], TextUtilies.StringToBool(dataRow["can_stack"].ToString()), TextUtilies.StringToBool(dataRow["is_walkable"].ToString()), TextUtilies.StringToBool(dataRow["can_sit"].ToString()), TextUtilies.StringToBool(dataRow["allow_recycle"].ToString()), TextUtilies.StringToBool(dataRow["allow_trade"].ToString()), TextUtilies.StringToBool(dataRow["allow_marketplace_sell"].ToString()), TextUtilies.StringToBool(dataRow["allow_gift"].ToString()), TextUtilies.StringToBool(dataRow["allow_inventory_stack"].ToString()), (string)dataRow["interaction_type"], (int)dataRow["interaction_modes_count"], (string)dataRow["vending_ids"], (string)dataRow["height_adjustable"], (int)dataRow["effectM"], (int)dataRow["effectF"], TextUtilies.StringToBool((string)dataRow["HeightOverride"]));
        }

        public bool IsFloorItem
        {
            get
            {
                return this.Type == "s";
            }
        }

        public bool IsWallItem
        {
            get
            {
                return this.Type == "i";
            }
        }
    }
}
