using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemBattleBanzaiRandomTeleport : RoomItem
    {
        public RoomItemBattleBanzaiRandomTeleport(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnWalkOn(RoomUnit user)
        {
            this.ExtraData = "1";
            this.UpdateState(false, true);
            this.DoUpdate(1);

            IEnumerable<RoomItem> battleBanzaiTeleports = this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiRandomTeleport)).Where(i => i.ID != this.ID);
            if (battleBanzaiTeleports.Count() > 0)
            {
                RoomItem item = battleBanzaiTeleports.ElementAt(RandomUtilies.GetRandom(0, battleBanzaiTeleports.Count() - 1));
                item.ExtraData = "1";
                item.UpdateState(false, true);
                item.DoUpdate(1);

                user.StopMoving();
                user.SetLocation(item.X, item.Y, item.Z); //set new location
                user.UpdateState();
            }
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (this.ExtraData != "0")
            {
                this.ExtraData = "0";
                this.UpdateState(true, true);
            }
        }

        public override void OnLoad()
        {
            this.ExtraData = "0";
        }

        public override void OnPickup(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnPlace(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                }
            }
        }
    }
}
