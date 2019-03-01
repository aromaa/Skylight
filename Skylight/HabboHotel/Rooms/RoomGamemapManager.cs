using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomGamemapManager
    {
        private Room Room;

        public readonly RoomModel Model;
        public RoomTile[,] Tiles;

        public RoomGamemapManager(Room room)
        {
            this.Room = room;
            this.Model = Skylight.GetGame().GetRoomManager().GetModel(this.Room.RoomData.Model);

            this.Tiles = new RoomTile[this.Model.MaxX, this.Model.MaxY];
            for (int i = 0; i < this.Model.MaxX; i++)
            {
                for (int j = 0; j < this.Model.MaxY; j++)
                {
                    this.Tiles[i, j] = new RoomTile(room, i, j, this.Model.ModelHeights[i, j], this.Model.TileStates[i, j]);
                }
            }
        }

        public void UpdateTiles()
        {
            //reset some data
            foreach(RoomTile tile in this.Tiles)
            {
                if (tile != null)
                {
                    tile.ResetGamemap();
                }
            }

            foreach(RoomItem item in this.Room.RoomItemManager.FloorItems.Values)
            {
                HashSet<AffectedTile> tiles = new HashSet<AffectedTile>(item.AffectedTiles) { new AffectedTile(item.X, item.Y, item.Rot) };
                foreach (AffectedTile tile in tiles)
                {
                    RoomTile roomtile = this.GetTile(tile.X, tile.Y);
                    if (roomtile != null)
                    {
                        roomtile.AddItemToTile(item);
                    }
                }
            }

            foreach(RoomUnit user in this.Room.RoomUserManager.GetRoomUsers())
            {
                if (user.HasNextStep)
                {
                    this.GetTile(user.NextStepX, user.NextStepY).UsersOnTile.Add(user.VirtualID, user);
                }
                else
                {
                    this.GetTile(user.X, user.Y).UsersOnTile.Add(user.VirtualID, user);
                }
            }
        }

        public bool CoordsInsideRoom(int x, int y)
        {
            return x >= 0 && y >= 0 && x < this.Model.MaxX && y < this.Model.MaxY;
        }

        public RoomTile GetTile(int x, int y)
        {
            if (this.CoordsInsideRoom(x, y))
            {
                return this.Tiles[x, y];
            }
            else
            {
                return null;
            }
        }

        public void UserWalkOff(RoomUnit unit)
        {
            this.Model.Triggers?[unit.X, unit.Y]?.UserWalkOff(unit);
        }

        public void UserWalkOn(RoomUnit unit)
        {
            this.Model.Triggers?[unit.X, unit.Y]?.UserWalkOn(unit);
        }
    }
}
