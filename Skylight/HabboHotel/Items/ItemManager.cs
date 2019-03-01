using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
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
        public Dictionary<uint, List<RoomItem>> NewbieRoomItems;
        private Dictionary<int, Soundtrack> Soundtracks;

        public ItemManager()
        {
            this.Items = new Dictionary<uint, Item>();
            this.NewbieRoomItems = new Dictionary<uint, List<RoomItem>>();
            this.Soundtracks = new Dictionary<int, Soundtrack>();
        }

        public void LoadItems(DatabaseClient dbClient)
        {
            Logging.Write("Loading items... ");

            DataTable items = dbClient.ReadDataTable("SELECT * FROM furniture");
            if (items != null)
            {
                foreach (DataRow dataRow in items.Rows)
                {
                    uint id = (uint)dataRow["id"];

                    Item item;
                    if (this.Items.TryGetValue(id, out item))
                    {
                        item.SetValues(dataRow);
                    }
                    else
                    {
                        this.Items.Add(id, new Item(dataRow));
                    }
                }
            }

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadSoundtracks(DatabaseClient dbClient)
        {
            Logging.Write("Loading soundtracks... ");
            Dictionary<int, Soundtrack> newSoundtracks = new Dictionary<int, Soundtrack>();
            DataTable soundtracks = dbClient.ReadDataTable("SELECT * FROM soundtracks");
            if (soundtracks != null && soundtracks.Rows.Count > 0)
            {
                foreach (DataRow dataRow in soundtracks.Rows)
                {
                    int id = (int)dataRow["id"];
                    newSoundtracks.Add(id, new Soundtrack(id, (string)dataRow["name"], (string)dataRow["author"], (string)dataRow["track"], (int)dataRow["length"]));
                }
            }
            this.Soundtracks = newSoundtracks;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadNewbieRoomItems(DatabaseClient dbClient)
        {
            Logging.Write("Loading newbie room items... ");
            Dictionary<uint, List<RoomItem>> newItems = new Dictionary<uint, List<RoomItem>>();

            DataTable items = dbClient.ReadDataTable("SELECT * FROM items_newbie_room");
            if (items != null)
            {
                foreach (DataRow dataRow in items.Rows)
                {
                    string wallPos = (string)dataRow["wall_pos"];

                    RoomItem item = RoomItem.GetRoomItem((uint)dataRow["Id"], 0, 0, (uint)dataRow["base_item"], (string)dataRow["extra_data"], (int)dataRow["x"], (int)dataRow["y"], (double)dataRow["z"], (int)dataRow["rot"], (string.IsNullOrEmpty(wallPos) ? null : new WallCoordinate(wallPos)), null);
                    if (!newItems.ContainsKey((uint)dataRow["room_id"]))
                    {
                        newItems[(uint)dataRow["room_id"]] = new List<RoomItem>();
                    }

                    newItems[(uint)dataRow["room_id"]].Add(item);
                }
            }

            this.NewbieRoomItems = newItems;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public Item TryGetItem(uint id)
        {
            Item item;
            this.Items.TryGetValue(id, out item);
            return item;
        }

        public Soundtrack TryGetSoundtrack(int id)
        {
            Soundtrack track;
            this.Soundtracks.TryGetValue(id, out track);
            return track;
        }

        public void Shutdown()
        {
            if (this.Items != null)
            {
                this.Items.Clear();
            }
            this.Items = null;

            if (this.Soundtracks != null)
            {
                this.Soundtracks.Clear();
            }
            this.Soundtracks = null;
        }
    }
}
