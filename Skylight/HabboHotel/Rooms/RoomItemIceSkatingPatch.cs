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
    class RoomItemIceSkatingPatch : RoomItem
    {
        public RoomItemIceSkatingPatch(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
        }

        public override void OnWalkOn(RoomUnit user)
        {
            if (user.IsRealUser && user is RoomUnitUser user_)
            {
                if (user_.IceSkateStatus == IceSkateStatus.None)
                {
                    this.Room.RoomGameManager.JoinTag(user_);
                }
                else if (user_.IceSkateStatus == IceSkateStatus.Tagged)
                {
                    RoomUnitUser nextTaggedUser = this.GetTaggedUser(user.X + 1, user.Y);
                    if (nextTaggedUser == null) //try get next tagged user
                    {
                        nextTaggedUser = this.GetTaggedUser(user.X - 1, user.Y);
                        if (nextTaggedUser == null)
                        {
                            nextTaggedUser = this.GetTaggedUser(user.X, user.Y + 1);
                            if (nextTaggedUser == null)
                            {
                                nextTaggedUser = this.GetTaggedUser(user.X, user.Y - 1);
                            }
                        }
                    }

                    if (nextTaggedUser != null)
                    {
                        user_.IceSkateStatus = IceSkateStatus.Playing;
                        nextTaggedUser.IceSkateStatus = IceSkateStatus.Tagged;

                        foreach (RoomItemTagPole tagPole in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemTagPole)))
                        {
                            if (tagPole.Tagged == user)
                            {
                                tagPole.Tagged = nextTaggedUser;
                            }
                        }

                        this.Room.RoomGameManager.GiveEffect(user_);
                        this.Room.RoomGameManager.GiveEffect(nextTaggedUser);

                        nextTaggedUser.Session.GetHabbo().GetUserStats().CaughtOnIceTag++;
                        nextTaggedUser.Session.GetHabbo().GetUserAchievements().CheckAchievement("CaughtOnIceTag");
                    }
                }
            }
        }

        public RoomUnitUser GetTaggedUser(int x, int y)
        {
            RoomTile tile = this.Room.RoomGamemapManager.GetTile(x, y);
            if (tile != null)
            {
                return (RoomUnitUser)tile.UsersOnTile.Values.FirstOrDefault(u => u.IsRealUser && ((RoomUnitUser)u).IceSkateStatus == IceSkateStatus.Playing);
            }
            else
            {
                return null;
            }
        }

        public override void OnWalkOff(RoomUnit user)
        {
            RoomTile nextTile = this.Room.RoomGamemapManager.GetTile(user.NextStepX, user.NextStepY);
            if (!(nextTile.HigestRoomItem is RoomItemIceSkatingPatch))
            {
                if (user.IsRealUser && user is RoomUnitUser user_)
                {
                    this.Room.RoomGameManager.LeaveTag(user_);
                }
            }
        }
    }
}
