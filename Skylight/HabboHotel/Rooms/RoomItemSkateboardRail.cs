using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemSkateboardRail : RoomItem
    {
        public RoomItemSkateboardRail(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {

        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {

        }

        public override void AboutToWalkOn(RoomUnit user)
        {
            if (user.IsRealUser && user is RoomUnitUser user_ && user_.ActiveEffect == 71) //are we even skateboarding?!?
            {
                RoomTile lastTile = this.Room.RoomGamemapManager.GetTile(user.X, user.Y);
                RoomTile nextTile = this.Room.RoomGamemapManager.GetTile(user.NextStepX, user.NextStepY);

                if (nextTile.HigestRoomItem is RoomItemSkateboardRail) //next tile aka this is skateboard rail.. yeah
                {
                    if (lastTile.HigestRoomItem is RoomItemSkateboardRail) //if the LAST tile was skateboard rail too
                    {
                        if (nextTile.HigestRoomItem.Rot != lastTile.HigestRoomItem.Rot) //are we performing jump?
                        {
                            if (nextTile.HigestRoomItem.Rot == 0)
                            {
                                if (user.HeadRotation != 0 && user.HeadRotation != 4)
                                {
                                    user_.Session.GetHabbo().GetUserStats().SkateboardJumper++;
                                    user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardJumper");
                                }
                                else
                                {
                                    user_.Session.GetHabbo().GetUserStats().SkateboardSlider++;
                                    user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardSlider");
                                }
                            }
                            else if (nextTile.HigestRoomItem.Rot == 2)
                            {
                                if (user.HeadRotation != 2 && user.HeadRotation != 6)
                                {
                                    user_.Session.GetHabbo().GetUserStats().SkateboardJumper++;
                                    user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardJumper");
                                }
                                else
                                {
                                    user_.Session.GetHabbo().GetUserStats().SkateboardSlider++;
                                    user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardSlider");
                                }
                            }
                        }
                        else //we are performing slide, its same item rotation as last tile!
                        {
                            if (user.HeadRotation != 2 && user.HeadRotation != 6)
                            {
                                user_.Session.GetHabbo().GetUserStats().SkateboardSlider++;
                                user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardSlider");
                            }
                        }
                    }
                    else
                    {
                        if (nextTile.HigestRoomItem.Rot == 0)
                        {
                            if (user.HeadRotation != 0 && user.HeadRotation != 4)
                            {
                                user_.Session.GetHabbo().GetUserStats().SkateboardJumper++;
                                user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardJumper");
                            }
                            else
                            {
                                user_.Session.GetHabbo().GetUserStats().SkateboardSlider++;
                                user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardSlider");
                            }
                        }
                        else if (nextTile.HigestRoomItem.Rot == 2)
                        {
                            if (user.HeadRotation != 2 && user.HeadRotation != 6)
                            {
                                user_.Session.GetHabbo().GetUserStats().SkateboardJumper++;
                                user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardJumper");
                            }
                            else
                            {
                                user_.Session.GetHabbo().GetUserStats().SkateboardSlider++;
                                user_.Session.GetHabbo().GetUserAchievements().CheckAchievement("SkateboardSlider");
                            }
                        }
                    }

                    //move user to next tile
                    if (nextTile.HigestRoomItem.Rot == 0)
                    {
                        if (user.HeadRotation == 0)
                        {
                            if (user.TargetX == nextTile.HigestRoomItem.X && user.TargetY == nextTile.HigestRoomItem.Y)
                            {
                                user.MoveTo(nextTile.HigestRoomItem.X, nextTile.HigestRoomItem.Y - 1);
                            }

                            user_.SkateboardRotation = 6;
                        }
                        else if (user.HeadRotation == 4)
                        {
                            if (user.TargetX == nextTile.HigestRoomItem.X && user.TargetY == nextTile.HigestRoomItem.Y)
                            {
                                user.MoveTo(nextTile.HigestRoomItem.X, nextTile.HigestRoomItem.Y + 1);
                            }

                            user_.SkateboardRotation = 2;
                        }
                        else
                        {
                            user_.SkateboardRotation = null;
                        }
                    }
                    else if (nextTile.HigestRoomItem.Rot == 2)
                    {
                        if (user.HeadRotation == 2)
                        {
                            if (user.TargetX == nextTile.HigestRoomItem.X && user.TargetY == nextTile.HigestRoomItem.Y)
                            {
                                user.MoveTo(nextTile.HigestRoomItem.X + 1, nextTile.HigestRoomItem.Y);
                            }

                            user_.SkateboardRotation = 0;
                        }
                        else if (user.HeadRotation == 6)
                        {
                            if (user.TargetX == nextTile.HigestRoomItem.X && user.TargetY == nextTile.HigestRoomItem.Y)
                            {
                                user.MoveTo(nextTile.HigestRoomItem.X - 1, nextTile.HigestRoomItem.Y);
                            }

                            user_.SkateboardRotation = 4;
                        }
                        else
                        {
                            user_.SkateboardRotation = null;
                        }
                    }
                    else
                    {
                        user_.SkateboardRotation = null;
                    }

                    user.NeedUpdate = true;
                }
                else
                {
                    user_.SkateboardRotation = null;
                }
            }
        }
    }
}
