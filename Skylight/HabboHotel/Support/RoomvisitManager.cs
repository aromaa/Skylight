using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class RoomvisitManager
    {
        private ConcurrentQueue<Roomvisit> RoomvisitsInsertNeeded;
        private ConcurrentDictionary<uint, List<Roomvisit>> Roomvisits;

        public RoomvisitManager()
        {
            this.RoomvisitsInsertNeeded = new ConcurrentQueue<Roomvisit>();
            this.Roomvisits = new ConcurrentDictionary<uint, List<Roomvisit>>();
        }

        public void LogRoomvisit(GameClient session)
        {
            Roomvisit roomvisit = new Roomvisit(session.GetHabbo().ID, session.GetHabbo().GetRoomSession().CurrentRoomID, TimeUtilies.GetUnixTimestamp());
            this.RoomvisitsInsertNeeded.Enqueue(roomvisit);

            List<Roomvisit> roomvisits = null;
            if (!this.Roomvisits.TryGetValue(session.GetHabbo().ID, out roomvisits))
            {
                roomvisits = this.GetRoomvisits(session.GetHabbo().ID);
            }

            if (roomvisits.Count >= 50)
            {
                roomvisits.RemoveAt(0);
            }
            roomvisits.Insert(0, roomvisit);

            this.Roomvisits.AddOrUpdate(session.GetHabbo().ID, roomvisits, (key, oldValue) => roomvisits);
        }

        public List<Roomvisit> GetRoomvisits(uint userId)
        {
            List<Roomvisit> roomvisits = null;
            if (!this.Roomvisits.TryGetValue(userId, out roomvisits))
            {
                //not even loaded
                roomvisits = new List<Roomvisit>();

                DataTable visits = null;
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", userId);
                    visits = dbClient.ReadDataTable("SELECT user_id, room_id, entry_timestamp FROM user_roomvisits ORDER BY entry_timestamp DESC LIMIT 50");
                }

                if (visits != null && visits.Rows.Count > 0)
                {
                    foreach(DataRow dataRow in visits.Rows)
                    {
                        roomvisits.Add(new Roomvisit((uint)dataRow["user_id"], (uint)dataRow["room_id"], (double)dataRow["entry_timestamp"]));
                    }
                }

                this.Roomvisits.TryAdd(userId, roomvisits);
            }

            return roomvisits;
        }

        public void PushRoomvisitsToDB()
        {
            StringBuilder query = new StringBuilder();

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                int i = 0;
                Roomvisit roomvisit;
                while (this.RoomvisitsInsertNeeded.TryDequeue(out roomvisit))
                {
                    i++; //needed so params works correctly

                    dbClient.AddParamWithValue("userId" + i, roomvisit.UserID);
                    dbClient.AddParamWithValue("roomId" + i, roomvisit.RoomID);
                    dbClient.AddParamWithValue("entryTimestamp" + i, roomvisit.EntryTimestamp);

                    query.Append("INSERT INTO user_roomvisits(user_id, room_id, entry_timestamp) VALUES(@userId" + i + ", @roomId" + i + ", @entryTimestamp" + i + "); ");
                }

                if (query.Length > 0)
                {
                    dbClient.ExecuteQuery(query.ToString());
                }
            }
        }

        public void Shutdown()
        {
            if (this.Roomvisits != null)
            {
                this.Roomvisits.Clear();
            }
            this.Roomvisits = null;

            if (this.RoomvisitsInsertNeeded != null)
            {
                this.PushRoomvisitsToDB();
            }
            this.RoomvisitsInsertNeeded = null;
        }
    }
}
