using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItemBattleBanzaiPatch : RoomItem
    {
        public RoomItemBattleBanzaiPatch(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnWalkOn(RoomUnit user)
        {
            this.TileWalkOnLogic(user);
        }

        public void TileWalkOnLogic(RoomUnit user)
        {
            if (this.Room.RoomGameManager.RoomBattleBanzaiManager.GameStarted)
            {
                if (user.IsRealUser && user is RoomUnitUser user_ && user_.GameType == GameType.BattleBanzai)
                {
                    if (user_.GameTeam != GameTeam.None)
                    {
                        int team = (int)user_.GameTeam;
                        int teamBBColor = team * 3;

                        if (this.ExtraData != "5" && this.ExtraData != "8" && this.ExtraData != "11" && this.ExtraData != "14") //locked tile
                        {
                            if (this.ExtraData == "1")
                            {
                                this.ExtraData = teamBBColor.ToString();
                                this.UpdateState(true, true);
                            }
                            else if (this.ExtraData == teamBBColor.ToString())
                            {
                                this.ExtraData = (teamBBColor + 1).ToString();
                                this.UpdateState(true, true);
                            }
                            else if (this.ExtraData == (teamBBColor + 1).ToString())
                            {
                                this.ExtraData = (teamBBColor + 2).ToString();
                                this.UpdateState(true, true);

                                this.LockedTile(user_);
                            }
                            else
                            {
                                this.ExtraData = teamBBColor.ToString();
                                this.UpdateState(true, true);
                            }
                        }
                    }
                }
            }
        }

        public void LockedTile(RoomUnitUser user)
        {
            user.Session.GetHabbo().GetUserStats().BattleBanzaiTilesLocked++;
            //battle banzai quest

            this.Room.RoomGameManager.RoomBattleBanzaiManager.RoomBattleBallGameField.updateLocation(this.X, this.Y, user.GameTeam);
            List<PointField> coords = this.Room.RoomGameManager.RoomBattleBanzaiManager.RoomBattleBallGameField.doUpdate();
            List<Point> checkedPoints = new List<Point>(); //we dont want check same point multiple time or we use huge amount of cpu
            foreach (PointField pointField in coords)
            {
                foreach (Point p in pointField.GetPoints())
                {
                    if (!checkedPoints.Contains(p))
                    {
                        checkedPoints.Add(p);

                        bool tileFound = false;
                        foreach(RoomItem item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPatch)))
                        {
                            if (item.X == p.X && item.Y == p.Y)
                            {
                                if (item.ExtraData != "5" && item.ExtraData != "8" && item.ExtraData != "11" && item.ExtraData != "14") //already locked
                                {
                                    item.ExtraData = (((int)pointField.GameTeam * 3) + 2).ToString();
                                    item.UpdateState(true, true);
                                    this.Room.RoomGameManager.RoomBattleBanzaiManager.AddScore(user, pointField.GameTeam, 1);
                                    tileFound = true;
                                }
                            }
                        }

                        if (tileFound)
                        {
                            user.Session.GetHabbo().GetUserStats().BattleBanzaiTilesLocked++;
                        }
                    }
                }
            }

            user.Session.GetHabbo().GetUserAchievements().CheckAchievement("BattleBanzaiTilesLocked");
            this.Room.RoomGameManager.RoomBattleBanzaiManager.AddScore(user, user.GameTeam, 1);
            this.Room.RoomGameManager.RoomBattleBanzaiManager.UpdateScore(user.GameTeam);

            int tilesLocked = 0;
            foreach (RoomItem item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPatch)))
            {
                if (item.ExtraData == "5" || item.ExtraData == "8" || item.ExtraData == "11" || item.ExtraData == "14") //already locked
                {
                    tilesLocked++;
                }
            }

            if (tilesLocked >= this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPatch)).Count)
            {
                this.Room.RoomGameManager.StopGame();
            }
        }

        public bool TileLocked
        {
            get
            {
                return this.ExtraData == "5" || this.ExtraData == "8" || this.ExtraData == "11" || this.ExtraData == "14";
            }
        }
    }
}
