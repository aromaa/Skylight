using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items
{
    public class TeleHandler
    {
        //tele and room id
        public static KeyValuePair<uint, uint> GetTeleDestiny(uint teleId)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("teleId", teleId);
                DataRow teleTwoData = dbClient.ReadDataRow("SELECT IF(tele_one_id != @teleId, tele_one_id, tele_two_id) AS tele_id FROM items_teleports_links WHERE tele_one_id = @teleId OR tele_two_id = @teleId LIMIT 1");
                if (teleTwoData != null)
                {
                    uint teleTwoId = (uint)teleTwoData["tele_id"];
                    if (teleTwoId > 0)
                    {
                        dbClient.AddParamWithValue("teleTwoId", teleTwoId);
                        DataRow roomIdData = dbClient.ReadDataRow("SELECT room_id FROM items WHERE id = @teleTwoId LIMIT 1");
                        if (roomIdData != null)
                        {
                            return new KeyValuePair<uint, uint>(teleTwoId, (uint)roomIdData["room_id"]);
                        }
                        else
                        {
                            return new KeyValuePair<uint, uint>(0, 0);
                        }
                    }
                    else
                    {
                        return new KeyValuePair<uint, uint>(0, 0);
                    }
                }
                else
                {
                    return new KeyValuePair<uint, uint>(0, 0);
                }
            }
        }
    }
}
