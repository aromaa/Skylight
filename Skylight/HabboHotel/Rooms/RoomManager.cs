using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms.Exceptions;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
        public Dictionary<uint, RoomData> NewbieRooms;

        public Task RoomCycleTask;

        public RoomManager()
        {
            this.RoomModels = new Dictionary<string, RoomModel>();
            this.LoadedRoomData = new Dictionary<uint, RoomData>();
            this.LoadedRooms = new Dictionary<uint, Room>();
            this.NewbieRooms = new Dictionary<uint, RoomData>();
        }

        public int LoadedRoomsCount
        {
            get
            {
                return this.LoadedRooms.Count;
            }
        }

        public void LoadRoomModels(DatabaseClient dbClient)
        {
            Logging.Write("Loading room models... ", ConsoleColor.White);
            Dictionary<string, RoomModel> newModels = new Dictionary<string, RoomModel>();

            DataTable models = dbClient.ReadDataTable("SELECT * FROM room_models");
            if (models != null)
            {
                foreach(DataRow dataRow in models.Rows)
                {
                    string id = (string)dataRow["id"];
                    newModels.Add(id, new RoomModel(id, (int)dataRow["door_x"], (int)dataRow["door_y"], (int)dataRow["door_z"], (int)dataRow["door_dir"], (string)dataRow["heightmap"], (string)dataRow["public_items"], (string)dataRow["club_name"]));
                }
            }

            this.RoomModels = newModels;
            Logging.WriteLine("completed! ", ConsoleColor.Green);
        }

        public void LoadNewbieRooms(DatabaseClient dbClient)
        {
            Logging.Write("Loading newbie rooms... ", ConsoleColor.White);
            Dictionary<uint, RoomData> newRooms = new Dictionary<uint, RoomData>();

            DataTable rooms = dbClient.ReadDataTable("SELECT * FROM rooms_newbie_room");
            if (rooms != null)
            {
                foreach(DataRow dataRow in rooms.Rows)
                {
                    RoomData newbieRoom = new RoomData(dataRow);
                    newRooms.Add(newbieRoom.ID, newbieRoom);
                }
            }

            this.NewbieRooms = newRooms;
            Logging.WriteLine("completed! ", ConsoleColor.Green);
        }

        public RoomData LoadRoomData(uint id)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("roomid", id);
                DataRow dataRow = dbClient.ReadDataRow("SELECT * FROM rooms WHERE id = @roomid LIMIT 1");

                return this.LoadRoomData(id, dataRow);
            }
        }

        public RoomData LoadRoomData(uint id, DataRow dataRow)
        {
            if (dataRow != null)
            {
                RoomData roomData = new RoomData(dataRow);
                this.LoadedRoomData.Add(id, roomData);
                return roomData;
            }
            else
            {
                return null;
            }
        }

        public RoomData TryGetRoomData(uint id)
        {
            this.LoadedRoomData.TryGetValue(id, out RoomData roomData);
            return roomData;
        }

        public RoomData CreateRoom(GameClient gameClient, string roomName, string roomDescription, string roomModel, int categoryId, int maxUsers, int tradeType, RoomStateType state)
        {
            roomName = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(roomName));
            if (this.RoomModels.ContainsKey(roomModel))
            {
                string clubName = this.RoomModels[roomModel].ClubName;
                if (string.IsNullOrEmpty(clubName) || gameClient.GetHabbo().GetSubscriptionManager().HasSubscription(clubName)) //hc check
                {
                    if (roomName.Length >= 1)
                    {
                        uint roomId = 0;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("name", roomName);
                            dbClient.AddParamWithValue("description", roomDescription);
                            dbClient.AddParamWithValue("model", roomModel);
                            dbClient.AddParamWithValue("category", categoryId);
                            dbClient.AddParamWithValue("usersMax", maxUsers);
                            dbClient.AddParamWithValue("ownerid", gameClient.GetHabbo().ID);
                            dbClient.ExecuteQuery("INSERT INTO rooms(type, name, description, model, category, users_max, trade, ownerid, data, state) VALUES('private', @name, @description, @model, @category, @usersMax, '" + tradeType + "', @ownerid, '', '" + (state == RoomStateType.PASSWORD ? "password" : state == RoomStateType.LOCKED ? "locked" : "open") + "')");
                            roomId = (uint)dbClient.GetID();
                        }
                        gameClient.GetHabbo().AddRoom(roomId);

                        return this.TryGetAndLoadRoomData(roomId);
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

        public Room TryGetRoom(uint id)
        {
            this.LoadedRooms.TryGetValue(id, out Room room);
            return room;
        }

        public Room TryGetAndLoadRoom(uint roomId)
        {
            Room tryGetRoom = this.TryGetRoom(roomId);
            if (tryGetRoom == null)
            {
                RoomData roomData = this.TryGetAndLoadRoomData(roomId);
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

        public RoomData TryGetAndLoadRoomData(uint roomId)
        {
            RoomData roomData = this.TryGetRoomData(roomId);
            if (roomData == null)
            {
                return this.LoadRoomData(roomId);
            }
            else
            {
                return roomData;
            }
        }

        public RoomData TryGetAndLoadRoomData(uint roomId, DataRow dataRow)
        {
            RoomData roomData = this.TryGetRoomData(roomId);
            if (roomData == null)
            {
                return this.LoadRoomData(roomId, dataRow);
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
            try
            {
                foreach (Room room in this.LoadedRooms.Values.ToList())
                {
                    if (!(room?.RoomUnloaded ?? false))
                    {
                        if (!room.RoomCycleCancellationTokenSource.IsCancellationRequested) //room is not about to unload bcs of overloading?
                        {
                            if (room.RoomCycleTask?.IsCompleted ?? true) //its null or completed and we should start new cycle
                            {
                                (room.RoomCycleTask = new Task(new Action(() => room.OnCycle()), room.RoomCycleCancellationTokenSource.Token)).Start();  //start cycle the room async

                                room.RoomOverloadTimer = null; //we managed to cycle, reset the timer if it was started
                            }
                            else //we should cycle, but we still cycling, start track overload
                            {
                                if (room.RoomOverloadTimer?.Elapsed.TotalSeconds >= 5) //after 5s we are going to unload this shit!
                                {
                                    room.RoomCycleCancellationTokenSource.Cancel();
                                }
                            }
                        }
                        else
                        {
                            room.CrashRoom(new RoomOverloadException(room.ID, "Room cycle cancalled", this));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogException("Error in room cycle! " + ex.ToString());
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

        public void Shutdown()
        {
            if (this.LoadedRooms != null)
            {
                foreach (Room room in this.LoadedRooms.Values.ToList())
                {
                    if (room != null)
                    {
                        Skylight.GetGame().GetRoomManager().UnloadRoom(room);
                    }
                }

                this.LoadedRooms.Clear();
            }
            this.LoadedRooms = null;

            if (this.LoadedRoomData != null)
            {
                this.LoadedRoomData.Clear();
            }
            this.LoadedRoomData = null;

            if (this.RoomModels != null)
            {
                this.RoomModels.Clear();
            }
            this.RoomModels = null;
        }

        public Room CreateNewbieRoom(GameClient gameClient, uint roomId_)
        {
            if (this.NewbieRooms.TryGetValue(roomId_, out RoomData newbieData))
            {
                uint roomId = 0;
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("name", newbieData.Name);
                    dbClient.AddParamWithValue("description", newbieData.Description);
                    dbClient.AddParamWithValue("ownerid", gameClient.GetHabbo().ID);
                    dbClient.AddParamWithValue("type", newbieData.Type);
                    dbClient.AddParamWithValue("model", newbieData.Model);
                    dbClient.AddParamWithValue("state", newbieData.State == RoomStateType.PASSWORD ? "password" : newbieData.State == RoomStateType.LOCKED ? "locked" : "open");
                    dbClient.AddParamWithValue("category", newbieData.Category);
                    dbClient.AddParamWithValue("usersNow", newbieData.UsersNow);
                    dbClient.AddParamWithValue("usersMax", newbieData.UsersMax);
                    dbClient.AddParamWithValue("publicCcts", newbieData.PublicCCTs);
                    dbClient.AddParamWithValue("score", newbieData.Score);
                    dbClient.AddParamWithValue("tags", string.Join(",", newbieData.Tags));
                    dbClient.AddParamWithValue("iconBg", newbieData.RoomIcon.BackgroundImage);
                    dbClient.AddParamWithValue("iconFg", newbieData.RoomIcon.ForegroundImage);
                    dbClient.AddParamWithValue("iconItems", newbieData.RoomIcon.ItemsToString());
                    dbClient.AddParamWithValue("password", newbieData.Password);
                    dbClient.AddParamWithValue("wallpaper", newbieData.Wallpaper);
                    dbClient.AddParamWithValue("floor", newbieData.Floor);
                    dbClient.AddParamWithValue("landscape", newbieData.Landscape);
                    dbClient.AddParamWithValue("allowPets", TextUtilies.BoolToString(newbieData.AllowPets));
                    dbClient.AddParamWithValue("allowPetsEat", TextUtilies.BoolToString(newbieData.AllowPetsEat));
                    dbClient.AddParamWithValue("allowWalkthrough", TextUtilies.BoolToString(newbieData.AllowWalkthrough));
                    dbClient.AddParamWithValue("hidewalls", TextUtilies.BoolToString(newbieData.Hidewalls));
                    dbClient.AddParamWithValue("wallthick", newbieData.Wallthick);
                    dbClient.AddParamWithValue("floorthick", newbieData.Floorthick);
                    dbClient.ExecuteQuery("INSERT INTO rooms(name, description, ownerid, type, model, state, category, users_now, users_max, public_ccts, score, tags, icon_bg, icon_fg, icon_items, password, wallpaper, floor, landscape, allow_pets, allow_pets_eat, allow_walkthrough, hidewalls, wallthick, floorthick, data) VALUES(@name, @description, @ownerid, @type, @model, @state, @category, @usersNow, @usersMax, @publicCcts, @score, @tags, @iconBg, @iconFg, @iconItems, @password, @wallpaper, @floor, @landscape, @allowPets, @allowPetsEat, @allowWalkthrough, @hidewalls, @wallthick, @floorthick, '')");
                    roomId = (uint)dbClient.GetID();

                    if (roomId > 0) //check for possible error, ExecuteQuery should throw exception if there is something wrong with the query
                    {
                        if (Skylight.GetGame().GetItemManager().NewbieRoomItems.TryGetValue(roomId_, out List<RoomItem> items) && items.Count > 0) //does the room have items or is it empty, LMAO
                        {
                            dbClient.AddParamWithValue("roomId", roomId);
                            dbClient.AddParamWithValue("userId", gameClient.GetHabbo().ID);

                            string itemQuery = "";
                            foreach (RoomItem item in items)
                            {
                                dbClient.AddParamWithValue("baseItem" + item.ID, item.BaseItem.ID);
                                dbClient.AddParamWithValue("extraData" + item.ID, item.ExtraData);
                                dbClient.AddParamWithValue("x" + item.ID, item.X);
                                dbClient.AddParamWithValue("y" + item.ID, item.Y);
                                dbClient.AddParamWithValue("z" + item.ID, item.Z);
                                dbClient.AddParamWithValue("rot" + item.ID, item.Rot);
                                dbClient.AddParamWithValue("wallPos" + item.ID, item.WallCoordinate != null ? item.WallCoordinate.ToString() : "");

                                itemQuery += "INSERT INTO items(user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos) VALUES(@userId, @roomId, @baseItem" + item.ID + ", @extraData" + item.ID + ", @x" + item.ID + ", @y" + item.ID + ", @z" + item.ID + ", @rot" + item.ID + ", @wallPos" + item.ID + "); ";
                            }

                            if (itemQuery.Length > 0) //for sure
                            {
                                dbClient.ExecuteQuery(itemQuery);
                            }
                        }
                    }
                }

                gameClient.GetHabbo().AddRoom(roomId); //navigator stuff
                return this.TryGetAndLoadRoom(roomId); //load the room and return it
            }
            else
            {
                return null;//now newbie room :(
            }
        }
    }
}
