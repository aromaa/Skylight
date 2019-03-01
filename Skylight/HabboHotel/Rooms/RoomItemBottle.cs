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
    class RoomItemBottle : RoomItem
    {
        public RoomItemBottle(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (item.ExtraData != "-1")
            {
                item.ExtraData = "-1";
                item.UpdateState(false, true);
                item.DoUpdate(3);
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

                    this.ExtraData = RandomUtilies.GetRandom(0, 7).ToString();
                    this.UpdateState(true, true);
                }
            }
        }

        public override void OnLoad()
        {
            this.ExtraData = "0";
            this.UpdateState(false, true);
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

        public override void OnMove(GameClient session)
        {
            this.ExtraData = "0";
            this.UpdateState(false, true);
        }
    }
}
