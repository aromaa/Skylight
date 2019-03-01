using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemPuzzleBox : RoomItem
    {
        public RoomItemPuzzleBox(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            ThreeDCoord userLocation = new ThreeDCoord(session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y);
            ThreeDCoord test1 = new ThreeDCoord(this.X + 1, this.Y);
            ThreeDCoord test2 = new ThreeDCoord(this.X - 1, this.Y);
            ThreeDCoord test3 = new ThreeDCoord(this.X, this.Y + 1);
            ThreeDCoord test4 = new ThreeDCoord(this.X, this.Y - 1);

            int x = this.X;
            int y = this.Y;

            if (ThreeDCoord.smethod_0(test1, userLocation))
            {
                x--;
            }
            else if (ThreeDCoord.smethod_0(test2, userLocation))
            {
                x++;
            }
            else if (ThreeDCoord.smethod_0(test3, userLocation))
            {
                y--;
            }
            else if (ThreeDCoord.smethod_0(test4, userLocation))
            {
                y++;
            }
            else
            {
                if ((session.GetHabbo().GetRoomSession().GetRoomUser().RestrictMovementType & RestrictMovementType.Client) == 0)
                {
                    session.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(this.X, this.Y);
                }

                return;
            }
            
            RoomTile tile = this.Room.RoomGamemapManager.GetTile(x, y);
            if (tile != null && (tile.HigestRoomItem == null || tile.HigestRoomItem.GetBaseItem().Walkable))
            {
                int oldX = this.X;
                int oldY = this.Y;
                double oldZ = this.Z;

                if (this.Room.RoomItemManager.MoveFloorItemOnRoom(session, item, x, y, this.Rot))
                {
                    this.Room.RoomItemManager.MoveAnimation[this.ID] = new RoomItemRollerMovement(this.ID, this.X, this.Y, this.Z, 0, oldX, oldY, oldZ);
                }
            }
        }
    }
}
