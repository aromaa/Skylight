using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items
{
    public class Item
    {
        public uint ID;
        public int SpriteID;
        public string PublicName;
        public string ItemName;
        public string Type;
        public int Width;
        public int Lenght;
        public double Height;
        public bool Stackable;
        public bool Walkable;
        public bool IsSeat;
        public bool AllowRecycle;
        public bool AllowTrade;
        public bool AllowMarketplaceSell;
        public bool AllowGift;
        public bool AllowInventoryStack;
        public string InteractionType;
        public int InteractionModeCounts;

        public Item(uint id, int spriteId, string publicName, string itemName, string type, int width, int lenght, double height, bool stackable, bool walkable, bool isSeat, bool allowRecycle, bool allowTrade, bool allowMarketplaceSell, bool allowGift, bool allowInventoryStack, string interactionType, int interactionModeCounts)
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
