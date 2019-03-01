using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
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
    public class RoomManager
    {
        public Dictionary<uint, RoomData> LoadedRoomData;
        public Dictionary<string, RoomModel> RoomModels;
        public Dictionary<uint, Room> LoadedRooms;

        public Task RoomCycleTask;
        public DateTime LastRoomCycleExecution;

        public RoomManager()
        {
            this.RoomModels = new Dictionary<string, RoomModel>();
            this.LoadedRoomData = new Dictionary<uint, RoomData>();
            this.LoadedRooms = new Dictionary<uint, Room>();
        }

        public void LoadRoomModels(DatabaseClient dbClient)
        {
            Logging.Write("Loading room models... ", ConsoleColor.White);
            this.RoomModels.Clear();

            DataTable models = dbClient.ReadDataTable("SELECT * FROM room_models");
            if (models != null)
            {
                foreach(DataRow dataRow in models.Rows)
                {
                    string id = (string)dataRow["id"];
                    this.RoomModels.Add(id, new RoomModel(id, (int)dataRow["door_x"], (int)dataRow["door_y"], (int)dataRow["door_z"], (int)dataRow["door_dir"], (string)dataRow["heightmap"], (string)dataRow["public_items"], (string)dataRow["club_name"]));
                }
            }
            Logging.WriteLine("completed! ", ConsoleColor.Green);
        }

        public RoomData LoadRoomData(uint id)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("roomid", id);
                DataRow room = dbClient.ReadDataRow("SELECT * FROM rooms WHERE id = @roomid LIMIT 1");

                RoomData roomData = new RoomData(room);
                this.LoadedRoomData.Add(id, roomData);
                return roomData;
            }
        }

        public RoomData GetRoomData(uint id)
        {
            if (this.LoadedRoomData.ContainsKey(id))
            {
                return this.LoadedRoomData[id];
            }
            else
            {
                return this.LoadRoomData(id);
            }
        }

        public RoomData CreateRoom(GameClient gameClient, string roomName, string roomModel)
        {
            roomName = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(roomName));
            if (this.RoomModels.ContainsKey(roomModel))
            {
                string clubName = this.RoomModels[roomModel].ClubName;
                if (string.IsNullOrEmpty(clubName)) //hc check
                {
                    if (roomName.Length >= 3)
                    {
                        uint roomId = 0;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("name", roomName);
                            dbClient.AddParamWithValue("model", roomModel);
                            dbClient.AddParamWithValue("ownerid", gameClient.GetHabbo().ID);
                            dbClient.ExecuteQuery("INSERT INTO rooms(type, name, model, ownerid) VALUES('private', @name, @model, @ownerid)");
                            roomId = (uint)dbClient.GetID();
                        }
                        gameClient.GetHabbo().AddRoom(roomId);

                        return this.GetRoomData(roomId);
                    }
                    else
                    {
                        gameClient.SendNotif("Room name is too short!");
                        return null;
                    }
                }
                else
                {
                    gameClient.SendNotif("You need HC/VIP to use this room model!");
                    return null;
                }
            }
            else
            {
                gameClient.SendNotif("This room model havent yet added! Please try again later!");
                return null;
            }
        }

        public Room GetRoom(uint id)
        {
            if (this.LoadedRooms.ContainsKey(id))
            {
                return this.LoadedRooms[id];
            }
            else
            {
                return null;
            }
        }

        public Room GetAndLoadRoom(uint roomId)
        {
            Room tryGetRoom = this.GetRoom(roomId);
            if (tryGetRoom == null)
            {
                RoomData roomData = this.GetAndLoadRoomData(roomId);
                if (roomData != null)
                {
                    Room room = new Room(roomData);
                    this.LoadedRooms.Add(room.ID, room);
                    return room;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return tryGetRoom;
            }
        }

        public RoomData GetAndLoadRoomData(uint roomId)
        {
            RoomData roomData = this.GetRoomData(roomId);
            if (roomData == null)
            {
                return this.LoadRoomData(roomId);
            }
            else
            {
                return roomData;
            }
        }

        public RoomModel GetModel(string id)
        {
            if (this.RoomModels.ContainsKey(id))
            {
                return this.RoomModels[id];
            }
            else
            {
                return null;
            }
        }

        public void OnCycle()
        {
            TimeSpan span = (TimeSpan)(DateTime.Now - this.LastRoomCycleExecution);
            if (span.TotalMilliseconds >= 480.0)
            {
                this.LastRoomCycleExecution = DateTime.Now;
                foreach(Room room in this.LoadedRooms.Values.ToList())
                {
                    if (room != null)
                    {
                        if (room.RoomCycleTask == null || room.RoomCycleTask.IsCompleted)
                        {
                            room.RoomCycleTask = new Task(new Action(room.OnCycle));
                            room.RoomCycleTask.Start();
                        }
                    }
                }
            }
        }

        public void UnloadRoom(Room room)
        {
            if (room != null)
            {
                this.LoadedRooms.Remove(room.ID);
                this.LoadedRoomData.Remove(room.ID);
                room.UnloadRoom();
            }
        }

        public List<Room> GetLoadedRooms()
        {
            return this.LoadedRooms.Values.ToList();
        }
    }
}
