using SkylightEmulator.Collections;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Outgoing.r63a;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItemManager
    {
        private Room Room;

        public ConcurrentListDictionary<uint, Type, RoomItem> FloorItems;
        public ConcurrentListDictionary<uint, Type, RoomItem> WallItems;

        public ConcurrentDictionaryQueue<uint, RoomItem> AddedAndMovedRoomItems;
        public ConcurrentDictionaryQueue<uint, RoomItem> RoomItemStateUpdated;
        public ConcurrentDictionaryQueue<uint, RoomItem> ItemDataChanged;

        public ConcurrentDictionaryQueue<uint, RoomItem> RequireUpdateClientSide;
        public ConcurrentDictionaryQueue<uint, RoomItemRollerMovement> MoveAnimation;

        public int RollerTimer;
        public RoomItemJukebox Jukebox;

        public RoomItemManager(Room room)
        {
            this.Room = room;

            this.FloorItems = new ConcurrentListDictionary<uint, Type, RoomItem>();
            this.WallItems = new ConcurrentListDictionary<uint, Type, RoomItem>();

            this.AddedAndMovedRoomItems = new ConcurrentDictionaryQueue<uint, RoomItem>();
            this.RoomItemStateUpdated = new ConcurrentDictionaryQueue<uint, RoomItem>();
            this.ItemDataChanged = new ConcurrentDictionaryQueue<uint, RoomItem>();

            this.RequireUpdateClientSide = new ConcurrentDictionaryQueue<uint, RoomItem>();
            this.MoveAnimation = new ConcurrentDictionaryQueue<uint, RoomItemRollerMovement>();
        }


        public void LoadItems()
        {
            DataTable items = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                items = dbClient.ReadDataTable("SELECT id, user_id, base_item, extra_data, x, y, z, rot, wall_pos FROM items WHERE room_id = '" + this.Room.ID + "' ORDER BY room_id DESC");
            }

            Dictionary<uint, RoomItem> requestData = new Dictionary<uint, RoomItem>();
            if (items != null)
            {
                foreach (DataRow dataRow in items.Rows)
                {
                    string wallPos = (string)dataRow["wall_pos"];
                    RoomItem roomItem = RoomItem.GetRoomItem((uint)dataRow["Id"], this.Room.ID, (uint)dataRow["user_id"], (uint)dataRow["base_item"], (string)dataRow["extra_data"], (int)dataRow["x"], (int)dataRow["y"], (double)dataRow["z"], (int)dataRow["rot"], (string.IsNullOrEmpty(wallPos) ? null : new WallCoordinate(wallPos)),this.Room);
                    this.AddItem(null, roomItem, false);

                    if (roomItem.GetBaseItem().InteractionType.ToLower().StartsWith("wf_") || roomItem.GetBaseItem().InteractionType.ToLower() == "fbgate" || roomItem.GetBaseItem().InteractionType.ToLower() == "firework" || roomItem.GetBaseItem().InteractionType.ToLower() == "jukebox" || roomItem.GetBaseItem().InteractionType.ToLower() == "photo")
                    {
                        requestData.Add(roomItem.ID, roomItem);
                    }
                }
            }

            if (requestData != null && requestData.Count > 0)
            {
                DataTable data = null;
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    data = dbClient.ReadDataTable("SELECT * FROM items_data WHERE item_id IN(" + string.Join(",", requestData.Keys) + ") LIMIT " + requestData.Count);
                }

                if (data != null && data.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in data.Rows)
                    {
                        requestData[(uint)dataRow["item_id"]].LoadItemData((string)dataRow["data"]);
                    }
                }
            }
        }

        public void AddItem(GameClient session, RoomItem item, bool newItem)
        {
            if (item.IsFloorItem)
            {
                if (!this.FloorItems.Add(item.ID, item.GetType(), item))
                {
                    session.SendNotif("Something weird happend... Placement failed");
                    return;
                }
            }
            else if (item.IsWallItem)
            {
                if (!this.WallItems.Add(item.ID, item.GetType(), item))
                {
                    session.SendNotif("Something weird happend... Placement failed");
                    return;
                }
            }

            this.Room.RoomGameManager.AddItem(item);

            if (newItem)
            {
                this.AddedAndMovedRoomItems.AddOrUpdate(item.ID, item, (key, oldValue) => item);
                this.RoomItemStateUpdated.TryRemove(item.ID);

                item.OnPlace(session);
                this.CheckItemBasedAchievements(item);
            }
            else
            {
                item.OnLoad();
            }

            if (item is RoomItemJukebox)
            {
                if (this.Jukebox == null)
                {
                    this.Jukebox = (RoomItemJukebox)item;
                }
            }
        }

        public void RemoveItem(GameClient session, RoomItem item)
        {
            this.FloorItems.Remove(item.ID, item.GetType());
            this.WallItems.Remove(item.ID, item.GetType());
            this.AddedAndMovedRoomItems.TryRemove(item.ID, out RoomItem item_);
            this.RoomItemStateUpdated.TryRemove(item.ID, out item_);
            this.Room.RoomGameManager.RemoveItem(item);

            if (item.IsWallItem)
            {
                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.RemoveWallItem, new ValueHolder("ID", item.ID, "UserID", session == null ? 0 : session.GetHabbo().ID)));
            }
            else
            {
                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.RemoveFloorItem, new ValueHolder("ID", item.ID, "UserID", session == null ? 0 : session.GetHabbo().ID)));
            }

            item.OnPickup(session);
            this.CheckItemBasedAchievements(item);

            if (item is RoomItemJukebox)
            {
                if (this.Jukebox == (RoomItemJukebox)item)
                {
                    this.Jukebox = null;
                }
            }
        }

        public void CheckItemBasedAchievements(RoomItem latestItem)
        {
            GameClient owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.Room.RoomData.OwnerID);
            if (owner != null)
            {
                IEnumerable<RoomItem> items = this.GetItems();
                int itemsCount = items.Count();
                if (itemsCount > owner.GetHabbo().GetUserStats().RoomBuilder)
                {
                    owner.GetHabbo().GetUserStats().RoomBuilder = itemsCount;
                    owner.GetHabbo().GetUserAchievements().CheckAchievement("RoomBuilder");
                }

                int diffItemsCount = items.GroupBy(i => i.GetBaseItem().ID).Count();
                if (diffItemsCount > owner.GetHabbo().GetUserStats().FurniCollector)
                {
                    owner.GetHabbo().GetUserStats().FurniCollector = diffItemsCount;
                    owner.GetHabbo().GetUserAchievements().CheckAchievement("FurniCollector");
                }

                if (latestItem is RoomItemBlackHole)
                {
                    int blackHolesCount = items.Count(i => i is RoomItemBlackHole);
                    if (blackHolesCount > owner.GetHabbo().GetUserStats().RoomArchitect)
                    {
                        owner.GetHabbo().GetUserStats().RoomArchitect = blackHolesCount;
                        owner.GetHabbo().GetUserAchievements().CheckAchievement("RoomArchitect");
                    }
                }
                else if (latestItem is RoomItemIceSkatingPatch)
                {
                    int iceSkatePatchesCount = items.Count(i => i is RoomItemIceSkatingPatch);
                    if (iceSkatePatchesCount > owner.GetHabbo().GetUserStats().IceRinkBuilder)
                    {
                        owner.GetHabbo().GetUserStats().IceRinkBuilder = iceSkatePatchesCount;
                        owner.GetHabbo().GetUserAchievements().CheckAchievement("IceRinkBuilder");
                    }
                }
                else if (latestItem is RoomItemRollerskate)
                {
                    int rollerRinkBuilderPatchesCount = items.Count(i => i is RoomItemRollerskate);
                    if (rollerRinkBuilderPatchesCount > owner.GetHabbo().GetUserStats().RollerRinkBuilder)
                    {
                        owner.GetHabbo().GetUserStats().RollerRinkBuilder = rollerRinkBuilderPatchesCount;
                        owner.GetHabbo().GetUserAchievements().CheckAchievement("RollerRinkBuilder");
                    }
                }
                else if (latestItem.GetBaseItem().ItemName == "easter11_grasspatch")
                {
                    int bunnyRunBuilderPatchesCount = items.Count(i => i.GetBaseItem().ItemName == "easter11_grasspatch");
                    if (bunnyRunBuilderPatchesCount > owner.GetHabbo().GetUserStats().BunnyRunBuilder)
                    {
                        owner.GetHabbo().GetUserStats().BunnyRunBuilder = bunnyRunBuilderPatchesCount;
                        owner.GetHabbo().GetUserAchievements().CheckAchievement("BunnyRunBuilder");
                    }
                }
                else if (latestItem.GetBaseItem().ItemName == "snowb_slope")
                {
                    int snowboardingBuilderPatchesCount = items.Count(i => i.GetBaseItem().ItemName == "snowb_slope");
                    if (snowboardingBuilderPatchesCount > owner.GetHabbo().GetUserStats().SnowboardingBuilder)
                    {
                        owner.GetHabbo().GetUserStats().SnowboardingBuilder = snowboardingBuilderPatchesCount;
                        owner.GetHabbo().GetUserAchievements().CheckAchievement("SnowboardingBuilder");
                    }
                }
            }
        }

        public void SaveAll()
        {
            if (this.AddedAndMovedRoomItems.Count > 0 || this.RoomItemStateUpdated.Count > 0 || this.ItemDataChanged.Count > 0)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    if (this.AddedAndMovedRoomItems.Count > 0 || this.RoomItemStateUpdated.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        HashSet<uint> dublicates = new HashSet<uint>();

                        while (this.AddedAndMovedRoomItems.TryDequeueValue(out RoomItem item))
                        {
                            if (dublicates.Add(item.ID))
                            {
                                if (item.IsWallItem) //wall items
                                {
                                    dbClient.AddParamWithValue("room_id" + item.ID, this.Room.ID);
                                    dbClient.AddParamWithValue("base_item" + item.ID, item.BaseItem.ID);
                                    dbClient.AddParamWithValue("extra_data" + item.ID, item.ExtraData);
                                    dbClient.AddParamWithValue("wall_pos" + item.ID, item.WallCoordinate.ToString());
                                    dbClient.AddParamWithValue("item_id" + item.ID, item.ID);

                                    stringBuilder.Append("UPDATE items SET room_id = @room_id" + item.ID + ", base_item = @base_item" + item.ID + ", extra_data = @extra_data" + item.ID + ", wall_pos = @wall_pos" + item.ID + " WHERE id = @item_id" + item.ID + " LIMIT 1; ");
                                }
                                else //others
                                {
                                    dbClient.AddParamWithValue("room_id" + item.ID, this.Room.ID);
                                    dbClient.AddParamWithValue("base_item" + item.ID, item.BaseItem.ID);
                                    dbClient.AddParamWithValue("extra_data" + item.ID, item.ExtraData);
                                    dbClient.AddParamWithValue("x" + item.ID, item.X);
                                    dbClient.AddParamWithValue("y" + item.ID, item.Y);
                                    dbClient.AddParamWithValue("z" + item.ID, item.Z);
                                    dbClient.AddParamWithValue("rot" + item.ID, item.Rot);
                                    dbClient.AddParamWithValue("item_id" + item.ID, item.ID);

                                    stringBuilder.Append("UPDATE items SET room_id = @room_id" + item.ID + ", base_item = @base_item" + item.ID + ", extra_data = @extra_data" + item.ID + ", x = @x" + item.ID + ", y = @y" + item.ID + ", z = @z" + item.ID + ", rot = @rot" + item.ID + ", wall_pos = '' WHERE id = @item_id" + item.ID + " LIMIT 1; ");
                                }
                            }
                        }

                        while (this.RoomItemStateUpdated.TryDequeueValue(out RoomItem item))
                        {
                            if (dublicates.Add(item.ID))
                            {
                                dbClient.AddParamWithValue("room_id" + item.ID, this.Room.ID);
                                dbClient.AddParamWithValue("base_item" + item.ID, item.BaseItem.ID);
                                dbClient.AddParamWithValue("extra_data" + item.ID, item.ExtraData);
                                dbClient.AddParamWithValue("item_id" + item.ID, item.ID);

                                stringBuilder.Append("UPDATE items SET room_id = @room_id" + item.ID + ", base_item = @base_item" + item.ID + ", extra_data = @extra_data" + item.ID + " WHERE id = @item_id" + item.ID + " LIMIT 1; ");
                            }
                        }

                        if (stringBuilder.Length > 0)
                        {
                            dbClient.ExecuteQuery(stringBuilder.ToString());
                            dbClient.ClearParams();
                        }
                    }

                    //WIRED ETC
                    if (this.ItemDataChanged.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        HashSet<uint> dublicates = new HashSet<uint>();

                        while (this.ItemDataChanged.TryDequeueValue(out RoomItem item))
                        {
                            if (dublicates.Add(item.ID))
                            {
                                dbClient.AddParamWithValue("item_id" + item.ID, item.ID);
                                dbClient.AddParamWithValue("data" + item.ID, item.GetItemData());

                                stringBuilder.Append("INSERT INTO items_data(item_id, data) VALUES(@item_id" + item.ID + ", @data" + item.ID + ") ON DUPLICATE KEY UPDATE data = VALUES(data); ");
                            }
                        }

                        if (stringBuilder.Length > 0)
                        {
                            dbClient.ExecuteQuery(stringBuilder.ToString());
                        }
                    }
                }
            }
        }

        public bool CanPlaceItemAt(RoomItem item, int x, int y)
        {
            if (!this.Room.RoomGamemapManager.CoordsInsideRoom(x, y)) //its inside room
            {
                return false;
            }

            RoomTile tile = this.Room.RoomGamemapManager.GetTile(x, y);
            if (tile.HigestRoomItem == null || tile.HigestRoomItem.ID != item.ID) //its not same item
            {
                if (!tile.IsStackable) //cant stack
                {
                    return false;
                }
            }

            if (tile.IsHole) //its not on tile
            {
                return false;
            }

            if (tile.IsDoor) //we dont allow put stuff on door
            {
                return false;
            }

            if (tile.IsInUse) //user in tile
            {
                if (this.Room.RoomData.ExtraData.RoomSettingsLogic.Contains("building-users-block-furni-placement"))
                {
                    return false;
                }
                else
                {
                    if (!(item is RoomItemBall) && !(item is RoomItemBattleBanzaiPuck))
                    {
                        if (!item.GetBaseItem().IsSeat && !item.GetBaseItem().Walkable)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }


            return true;
        }

        public bool FitsInsideRoom(RoomItem item, int x, int y, int rotation)
        {
            foreach(AffectedTile tile in ItemUtilies.AffectedTiles(item.BaseItem.Lenght, item.BaseItem.Width, x, y, rotation).Values)
            {
                if (!this.Room.RoomGamemapManager.CoordsInsideRoom(tile.X, tile.Y))
                {
                    return false;
                }
            }

            return true;
        }

        public bool AddFloorItemToRoom(GameClient session, RoomItem item, int x, int y, int rotation)
        {
            int forceRotation = session != null ? session.GetHabbo().GetRoomSession().GetRoomUser().ForceRotate : -1;
            if (forceRotation != -1)
            {
                rotation = forceRotation;
            }

            HashSet<AffectedTile> tiles = new HashSet<AffectedTile>();
            tiles.UnionWith(ItemUtilies.AffectedTiles(item.BaseItem.Lenght, item.BaseItem.Width, x, y, rotation).Values);
            tiles.Add(new AffectedTile(x, y, rotation));

            double height = 0;
            foreach (AffectedTile tile in tiles)
            {
                if (!this.CanPlaceItemAt(item, tile.X, tile.Y))
                {
                    return false;
                }

                RoomTile tile_ = this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y);
                if (tile_.GetZ(true) > height)
                {
                    height = tile_.GetZ(true);
                }
            }

            double forceHeight = session.GetHabbo().GetRoomSession().GetRoomUser().ForceHeight;
            int forceState = session.GetHabbo().GetRoomSession().GetRoomUser().ForceState;

            if (forceState != -1)
            {
                if (item.GetBaseItem().InteractionType == "default")
                {
                    int mode = forceState;

                    if (mode > item.GetBaseItem().InteractionModeCounts - 1)
                    {
                        mode = 0;
                    }

                    item.ExtraData = mode.ToString();
                }
            }

            item.Rot = rotation;
            if (forceHeight != -1.0)
            {
                item.SetLocation(x, y, forceHeight);
            }
            else
            {
                item.SetLocation(x, y, height);
            }

            this.AddItem(session, item, true);
            foreach (AffectedTile tile in tiles)
            {
                this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y).AddItemToTile(item);
            }

            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.AddFloorItemToRoom, new ValueHolder("Item", item)));

            foreach (RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
            {
                foreach (AffectedTile tile in tiles)
                {
                    if (user.X == tile.X && user.Y == tile.Y)
                    {
                        user.UpdateState();
                        break;
                    }
                }
            }

            return true;
        }

        public bool AddWallItemToRoom(GameClient session, RoomItem item)
        {
            this.AddItem(session, item, true);

            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.AddWallItemToRoom, new ValueHolder("Item", item)));
            return true;
        }

        public bool MoveFloorItemOnRoom(GameClient session, RoomItem item, int x, int y, int rotation, double height = -1.0, bool sendPacket = true, params RoomItem[] ignore)
        {
            int forceRotation = session != null ? session.GetHabbo().GetRoomSession().GetRoomUser().ForceRotate : -1;
            if (forceRotation != -1)
            {
                rotation = forceRotation;
            }

            HashSet<AffectedTile> oldTiles = new HashSet<AffectedTile>(item.AffectedTiles) { new AffectedTile(item.X, item.Y, item.Rot) };
            HashSet<AffectedTile> newTiles = new HashSet<AffectedTile>(ItemUtilies.AffectedTiles(item.BaseItem.Lenght, item.BaseItem.Width, x, y, rotation).Values) { new AffectedTile(x, y, rotation) };

            if (height == -1.0)
            {
                foreach (AffectedTile tile in newTiles)
                {
                    if (!this.CanPlaceItemAt(item, tile.X, tile.Y))
                    {
                        return false;
                    }

                    RoomTile tile_ = this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y);
                    if (ignore.Length <= 0)
                    {
                        if (tile_.HigestRoomItem?.ID != item.ID)
                        {
                            height = tile_.GetZ(true);
                        }
                        else
                        {
                            height = tile_.GetZ(false);
                        }
                    }
                    else
                    {
                        RoomItem higestItem = tile_.GetHigestItem(ignore);
                        if (higestItem != null)
                        {
                            if (higestItem.ID != item.ID)
                            {
                                height = higestItem.ActiveHeight;
                            }
                            else
                            {
                                height = higestItem.Z;
                            }
                        }
                        else
                        {
                            height = tile_.ModelZ;
                        }
                    }
                }
            }

            this.AddedAndMovedRoomItems.AddOrUpdate(item.ID, item, (key, oldValue) => item);
            this.RoomItemStateUpdated.TryRemove(item.ID, out RoomItem item_);

            double forceHeight = session != null ? session.GetHabbo().GetRoomSession().GetRoomUser().ForceHeight : -1.0;
            int forceState = session != null ? session.GetHabbo().GetRoomSession().GetRoomUser().ForceState : -1;

            if (forceState != -1)
            {
                if (item.GetBaseItem().InteractionType == "default")
                {
                    int mode = forceState;

                    if (mode > item.GetBaseItem().InteractionModeCounts - 1)
                    {
                        mode = 0;
                    }

                    item.ExtraData = mode.ToString();
                }
            }

            item.Rot = rotation;
            if (forceHeight != -1.0)
            {
                item.SetLocation(x, y, forceHeight);
            }
            else
            {
                item.SetLocation(x, y, height);
            }

            foreach (AffectedTile tile in oldTiles)
            {
                this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y).RemoveItemFromTile(item);
            }

            foreach (AffectedTile tile in newTiles)
            {
                this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y).AddItemToTile(item);
            }

            if (sendPacket)
            {
                item.TryChangeUpdateState(RoomItem.UpdateStatus.MOVE);
                this.RequireUpdateClientSide.TryAdd(item.ID, item);
            }

            foreach (RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
            {
                foreach (AffectedTile tile in oldTiles.Concat(newTiles))
                {
                    if (user.X == tile.X && user.Y == tile.Y)
                    {
                        user.UpdateState();
                        break;
                    }
                }
            }

            item.OnMove(session);
            return true;
        }

        public void Pickall(GameClient session)
        {
            session.GetHabbo().GetInventoryManager().SetQueueBytes(true);

            foreach(RoomItem item in this.FloorItems.Values)
            {
                this.RemoveItem(session, item);
                session.GetHabbo().GetInventoryManager().AddRoomItemToHand(item);
            }

            foreach (RoomItem item in this.WallItems.Values)
            {
                this.RemoveItem(session, item);
                session.GetHabbo().GetInventoryManager().AddRoomItemToHand(item);
            }

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", session.GetHabbo().ID);
                dbClient.AddParamWithValue("roomid", this.Room.ID);
                dbClient.ExecuteQuery("UPDATE items SET room_id = 0, user_id = @userid WHERE room_id = @roomid");
            }

            this.AddedAndMovedRoomItems.Clear();
            this.RoomItemStateUpdated.Clear();

            this.Room.RoomGamemapManager.UpdateTiles();

            session.GetHabbo().GetInventoryManager().SetQueueBytes(false);
        }

        public bool MoveWallItemOnRoom(GameClient session, RoomItem item, WallCoordinate newCoords)
        {
            this.AddedAndMovedRoomItems.AddOrUpdate(item.ID, item, (key, oldValue) => item);
            this.RoomItemStateUpdated.TryRemove(item.ID, out RoomItem item_);

            item.WallCoordinate = newCoords;

            item.UpdateState(false, true);

            item.OnMove(session);
            return true;
        }

        public RoomItem TryGetRoomItem(uint id)
        {
            RoomItem item = this.TryGetFloorItem(id);
            if (item == null)
            {
                item = this.TryGetWallItem(id);
            }
            return item;
        }

        public RoomItem TryGetFloorItem(uint id)
        {
            this.FloorItems.TryGetValue(id, out RoomItem item);
            return item;
        }

        public RoomItem TryGetWallItem(uint id)
        {
            this.WallItems.TryGetValue(id, out RoomItem item);
            return item;
        }

        public void UpdateItemStateToDatabase(RoomItem item)
        {
            if (!this.AddedAndMovedRoomItems.ContainsKey(item.ID))
            {
                this.RoomItemStateUpdated.TryAdd(item.ID, item);
            }
        }

        public void PickupWallItemFromRoom(GameClient session, RoomItem item)
        {
            this.RemoveItem(session, item);

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", session.GetHabbo().ID);
                dbClient.AddParamWithValue("itemid", item.ID);
                dbClient.ExecuteQuery("UPDATE items SET room_id = 0, user_id = @userid WHERE id = @itemid");
            }
        }

        public void PickupFloorItemFromRoom(GameClient session, RoomItem item)
        {
            this.RemoveItem(session, item);

            HashSet<AffectedTile> oldTiles = new HashSet<AffectedTile>(item.AffectedTiles) { new AffectedTile(item.X, item.Y, item.Rot) };
            foreach (AffectedTile tile in oldTiles)
            {
                RoomTile tile_ = this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y);
                if (tile_ != null)
                {
                    tile_.RemoveItemFromTile(item);
                }
            }

            foreach (RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
            {
                foreach (AffectedTile tile in oldTiles)
                {
                    if (user.X == tile.X && user.Y == tile.Y)
                    {
                        user.UpdateState();
                        break;
                    }
                }
            }

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", session.GetHabbo().ID);
                dbClient.AddParamWithValue("itemid", item.ID);
                dbClient.ExecuteQuery("UPDATE items SET room_id = 0, user_id = @userid WHERE id = @itemid");
            }
        }

        public RoomItem GetRoomDimmer()
        {
            return this.WallItems.Values.FirstOrDefault(i => i.GetBaseItem().InteractionType.ToLower() == "dimmer");
        }

        public void OnCycle()
        {
            this.RollerTimer++;
            if (this.RollerTimer >= this.Room.RollerSpeed)
            {
                this.RollerTimer = 0;
                
                List<int> rollerUsers = new List<int>();
                List<uint> rollerItems = new List<uint>();

                foreach (RoomItem item in this.FloorItems.Get(typeof(RoomItemRoller)).OrderBy(i => i.Z))
                {
                    this.Room.ThrowIfRoomCycleCancalled("Cycle rollers", item); //Have room cycle cancalled?

                    ThreeDCoord way = item.TDC;
                    if (way.x >= this.Room.RoomGamemapManager.Model.MaxX || way.y >= this.Room.RoomGamemapManager.Model.MaxY || way.x < 0 || way.y < 0) //dont roll it out of room
                    {
                        continue;
                    }
                    else
                    {
                        RoomTile to = this.Room.RoomGamemapManager.GetTile(way.x, way.y);
                        if (!to.IsHole && !to.IsDoor && !to.IsInUse)
                        {
                            RoomTile from = this.Room.RoomGamemapManager.GetTile(item.X, item.Y);
                            List<RoomItem> itemsFrom = from.ItemsOnTile.Values.Where(i => i.Z >= item.ActiveHeight && !rollerItems.Contains(i.ID) && this.FitsInsideRoom(i, way.x, way.y, i.Rot)).ToList();
                            List<RoomItem> itemsTo = to.ItemsOnTile.Values.OrderByDescending(i => i.Z).ToList();
                            itemsTo.RemoveAll(t => itemsFrom.Contains(t));

                            RoomItem higestItem = null;
                            foreach (RoomItem item_ in itemsTo)
                            {
                                if (higestItem == null)
                                {
                                    higestItem = item_;
                                }
                                else
                                {
                                    if (higestItem.Z == item_.Z)
                                    {
                                        if (item_.ActiveHeight > higestItem.ActiveHeight)
                                        {
                                            higestItem = item_;
                                        }
                                    }
                                    else //not even on same height
                                    {
                                        break;
                                    }
                                }
                            }

                            if (!(higestItem is RoomItemRoller)) //the top item isint roller
                            {
                                if (itemsTo.Any(i => i is RoomItemRoller))
                                {
                                    continue;
                                }
                            }

                            RoomUnit user = to.CanUserMoveToTile ? this.Room.RoomGamemapManager.GetTile(item.X, item.Y).UsersOnTile.Values.FirstOrDefault(u => u.Moving == false && !rollerUsers.Contains(u.VirtualID)) : null;
                            if (itemsFrom.Count > 0 || user != null) //we have some work to do
                            {
                                double baseZ = 0;
                                if (higestItem is RoomItemRoller)
                                {
                                    baseZ = to.GetZ(true);
                                }
                                else
                                {
                                    baseZ = to.ModelZ;
                                }
                                
                                foreach (RoomItem item_ in itemsFrom)
                                {
                                    rollerItems.Add(item_.ID);

                                    foreach (AffectedTile tile in new HashSet<AffectedTile>(item_.AffectedTiles) { new AffectedTile(item_.X, item_.Y, item_.Rot) })
                                    {
                                        this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y).RemoveItemFromTile(item_);
                                    }
                                    
                                    foreach (AffectedTile tile in new HashSet<AffectedTile>(ItemUtilies.AffectedTiles(item_.GetBaseItem().Lenght, item_.GetBaseItem().Width, to.X, to.Y, item_.Rot).Values) { new AffectedTile(to.X, to.Y, item_.Rot) })
                                    {
                                        this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y).AddItemToTile(item_);
                                    }

                                    double oldZ = item_.Z;

                                    item_.SetLocation(to.X, to.Y, baseZ + item_.Z - (item.Z + item.GetBaseItem().Height));

                                    this.Room.RoomItemManager.MoveAnimation[item.ID] = new RoomItemRollerMovement(item_.ID, item.X, item.Y, oldZ, item.ID, to.X, to.Y, item_.Z);
                                }
                                
                                if (user != null)
                                {
                                    this.Room.UserWalkOff(user);
                                    if (user.X == item.X && user.Y == item.Y) //did walkoff prevent this?
                                    {
                                        rollerUsers.Add(user.VirtualID);

                                        double newZ = to.GetZ(true);
                                        
                                        this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.RollerMovement, new ValueHolder("User", new RoomUserRollerMovement(user.VirtualID, item.X, item.Y, user.Z, item.ID, to.X, to.Y, newZ))));

                                        user.SetLocation(to.X, to.Y, newZ, false);
                                        user.UpdateUserStateTimer = 2;

                                        //update user state somehow respecting the movement packet
                                        this.Room.UserWalkOn(user);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.FloorItems.Count > 0)
            {
                foreach (RoomItem item in this.FloorItems.Values)
                {
                    this.Room.ThrowIfRoomCycleCancalled("Cycle floor items", item); //Have room cycle cancalled?

                    item.OnCycle();
                }
            }

            if (this.WallItems.Count > 0)
            {
                foreach (RoomItem item in this.WallItems.Values)
                {
                    this.Room.ThrowIfRoomCycleCancalled("Cycle wall items", item); //Have room cycle cancalled?

                    item.OnCycle();
                }
            }

            //AUTO SAVE
            if (this.Room.LastAutoSave.Elapsed.TotalMinutes >= 5) //Every 5min :)
            {
                this.Room.LastAutoSave.Restart();

                this.SaveAll();
            }
        }

        public ICollection<RoomItem> GetFloorItems()
        {
            return this.FloorItems.Values;
        }

        public ICollection<RoomItem> GetWallItems()
        {
            return this.WallItems.Values;
        }

        public IEnumerable<RoomItem> GetItems()
        {
            return this.FloorItems.Values.Concat(this.WallItems.Values);
        }

        public void AboutToWalkOn(RoomUnit unit)
        {
            this.Room.RoomGamemapManager.GetTile(unit.NextStepX, unit.NextStepY)?.HigestRoomItem?.AboutToWalkOn(unit);
        }

        public void UserWalkOn(RoomUnit unit)
        {
            unit.CurrentTile?.HigestRoomItem?.OnWalkOn(unit);
        }

        public void UserWalkOff(RoomUnit unit)
        {
            unit.CurrentTile?.HigestRoomItem?.OnWalkOff(unit);
        }

        public void SyncPackets()
        {
            if (this.RequireUpdateClientSide.Count > 0)
            {
                while (this.RequireUpdateClientSide.TryDequeueValue(out RoomItem item))
                {
                    if (item.IsWallItem)
                    {
                        if (item.RoomUpdateStatus == RoomItem.UpdateStatus.STATE || item.RoomUpdateStatus == RoomItem.UpdateStatus.MOVE)
                        {
                            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.UpdateWallItem, new ValueHolder("Item", item)));
                        }
                    }
                    else
                    {
                        if (item.RoomUpdateStatus == RoomItem.UpdateStatus.STATE)
                        {
                            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.UpdateFloorItem, new ValueHolder("Item", item)));
                        }
                        else if (item.RoomUpdateStatus == RoomItem.UpdateStatus.MOVE)
                        {
                            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.MoveFloorItem, new ValueHolder("Item", item)));
                        }
                    }

                    item.TryChangeUpdateState(RoomItem.UpdateStatus.NONE);
                }
            }

            if (this.MoveAnimation.Count > 0)
            {
                Dictionary<Tuple<Point, Point>, List<RoomItemRollerMovement>> items = new Dictionary<Tuple<Point, Point>, List<RoomItemRollerMovement>>();

                while (this.MoveAnimation.TryDequeueValue(out RoomItemRollerMovement item))
                {
                    Tuple<Point, Point> tuple = new Tuple<Point, Point>(item.CurrentXY, item.NextXY);
                    if (!items.TryGetValue(tuple, out List<RoomItemRollerMovement> items_))
                    {
                        items_ = items[tuple] = new List<RoomItemRollerMovement>();
                    }

                    items_.Add(item);
                }

                foreach (List<RoomItemRollerMovement> items_ in items.Values)
                {
                    this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.RollerMovement, new ValueHolder("Movement", items_.ToArray())));
                }
            }
        }
    }
}
