using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
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
    public class RoomItemManager
    {
        private Room Room;

        public Dictionary<uint, RoomItem> Items;
        public List<RoomItem> FloorItems;
        public List<RoomItem> WallItems;

        public List<RoomItem> AddedRoomItems;
        public List<RoomItem> RoomItemStateUpdated;
        public List<RoomItem> RoomItemMoved;

        public RoomItemManager(Room room)
        {
            this.Room = room;

            this.Items = new Dictionary<uint, RoomItem>();
            this.FloorItems = new List<RoomItem>();
            this.WallItems = new List<RoomItem>();

            this.AddedRoomItems = new List<RoomItem>();
            this.RoomItemStateUpdated = new List<RoomItem>();
            this.RoomItemMoved = new List<RoomItem>();
        }


        public void LoadItems()
        {
            DataTable items = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                items = dbClient.ReadDataTable("SELECT id, base_item, extra_data, x, y, z, rot, wall_pos FROM items WHERE room_id = '" + this.Room.ID + "' ORDER BY room_id DESC");
            }

            if (items != null)
            {
                foreach (DataRow dataRow in items.Rows)
                {
                    string wallPos = (string)dataRow["wall_pos"];
                    RoomItem roomItem = new RoomItem((uint)dataRow["Id"], this.Room.ID, (uint)dataRow["base_item"], (string)dataRow["extra_data"], (int)dataRow["x"], (int)dataRow["y"], (double)dataRow["z"], (int)dataRow["rot"], (string.IsNullOrEmpty(wallPos) ? null : new WallCoordinate(wallPos)),this.Room);
                    this.AddItem(roomItem, false);
                }
            }
        }

        public void AddItem(RoomItem item, bool newItem)
        {
            this.Items.Add(item.ID, item);

            if (item.IsFloorItem)
            {
                this.FloorItems.Add(item);
            }

            if (item.IsWallItem)
            {
                this.WallItems.Add(item);
            }

            if (newItem)
            {
                this.AddedRoomItems.Add(item);
            }
        }

        public void RemoveItem(RoomItem item)
        {
            this.Items.Remove(item.ID);
            this.FloorItems.Remove(item);
            this.WallItems.Remove(item);
            this.AddedRoomItems.Remove(item);
            this.RoomItemStateUpdated.Remove(item);

            if (item.IsWallItem)
            {
                ServerMessage WallItemRemoved = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                WallItemRemoved.Init(r63aOutgoing.RemoveWallItemFromRoom);
                WallItemRemoved.AppendStringWithBreak(item.ID.ToString());
                WallItemRemoved.AppendInt32(0);
                this.Room.SendToAll(WallItemRemoved, null);
            }
            else
            {
                ServerMessage FloorItemRemoved = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                FloorItemRemoved.Init(r63aOutgoing.RemoveFloorItemFromRoom);
                FloorItemRemoved.AppendStringWithBreak(item.ID.ToString());
                FloorItemRemoved.AppendInt32(0);
                this.Room.SendToAll(FloorItemRemoved, null);
            }
        }

        public void SaveAll()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (this.AddedRoomItems != null && this.AddedRoomItems.Count > 0)
                {
                    foreach (RoomItem item in this.AddedRoomItems.ToList())
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

                    this.AddedRoomItems.Clear();
                }

                if (this.RoomItemMoved != null && this.RoomItemMoved.Count > 0)
                {
                    foreach (RoomItem item in this.AddedRoomItems.ToList())
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

                    this.RoomItemMoved.Clear();
                }

                if (this.RoomItemStateUpdated != null && this.RoomItemStateUpdated.Count > 0)
                {
                    foreach (RoomItem item in this.RoomItemStateUpdated.ToList())
                    {
                        dbClient.AddParamWithValue("room_id" + item.ID, this.Room.ID);
                        dbClient.AddParamWithValue("base_item" + item.ID, item.BaseItem.ID);
                        dbClient.AddParamWithValue("extra_data" + item.ID, item.ExtraData);
                        dbClient.AddParamWithValue("item_id" + item.ID, item.ID);

                        stringBuilder.Append("UPDATE items SET room_id = @room_id" + item.ID + ", base_item = @base_item" + item.ID + ", extra_data = @extra_data" + item.ID + " WHERE id = @item_id" + item.ID + " LIMIT 1; ");
                    }

                    this.RoomItemStateUpdated.Clear();
                }

                if (stringBuilder.Length > 0)
                {
                    dbClient.ExecuteQuery(stringBuilder.ToString());
                }
                stringBuilder.Clear();
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
                return false;
            }


            return true;
        }

        public bool AddFloorItemToRoom(GameClient session, RoomItem item, int x, int y, int rotation)
        {
            List<AffectedTile> tiles = ItemUtilies.AffectedTiles(item.BaseItem.Lenght, item.BaseItem.Width, x, y, rotation).Values.ToList();
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

            item.Rot = rotation;
            item.SetLocation(x, y, height);

            this.AddItem(item, true);
            this.Room.RoomGamemapManager.UpdateTiles();

            ServerMessage AddFloorItemToRoom = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            AddFloorItemToRoom.Init(r63aOutgoing.AddFloorItemToRoom);
            item.Serialize(AddFloorItemToRoom);
            this.Room.SendToAll(AddFloorItemToRoom, null);

            return true;
        }

        public bool AddWallItemToRoom(GameClient session, RoomItem item)
        {
            this.AddItem(item, true);

            ServerMessage AddWallItemToRoom = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            AddWallItemToRoom.Init(r63aOutgoing.AddWallItemToRoom);
            item.Serialize(AddWallItemToRoom);
            this.Room.SendToAll(AddWallItemToRoom, null);
            return true;
        }

        public bool MoveFloorItemOnRoom(GameClient session, RoomItem item, int x, int y, int rotation)
        {
            List<AffectedTile> oldTiles = item.AffectedTiles.Values.ToList();
            oldTiles.Add(new AffectedTile(item.X, item.Y, item.Rot));

            List<AffectedTile> newTiles = ItemUtilies.AffectedTiles(item.BaseItem.Lenght, item.BaseItem.Width, x, y, rotation).Values.ToList();
            newTiles.Add(new AffectedTile(x, y, rotation));

            double height = 0;
            foreach (AffectedTile tile in newTiles)
            {
                if (!this.CanPlaceItemAt(item, tile.X, tile.Y))
                {
                    return false;
                }

                RoomTile tile_ = this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y);
                if (tile_.HigestRoomItem == null || tile_.HigestRoomItem.ID != item.ID)
                {
                    if (tile_.GetZ(true) > height)
                    {
                        height = tile_.GetZ(true);
                    }
                }
            }

            if (!this.RoomItemMoved.Contains(item) && !this.AddedRoomItems.Contains(item))
            {
                this.RoomItemStateUpdated.Remove(item);
                this.RoomItemMoved.Add(item);
            }

            item.Rot = rotation;
            item.SetLocation(x, y, height);

            this.Room.RoomGamemapManager.UpdateTiles();

            ServerMessage roomFloorItemMoved = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            roomFloorItemMoved.Init(r63aOutgoing.RoomFloorItemMoved);
            item.Serialize(roomFloorItemMoved);
            this.Room.SendToAll(roomFloorItemMoved, null);

            foreach (RoomUser user in this.Room.RoomUserManager.GetRealUsers())
            {
                bool nextPlease = false;

                foreach (AffectedTile tile in oldTiles)
                {
                    if (user.GetX == tile.X && user.GetY == tile.Y)
                    {
                        this.Room.RoomUserManager.UpdateUserStateOnTile(user);

                        nextPlease = true;
                        break;
                    }
                }

                if (!nextPlease)
                {
                    foreach (AffectedTile tile in newTiles)
                    {
                        if (user.GetX == tile.X && user.GetY == tile.Y)
                        {
                            this.Room.RoomUserManager.UpdateUserStateOnTile(user);
                            break;
                        }
                    }
                }
            }
            return true;
        }

        public void Pickall(GameClient session)
        {
            foreach(RoomItem item in this.Items.Values.ToList())
            {
                this.RemoveItem(item);
            }

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", session.GetHabbo().ID);
                dbClient.AddParamWithValue("roomid", this.Room.ID);
                dbClient.ExecuteQuery("UPDATE items SET room_id = 0, user_id = @userid WHERE room_id = @roomid");
            }

            this.AddedRoomItems.Clear();
            this.RoomItemStateUpdated.Clear();
            this.RoomItemMoved.Clear();

            this.Room.RoomGamemapManager.UpdateTiles();
            this.Room.RoomUserManager.UpdateUserTiles();

            //and then update user inventory
            session.GetHabbo().GetInventoryManager().UpdateInventory(true);
        }

        public bool MoveWallItemOnRoom(GameClient session, RoomItem item, WallCoordinate newCoords)
        {
            if (!this.RoomItemMoved.Contains(item) && !this.AddedRoomItems.Contains(item))
            {
                this.RoomItemStateUpdated.Remove(item);
                this.RoomItemMoved.Add(item);
            }

            item.WallCoordinate = newCoords;

            ServerMessage updateRoomWallItem = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            updateRoomWallItem.Init(r63aOutgoing.UpdateRoomWallItem);
            item.Serialize(updateRoomWallItem);
            this.Room.SendToAll(updateRoomWallItem, null);
            return true;
        }

        public RoomItem GetRoomItem(uint id)
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

        public void UpdateItemStateToDatabase(RoomItem item)
        {
            if (!this.RoomItemStateUpdated.Contains(item) && !this.AddedRoomItems.Contains(item) && !this.RoomItemMoved.Contains(item))
            {
                this.RoomItemStateUpdated.Add(item);
            }
        }

        public void PickupWallItemFromRoom(GameClient session, RoomItem item)
        {
            this.RemoveItem(item);

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userid", session.GetHabbo().ID);
                dbClient.AddParamWithValue("itemid", item.ID);
                dbClient.ExecuteQuery("UPDATE items SET room_id = 0, user_id = @userid WHERE id = @itemid");
            }
        }

        public void PickupFloorItemFromRoom(GameClient session, RoomItem item)
        {
            this.RemoveItem(item);
            this.Room.RoomGamemapManager.UpdateTiles();

            List<AffectedTile> oldTiles = item.AffectedTiles.Values.ToList();
            oldTiles.Add(new AffectedTile(item.X, item.Y, item.Rot));
            foreach (RoomUser user in this.Room.RoomUserManager.GetRealUsers())
            {
                foreach (AffectedTile tile in oldTiles)
                {
                    if (user.GetX == tile.X && user.GetY == tile.Y)
                    {
                        this.Room.RoomUserManager.UpdateUserStateOnTile(user);
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
    }
}
