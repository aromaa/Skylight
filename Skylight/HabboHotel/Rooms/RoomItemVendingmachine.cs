using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemVendingmachine : RoomItem
    {
        public RoomUnitUser Interactor;

        public RoomItemVendingmachine(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Interactor = null;
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    if (this.ExtraData == "1")
                    {
                        if (this.Interactor != null)
                        {
                            this.Interactor.RestrictMovementType &= ~RestrictMovementType.Client;
                            this.Interactor.SetHanditem(this.GetBaseItem().VendingIds[RandomUtilies.GetRandom(0, this.GetBaseItem().VendingIds.Length - 1)]);
                        }

                        this.Interactor = null;
                        this.ExtraData = "0";
                        this.UpdateState(false, true);
                    }
                }
            }
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (this.ExtraData != "1" && this.GetBaseItem().VendingIds.Length > 0 && this.Interactor == null)
            {
                if (CoordUtilies.InRange(this.X, this.Y, session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y))
                {
                    this.Interactor = session.GetHabbo().GetRoomSession().GetRoomUser();
                    this.Interactor.RestrictMovementType |= RestrictMovementType.Client;
                    this.Interactor.SetRotation(WalkRotation.Walk(this.Interactor.X, this.Interactor.Y, this.X, this.Y), !this.Interactor.Session.GetHabbo().GetRoomSession().GetRoomUser().HasStatus("sit"));

                    this.ExtraData = "1";
                    this.UpdateState(false, true);
                    this.DoUpdate(2);
                }
                else
                {
                    session.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(this.TDC.x, this.TDC.y);
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
    }
}
