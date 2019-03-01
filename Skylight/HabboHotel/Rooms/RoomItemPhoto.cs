using Newtonsoft.Json;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemPhoto : RoomItem
    {
        private static Photo Empty = new Photo() { CS = 0, Image = new byte[0], Text = "", Time = 0 };

        public Photo Photo { get; private set; } = RoomItemPhoto.Empty;

        public RoomItemPhoto(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void LoadItemData(string data)
        {
            this.Photo = JsonConvert.DeserializeObject<Photo>(data);
        }


        public override void OnPlace(GameClient session)
        {
            DataRow dataRow = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("itemId", this.ID);
                dataRow = dbClient.ReadDataRow("SELECT data FROM items_data WHERE item_id = @itemId LIMIT 1");
            }

            if (dataRow != null)
            {
                this.Photo = JsonConvert.DeserializeObject<Photo>((string)dataRow["data"]);
            }
        }
    }
}
