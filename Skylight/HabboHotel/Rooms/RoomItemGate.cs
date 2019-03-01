using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemGate : RoomItem
    {
        public RoomItemGate(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClients.GameClient session, RoomItem item, int request, bool userHasRights)
        {
            RoomTile tile = this.Room.RoomGamemapManager.GetTile(item.X, item.Y);
            if (tile != null && !tile.IsInUse)
            {
                foreach (AffectedTile tile_ in this.AffectedTiles)
                {
                    if (tile.IsInUse)
                    {
                        return;
                    }
                }

                base.OnUse(session, item, request, userHasRights);

                this.Room.RoomGamemapManager.GetTile(this.X, this.Y).UpdateTile();
                foreach (AffectedTile tile_ in this.AffectedTiles)
                {
                    this.Room.RoomGamemapManager.GetTile(tile_.X, tile_.Y).UpdateTile();
                }
            }
        }
    }
}
