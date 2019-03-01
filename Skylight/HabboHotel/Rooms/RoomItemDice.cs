using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemDice : RoomItem
    {
        public RoomItemDice(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (session != null)
            {
                if (CoordUtilies.InRange(this.X, this.Y, session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y))
                {
                    if (this.ExtraData != "-1")
                    {
                        if (request == -1)
                        {
                            this.ExtraData = "0";
                            this.UpdateState(true, true);
                        }
                        else
                        {
                            this.ExtraData = "-1";
                            this.UpdateState(false, true);
                            this.DoUpdate(4);
                        }
                    }
                }
                else
                {
                    session.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(this.TDC.x, this.TDC.y);
                }
            }
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    this.ExtraData = RandomUtilies.GetRandom(1, 6).ToString();
                    this.UpdateState(true, true);
                }
            }
        }

        public override void OnPlace(GameClient session)
        {
            this.ExtraData = "0";
            this.UpdateState(false, true);
        }

        public override void OnPickup(GameClient session)
        {
            this.ExtraData = "0";
        }
    }
}
