using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class DatabaseUtils
    {
        public static List<uint> CreateItems(uint userId, Item baseItem, string extraData, int amount)
        {
            List<uint> items = new List<uint>();

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", userId);
                dbClient.AddParamWithValue("baseitem", baseItem.ID);
                dbClient.AddParamWithValue("extradata", extraData);

                string runAfterItems = "";
                for(int i = 0; i < amount; i++)
                {
                    uint id = (uint)dbClient.ExecuteQuery("INSERT INTO items(user_id, base_item, extra_data) VALUES (@userid, @baseitem, @extradata)");
                    items.Add(id);

                    switch (baseItem.InteractionType.ToLower())
                    {
                        case "dimmer":
                            {
                                dbClient.AddParamWithValue("itemId_" + i, id);
                                runAfterItems += "INSERT INTO items_moodlight(item_id, enabled, current_preset, preset_one, preset_two, preset_three) VALUES(@itemId_" + i + ", '0', '1', '#000000,255,0', '#000000,255,0', '#000000,255,0'); ";
                            }
                            break;
                        case "teleport":
                            {
                                uint teleportPairId = (uint)dbClient.ExecuteQuery("INSERT INTO items(user_id, base_item, extra_data) VALUES (@userid, @baseitem, @extradata)");
                                items.Add(teleportPairId);

                                dbClient.AddParamWithValue("tele1_" + i, id);
                                dbClient.AddParamWithValue("tele2_" + i, teleportPairId);
                                runAfterItems += "INSERT INTO items_teleports_links(tele_one_id, tele_two_id) VALUES(@tele1_" + i + ", @tele2_" + i + "); ";
                                break;
                            }
                    }
                }

                if (runAfterItems.Length > 0)
                {
                    dbClient.ExecuteQuery(runAfterItems);
                }
            }

            return items;
        }

        public static List<uint> CreatePets(uint userId, string petName, string petRace, string petColor, int petType, double timestamp, int amount)
        {
            List<uint> pets = new List<uint>();
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", userId);
                dbClient.AddParamWithValue("name", petName);
                dbClient.AddParamWithValue("race", petRace);
                dbClient.AddParamWithValue("color", petColor);
                dbClient.AddParamWithValue("type", petType);
                dbClient.AddParamWithValue("timestamp", timestamp);

                for (int i = 0; i < amount; i++)
                {
                    uint petId = (uint)dbClient.ExecuteQuery("INSERT INTO user_pets(user_id, name, race, color, type, create_timestamp, expirience, energy, happiness, respect) VALUES(@userId, @name, @race, @color, @type, @timestamp, 0, 120, 100, 0)");
                    pets.Add(petId);
                }
            }
            return pets;
        }
    }
}
