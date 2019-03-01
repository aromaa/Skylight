using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItemWater : RoomItem
    {
        public RoomItemWater(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnLoad()
        {
            this.Update();
        }

        public override void OnPlace(GameClient session)
        {
            this.Update();
        }

        public override void OnMove(GameClient session)
        {
            this.Update();
        }

        public override void OnPickup(GameClient session)
        {
            this.ExtraData = "0";
        }

        public void Update()
        {
            int state = 0;

            RoomTile tile2 = this.Room.RoomGamemapManager.GetTile(this.X + 1, this.Y + 2);
            if (tile2 == null || tile2.IsHole)
            {
                state = state | 2;
            }

            RoomTile tile3 = this.Room.RoomGamemapManager.GetTile(this.X, this.Y + 2);
            if (tile3 == null || tile3.IsHole)
            {
                state = state | 4;
            }

            RoomTile tile5 = this.Room.RoomGamemapManager.GetTile(this.X + 2, this.Y + 1);
            if (tile5 == null || tile5.IsHole)
            {
                state = state | 16;
            }

            RoomTile tile6 = this.Room.RoomGamemapManager.GetTile(this.X - 1, this.Y + 1);
            if (tile6 == null || tile6.IsHole)
            {
                state = state | 32;
            }

            RoomTile tile7 = this.Room.RoomGamemapManager.GetTile(this.X + 2, this.Y);
            if (tile7 == null || tile7.IsHole)
            {
                state = state | 64;
            }

            RoomTile tile8 = this.Room.RoomGamemapManager.GetTile(this.X - 1, this.Y);
            if (tile8 == null || tile8.IsHole)
            {
                state = state | 128;
            }

            RoomTile tile10 = this.Room.RoomGamemapManager.GetTile(this.X + 1, this.Y - 1);
            if (tile10 == null || tile10.IsHole)
            {
                state = state | 512;
            }

            RoomTile tile11 = this.Room.RoomGamemapManager.GetTile(this.X, this.Y - 1);
            if (tile11 == null || tile11.IsHole)
            {
                state = state | 1024;
            }

            //CORNERS
            RoomTile tile = this.Room.RoomGamemapManager.GetTile(this.X + 2, this.Y + 2);
            if (tile == null || tile.IsHole)
            {
                if (tile5 != null && !tile5.IsHole && tile2 != null && !tile2.IsHole)
                {
                    state = state | 1;
                }
            }

            RoomTile tile4 = this.Room.RoomGamemapManager.GetTile(this.X - 1, this.Y + 2);
            if (tile4 == null || tile4.IsHole)
            {
                if (tile10 != null && !tile10.IsHole && tile7 != null && !tile7.IsHole)
                {
                    state = state | 8;
                }
            }

            RoomTile tile9 = this.Room.RoomGamemapManager.GetTile(this.X + 2, this.Y - 1);
            if (tile9 == null || tile9.IsHole)
            {
                if (tile6 != null && !tile6.IsHole && tile3 != null && !tile3.IsHole)
                {
                    state = state | 256;
                }
            }

            RoomTile tile12 = this.Room.RoomGamemapManager.GetTile(this.X - 1, this.Y - 1);
            if (tile12 == null || tile12.IsHole)
            {
                if (tile8 != null && !tile8.IsHole && tile11 != null && !tile11.IsHole)
                {
                    state = state | 2048;
                }
            }

            this.ExtraData = state.ToString();
            this.UpdateState(true, true);
        }
    }
}
