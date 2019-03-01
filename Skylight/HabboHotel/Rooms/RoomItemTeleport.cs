using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemTeleport : RoomItem
    {
        public RoomUnitUser Interactor;
        public int Way;

        public RoomItemTeleport(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Interactor = null;
        }

        public override void OnCycle()
        {
            bool open = false;
            bool tele = false;
            if (this.Interactor != null && this.Interactor.Interacting == this)
            {
                if (this.Way == 0) //none
                {
                    this.Interactor.Interacting = null;
                    this.Interactor = null;
                }
                else if (this.Way == 1) //going in
                {
                    if (ThreeDCoord.smethod_0(new ThreeDCoord(this.Interactor.X, this.Interactor.Y), new ThreeDCoord(this.X, this.Y)))
                    {
                        KeyValuePair<uint, uint> teleDate = TeleHandler.GetTeleDestiny(this.ID);
                        if (teleDate.Key != 0 && teleDate.Value != 0)
                        {
                            tele = true;
                            if (teleDate.Value == this.Room.ID)
                            {
                                RoomItemTeleport item = (RoomItemTeleport)this.Room.RoomItemManager.TryGetRoomItem(teleDate.Key);
                                if (item != null)
                                {
                                    this.Interactor.SetLocation(item.X, item.Y, item.Z);
                                    this.Interactor.SetRotation(item.Rot, true);
                                    this.Interactor.Interacting = item;

                                    this.Interactor.Interacting = item;
                                    item.Interactor = this.Interactor;
                                    item.Way = 2;
                                }
                                else
                                {
                                    this.Interactor.RestrictMovementType &=  ~RestrictMovementType.Client;
                                    this.Interactor.Override = false;
                                    this.Interactor.Moving = true;
                                    this.Interactor.MoveTo(this.TDC.x, this.TDC.y);
                                }
                            }
                            else
                            {
                                Room room = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoom(teleDate.Value);
                                if (room != null)
                                {
                                    this.Interactor.Session.GetHabbo().GetRoomSession().HandleTeleport(room, teleDate.Key);
                                }
                                else
                                {
                                    this.Interactor.RestrictMovementType &= ~RestrictMovementType.Client;
                                    this.Interactor.Override = false;
                                    this.Interactor.Moving = true;
                                    this.Interactor.MoveTo(this.TDC.x, this.TDC.y);
                                }
                            }
                        }
                        else
                        {
                            this.Interactor.RestrictMovementType &= RestrictMovementType.Client;
                            this.Interactor.Override = false;
                            this.Interactor.Moving = true;
                            this.Interactor.MoveTo(this.TDC.x, this.TDC.y);
                        }
                        this.Way = 0;
                    }
                    else
                    {
                        if (ThreeDCoord.smethod_0(new ThreeDCoord(this.Interactor.X, this.Interactor.Y), this.TDC))
                        {
                            open = true;

                            this.Interactor.RestrictMovementType |= RestrictMovementType.Client;
                            this.Interactor.Override = true;
                            this.Interactor.Moving = true;
                            this.Interactor.MoveTo(this.TDC.x, this.TDC.y);
                        }
                        else
                        {
                            this.Way = 0;
                        }
                    }
                }
                else if (this.Way == 2) //going out
                {
                    if (this.ExtraData == "2")
                    {
                        open = true;

                        this.Interactor.RestrictMovementType &=  ~RestrictMovementType.Client;
                        this.Interactor.Override = false;
                        this.Interactor.MoveTo(this.TDC.x, this.TDC.y);
                        this.Way = 0;
                    }
                    else
                    {
                        tele = true;
                    }
                }
            }
            else
            {
                if (this.Interactor != null)
                {
                    this.Interactor = null;
                }
                this.Way = 0;
            }

            if (open)
            {
                if (this.ExtraData != "1")
                {
                    this.ExtraData = "1";
                    this.UpdateState(false, true);
                }
            }
            else if (tele)
            {
                if (this.ExtraData != "2")
                {
                    this.ExtraData = "2";
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
                        RoomItemTeleport teleport = (RoomItemTeleport)item;
                        if (teleport.Interactor == null && user.Interacting == null)
                        {
                            teleport.Interactor = user;
                            teleport.Way = 1;
                            user.Interacting = item;
                        }
                    }
                    else
                    {
                        if (user.Interacting == null)
                        {
                            user.MoveTo(item.TDC.x, item.TDC.y);
                        }
                    }
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
