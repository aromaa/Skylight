using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Pathfinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemOneWayGate : RoomItem
    {
        public RoomUnitUser Interactor;
        public int Tick;

        public RoomItemOneWayGate(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
        }

        public override void OnCycle()
        {
            bool open = false;

            if (this.Interactor != null && this.Interactor.Interacting == this)
            {
                if (this.Tick == 1)
                {
                    if (ThreeDCoord.smethod_0(new ThreeDCoord(this.Interactor.X, this.Interactor.Y), this.TDC))
                    {
                        open = true;

                        this.Tick = 2;
                        this.Interactor.Override = true;
                        this.Interactor.MoveTo(this.TDCO.x, this.TDCO.y);
                    }
                    else
                    {
                        this.Interactor.Interacting = null;
                        this.Interactor = null;
                    }
                }
                else if (this.Tick == 2)
                {
                    this.Interactor.Override = false;
                    this.Interactor.Interacting = null;
                    this.Interactor = null;
                }
            }
            else
            {
                if (this.Interactor != null)
                {
                    this.Interactor.Interacting = null;
                    this.Interactor = null;
                }
            }

            if (open)
            {
                if (this.ExtraData != "1")
                {
                    this.ExtraData = "1";
                    this.UpdateState(false, true);
                }
            }
            else
            {
                if (this.ExtraData != "0")
                {
                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                }
            }
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (session != null)
            {
                RoomUnitUser user = session.GetHabbo().GetRoomSession().GetRoomUser();
                if (user != null)
                {
                    if (ThreeDCoord.smethod_0(new ThreeDCoord(user.X, user.Y), new ThreeDCoord(item.X, item.Y)) || ThreeDCoord.smethod_0(new ThreeDCoord(user.X, user.Y), item.TDC))
                    {
                        RoomItemOneWayGate oneWayGate = (RoomItemOneWayGate)item;
                        if (oneWayGate.Interactor == null)
                        {
                            oneWayGate.Interactor = user;
                            oneWayGate.Tick = 1;
                            user.Interacting = item;
                        }
                    }
                    else
                    {
                        user.MoveTo(item.TDC.x, item.TDC.y);
                    }
                }
            }
        }
    }
}
