using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
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
            message.AppendString(this.GetItem().Type.ToUpper());
            message.AppendUInt(this.ID);
            message.AppendInt32(this.GetItem().SpriteID);
            if (this.GetItem().ItemName.Contains("floor") && this.GetItem().InteractionType == "roomeffect")
            {
                message.AppendInt32(3); //item category
            }
            else if (this.GetItem().ItemName.Contains("wallpaper") && this.GetItem().InteractionType == "roomeffect")
            {
                message.AppendInt32(2); //item category
            }
            else if (this.GetItem().ItemName.Contains("landscape") && this.GetItem().InteractionType == "roomeffect")
            {
                message.AppendInt32(4); //item category
            }
            else
            {
                message.AppendInt32(0); //item category
            }

            if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
            {
                message.AppendInt32(0);
            }
            message.AppendString(this.ExtraData);
            message.AppendBoolean(this.GetItem().AllowRecycle);
            message.AppendBoolean(this.GetItem().AllowTrade);
            message.AppendBoolean(this.GetItem().AllowInventoryStack);
            message.AppendBoolean(this.GetItem().AllowMarketplaceSell);
            message.AppendInt32(-1); //expire time (?)
            if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
            {
                message.AppendBoolean(false); //rent period started
                message.AppendInt32(-1); //flat id
            }

            if (this.GetItem().IsFloorItem)
            {
                if (this.GetItem().ItemName.StartsWith("present_"))
                {
                    string[] data = this.ExtraData.Split((char)9);
                    if (data.Length >= 2)
                    {
                        message.AppendString("!" + data[0] + "\n\n-" + Skylight.GetGame().GetGameClientManager().GetUsernameByID(uint.Parse(data[1]))); //client ignores first char
                        if (this.GetItem().ItemName.StartsWith("present_wrap"))
                        {
                            message.AppendInt32(int.Parse(data[2]) * 1000 + int.Parse(data[3])); //gift style
                        }
                        else
                        {
                            message.AppendInt32(0);
                        }
                    }
                    else
                    {
                        message.AppendString(data[0]);
                        message.AppendInt32(0);
                    }
                }
                else
                {
                    message.AppendString(""); //slot id
                    message.AppendInt32(0);
                }
            }
        }

        public Item GetItem()
        {
            return Skylight.GetGame().GetItemManager().TryGetItem(this.BaseItem);
        }
    }
}
