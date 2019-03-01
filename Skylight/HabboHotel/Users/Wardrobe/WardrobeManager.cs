using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Wardrobe
{
    public class WardrobeManager
    {
        private uint HabboID;
        private Habbo Habbo;
        private Dictionary<int, WardrobeSlot> WardrobeItems;

        public WardrobeManager(uint id, Habbo habbo, UserDataFactory factory)
        {
            this.WardrobeItems = new Dictionary<int, WardrobeSlot>();
            this.HabboID = id;
            this.Habbo = habbo;
            
            foreach(DataRow dataRow in factory.GetWardrobe()?.Rows)
            {
                int slotId = (int)dataRow["slot_id"];
                this.WardrobeItems.Add(slotId, new WardrobeSlot(slotId, (string)dataRow["gender"], (string)dataRow["look"]));
            }
        }

        public ICollection<WardrobeSlot> GetWardrobeItems()
        {
            return this.WardrobeItems.Values;
        }

        public WardrobeSlot TryGetSlot(int slotID)
        {
            this.WardrobeItems.TryGetValue(slotID, out WardrobeSlot slot);
            return slot;
        }

        public void UpdateSlot(int slotID, string gender, string look)
        {
            WardrobeSlot slot = this.TryGetSlot(slotID);
            if (slot != null)
            {
                slot.Gender = gender;
                slot.Look = look;

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", this.HabboID);
                    dbClient.AddParamWithValue("slotId", slotID);
                    dbClient.AddParamWithValue("gender", gender);
                    dbClient.AddParamWithValue("look", look);

                    dbClient.ExecuteQuery("UPDATE user_wardrobe SET gender = @gender, look = @look WHERE user_id = @userId AND slot_id = @slotId LIMIT 1");
                }
            }
            else
            {
                this.WardrobeItems.Add(slotID, new WardrobeSlot(slotID, gender, look));

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", this.HabboID);
                    dbClient.AddParamWithValue("slotId", slotID);
                    dbClient.AddParamWithValue("gender", gender);
                    dbClient.AddParamWithValue("look", look);

                    dbClient.ExecuteQuery("INSERT INTO user_wardrobe(user_id, slot_id, gender, look) VALUES(@userId, @slotId, @gender, @look)");
                }
            }
        }
    }
}
