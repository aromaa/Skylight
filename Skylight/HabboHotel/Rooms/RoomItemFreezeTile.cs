using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemFreezeTile : RoomItem
    {
        public RoomItemFreezeTile(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

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

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (session != null)
            {
                if (this.Room.RoomFreezeManager.GameStarted)
                {
                    FreezePlayer player = session.GetHabbo().GetRoomSession().GetRoom().RoomFreezeManager.TryGetFreezePlayer(session.GetHabbo().ID);
                    if (player != null && !player.Freezed && player.Balls > 0)
                    {
                        if (Math.Abs(player.User.X - this.X) < 2 && Math.Abs(player.User.Y - this.Y) < 2)
                        {
                            RoomItem iceBlock = session.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.FloorItems.Get(typeof(RoomItemFreezeIceBlock)).FirstOrDefault(b => b.X == this.X && b.Y == this.Y);
                            if (iceBlock == null || (!string.IsNullOrEmpty(iceBlock.ExtraData) && iceBlock.ExtraData != "0"))
                            {
                                if (!session.GetHabbo().GetRoomSession().GetRoom().RoomFreezeManager.Balls.Any(b => b.Source == this))
                                {
                                    player.Balls--;

                                    if (player.BallType == FreezeBallType.Mega)
                                    {
                                        this.ExtraData = "6000";
                                    }
                                    else
                                    {
                                        this.ExtraData = "1000";
                                    }

                                    session.GetHabbo().GetRoomSession().GetRoom().RoomFreezeManager.Balls.Add(new FreezeBall(player.Range, player.BallType, this, player));
                                    player.BallType = FreezeBallType.Normal;

                                    this.UpdateState(false, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
