using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items
{
    public class ItemManager
    {
        private Dictionary<uint, Item> Items;

        public ItemManager()
        {
            this.Items = new Dictionary<uint, Item>();
        }

        public void LoadItems(DatabaseClient dbClient)
        {
            Logging.Write("Loading items... ");
            this.Items.Clear();

            DataTable items = dbClient.ReadDataTable("SELECT * FROM furniture");
            if (items != null)
            {
                foreach (DataRow dataRow in items.Rows)
                {
                    uint id = (uint)dataRow["id"];
                    this.Items.Add(id, new Item((uint)dataRow["Id"], (int)dataRow["sprite_id"], (string)dataRow["public_name"], (string)dataRow["item_name"], (string)dataRow["type"], (int)dataRow["width"], (int)dataRow["length"], (double)dataRow["stack_height"], TextUtilies.StringToBool(dataRow["can_stack"].ToString()), TextUtilies.StringToBool(dataRow["is_walkable"].ToString()), TextUtilies.StringToBool(dataRow["can_sit"].ToString()), TextUtilies.StringToBool(dataRow["allow_recycle"].ToString()), TextUtilies.StringToBool(dataRow["allow_trade"].ToString()), TextUtilies.StringToBool(dataRow["allow_marketplace_sell"].ToString()), TextUtilies.StringToBool(dataRow["allow_gift"].ToString()), TextUtilies.StringToBool(dataRow["allow_inventory_stack"].ToString()), (string)dataRow["interaction_type"], (int)dataRow["interaction_modes_count"]));
                }
            }
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public Item GetItem(uint id)
        {
            if (this.Items.ContainsKey(id))
            {
                return this.Items[id];
            }
            else
            {
                return null;
            }
        }
    }
}
