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
    class RoomItemFreezeIceBlock : RoomItem
    {
        public RoomItemFreezeIceBlock(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (!string.IsNullOrEmpty(this.ExtraData) && this.ExtraData != "0")
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
                                RoomItem freezeTile = session.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.FloorItems.Get(typeof(RoomItemFreezeIceBlock)).FirstOrDefault(t => t.X == this.X && t.Y == this.Y);
                                if (freezeTile != null)
                                {
                                    if (!session.GetHabbo().GetRoomSession().GetRoom().RoomFreezeManager.Balls.Any(b => b.Source == freezeTile))
                                    {
                                        player.Balls--;

                                        if (player.BallType == FreezeBallType.Mega)
                                        {
                                            freezeTile.ExtraData = "6000";
                                        }
                                        else
                                        {
                                            freezeTile.ExtraData = "1000";
                                        }

                                        session.GetHabbo().GetRoomSession().GetRoom().RoomFreezeManager.Balls.Add(new FreezeBall(player.Range, player.BallType, freezeTile, player));
                                        player.BallType = FreezeBallType.Normal;

                                        freezeTile.UpdateState(false, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnWalkOn(RoomUnit user)
        {
            if (this.ExtraData == "2000" || this.ExtraData == "3000" || this.ExtraData == "4000" || this.ExtraData == "5000" || this.ExtraData == "6000" || this.ExtraData == "7000")
            {
                if (user.IsRealUser && user is RoomUnitUser user_)
                {
                    FreezePlayer player = this.Room.RoomFreezeManager.TryGetFreezePlayer(user_.Session.GetHabbo().ID);
                    if (player != null)
                    {
                        switch (this.ExtraData)
                        {
                            case "2000":
                                player.Range++;
                                break;
                            case "3000":
                                player.Balls++;
                                break;
                            case "4000":
                                player.BallType = FreezeBallType.Diagonal;
                                break;
                            case "5000":
                                player.BallType = FreezeBallType.Mega;
                                break;
                            case "6000":
                                if (player.Lives < 4)
                                {
                                    player.Lives++;
                                    this.Room.RoomFreezeManager.UpdateScoreboards();
                                }
                                player.ShowLives();
                                break;
                            case "7000":
                                player.ActiveShield();
                                break;
                            default:
                                return;
                        }
                    }

                    user_.Session.GetHabbo().GetUserStats().FreezePowerUpCollector++;
                    user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("FreezePowerUpCollector");
                }

                this.ExtraData = "1" + this.ExtraData;
                this.UpdateState(true, true);
            }
        }
    }
}
