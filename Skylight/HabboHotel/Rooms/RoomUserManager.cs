using SkylightEmulator.Collections;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomUserManager
    {
        internal ConcurrentListDictionary<int, Type, RoomUnit> RoomUsers; //temp internal
        internal ConcurrentDictionary<int, RoomUnit> MovingUsers; //temp internal
        private Dictionary<uint, double> Bans;

        private Room Room;
        private int NextVirtualID = 0;
        private int AddNewbieGuideTimer = 0;
        private Stopwatch LastCycle;

        public RoomUserManager(Room room)
        {
            this.RoomUsers = new ConcurrentListDictionary<int, Type, RoomUnit>();
            this.MovingUsers = new ConcurrentDictionary<int, RoomUnit>();
            this.Bans = new Dictionary<uint, double>();

            this.Room = room;
            this.LastCycle = Stopwatch.StartNew();
        }

        public int UsersInRoom
        {
            get
            {
                return this.GetRealUsers().Count;
            }
        }

        public int GetNextVirtualID()
        {
            return this.NextVirtualID++;
        }

        public void OnCycle()
        {
            double timeSinceLastRoomCycle = this.LastCycle.Elapsed.TotalSeconds;
            this.LastCycle.Restart();

            foreach(RoomUnit unit in this.MovingUsers.Values)
            {
                this.Room.ThrowIfRoomCycleCancalled("Cycle moving room units", unit); //Have room cycle cancalled?

                unit.MoveCycle();
            }

            double achievementRoomHost = 0;
            foreach (RoomUnit unit in this.RoomUsers.Values)
            {
                this.Room.ThrowIfRoomCycleCancalled("Cycle room units", unit); //Have room cycle cancalled?

                unit.Cycle();

                if (unit.IsRealUser && unit is RoomUnitUser user)
                {
                    if (user.IceSkateStatus >= IceSkateStatus.Playing)
                    {
                        user.Session.GetHabbo().GetUserStats().IceIceBaby += timeSinceLastRoomCycle;
                        user.Session.GetHabbo().GetUserAchievements().CheckAchievement("IceIceBaby");
                    }

                    if (user.Rollerskate)
                    {
                        user.Session.GetHabbo().GetUserStats().RollerDerby += timeSinceLastRoomCycle;
                        user.Session.GetHabbo().GetUserAchievements().CheckAchievement("RollerDerby");
                    }

                    if (user.Session.GetHabbo().ID != this.Room.RoomData.OwnerID)
                    {
                        achievementRoomHost += timeSinceLastRoomCycle;
                    }
                }

                //last
                (unit as BotAI)?.OnRoomCycle();
            }

            if (this.AddNewbieGuideTimer > 0)
            {
                if (--this.AddNewbieGuideTimer <= 0)
                {
                    if (this.RoomUsers.Get(typeof(RoomBotNewbieGuide)).Count <= 0 && this.RoomUsers.Get(typeof(RoomUnitUser)).Any(u => ((RoomUnitUser)u).Session.GetHabbo().ID == this.Room.RoomData.OwnerID))
                    {
                        this.AddAIUser(new RoomBotData(0, this.Room.RoomData.ID, "Jonny", "/", "Dev", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, null), 2, this.Room.RoomGamemapManager.Model.DoorX, this.Room.RoomGamemapManager.Model.DoorY, this.Room.RoomGamemapManager.Model.DoorZ, this.Room.RoomGamemapManager.Model.DoorDir);
                    }
                }
            }

            if (achievementRoomHost > 0)
            {
                this.Room.RoomHost(achievementRoomHost);
            }

            //List<uint> idleKickUsers = new List<uint>();
            //foreach (RoomUser user in this.MovingUsers.Values)
            //{
            //    this.Room.ThrowIfRoomCycleCancalled("Cycle moving room users", user); //Have room cycle cancalled?

            //    if (user.NextStep && user.Riding == null)
            //    {
            //        this.WalkOff(user);
            //        if (user.NextStep) //did walkoff prevent for moving to next tile?
            //        {
            //            this.AboutToWalkOn(user);

            //            user.NextStep = false;
            //            user.SetLocation(user.NextStepX, user.NextStepY, 0, true, false); //we set height on EnterTile

            //            (user as RoomPet)?.Rider?.SetLocation(user.X, user.Y, user.Z + 1, true, false);

            //            this.UpdateUserStateOnTile(user);
            //            this.WalkOn(user);

            //            if (user.IsRealUser && user.X == this.Room.RoomGamemapManager.Model.DoorX && user.Y == this.Room.RoomGamemapManager.Model.DoorY && !idleKickUsers.Contains(user.GetClient().GetHabbo().ID))
            //            {
            //                idleKickUsers.Add(user.GetClient().GetHabbo().ID);
            //            }
            //        }
            //    }

            //    if (user.Moving)
            //    {
            //        if (!user.Freezed && user.Riding == null)
            //        {
            //            RoomTile tile = DreamPathfinder.GetNearestRoomTile(new Point(user.X, user.Y), new Point(user.TargetX, user.TargetY), user.Z, this.Room, !this.Room.DisableDiagonal, user, this.Room.RoomData.AllowWalkthrough);
            //            if (tile.X != user.X || tile.Y != user.Y)
            //            {
            //                user.CurrentTile.UsersOnTile.Remove(user.VirtualID); //last tile

            //                int x = tile.X;
            //                int y = tile.Y;
            //                double z = tile.GetZ(!tile.IsSeat && !tile.IsBed);

            //                this.Room.RoomGamemapManager.GetTile(x, y).UsersOnTile.Add(user.VirtualID, user); //new tile

            //                user.NextStep = true;
            //                user.NextStepX = x;
            //                user.NextStepY = y;

            //                int rotation = user.Moonwalk ? WalkRotation.Moonwalk(user.X, user.Y, x, y) : WalkRotation.Walk(user.X, user.Y, x, y);
            //                user.SetRotation(rotation, true);

            //                user.RemoveStatus("mv");
            //                user.RemoveStatus("sit");
            //                user.RemoveStatus("lay");

            //                RoomPet pet = user as RoomPet;
            //                if (pet != null)
            //                {
            //                    if (pet.JumpStatus == HorseJumpStatus.ABOUT_TO_JUMP && tile.HigestRoomItem is RoomItemHorseObstacle && ((RoomItemHorseObstacle)tile.HigestRoomItem).IsMiddlePart(x, y))
            //                    {
            //                        user.AddStatus("mv", string.Concat(new object[] { x, ",", y, ",", TextUtilies.DoubleWithDotDecimal(z), "///jmp" }));
            //                    }
            //                    else
            //                    {
            //                        user.AddStatus("mv", string.Concat(new object[] { x, ",", y, ",", TextUtilies.DoubleWithDotDecimal(z) }));
            //                    }

            //                    if (pet.Rider != null)
            //                    {
            //                        this.Room.RoomGamemapManager.GetTile(pet.Rider.X, pet.Rider.Y).UsersOnTile.Remove(pet.Rider.VirtualID);
            //                        this.Room.RoomGamemapManager.GetTile(x, y).UsersOnTile.Add(pet.Rider.VirtualID, pet.Rider);

            //                        pet.Rider.SetRotation(rotation, true);
            //                        pet.Rider.RemoveStatus("mv");
            //                        pet.Rider.AddStatus("mv", string.Concat(new object[] { x, ",", y, ",", TextUtilies.DoubleWithDotDecimal(z + 1) }));
            //                    }
            //                }
            //                else
            //                {
            //                    if (tile.HigestRoomItem is RoomItemSkateboardRail) //make user face correct rotation on rails
            //                    {
            //                        if (tile.HigestRoomItem.Rot == 0)
            //                        {
            //                            if (user.HeadRotation == 0)
            //                            {
            //                                user.SkateboardRotation = 6;
            //                            }
            //                            else if (user.HeadRotation == 4)
            //                            {
            //                                user.SkateboardRotation = 2;
            //                            }
            //                            else
            //                            {
            //                                user.SkateboardRotation = null;
            //                            }
            //                        }
            //                        else if (tile.HigestRoomItem.Rot == 2)
            //                        {
            //                            if (user.HeadRotation == 2)
            //                            {
            //                                user.SkateboardRotation = 0;
            //                            }
            //                            else if (user.HeadRotation == 6)
            //                            {
            //                                user.SkateboardRotation = 4;
            //                            }
            //                            else
            //                            {
            //                                user.SkateboardRotation = null;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            user.SkateboardRotation = null;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        user.SkateboardRotation = null;
            //                    }

            //                    user.AddStatus("mv", string.Concat(new object[] { x, ",", y, ",", TextUtilies.DoubleWithDotDecimal(z) }));
            //                }
            //            }
            //            else
            //            {
            //                user.Moving = false;
            //                user.RemoveStatus("mv");

            //                (user as RoomPet)?.Rider?.RemoveStatus("mv");
            //            }
            //        }
            //    }
            //    else
            //    {
            //        this.MovingUsers.TryRemove(user.VirtualID, out RoomUser trash);
            //    }
            //}

            //foreach (RoomUser user in this.RoomUsers.Values)
            //{
            //    this.Room.ThrowIfRoomCycleCancalled("Cycle room units", user); //Have room cycle cancalled?

            //    if (user != null)
            //    {
            //        //if (user.Sleeps)
            //        //{
            //        //    if (user.IsRealUser && !this.Room.HaveOwnerRights(user.GetClient()) && user.IdleTime.Elapsed.TotalSeconds >= ServerConfiguration.IdleKickTime)
            //        //    {
            //        //        idleKickUsers.Add(user.GetClient().GetHabbo().ID);
            //        //    }
            //        //}
            //        //else
            //        //{
            //        //    if (user.IdleTime.Elapsed.TotalSeconds >= ServerConfiguration.IdleTime)
            //        //    {
            //        //        user.Sleeps = true;

            //        //        this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Idle, new ValueHolder("VirtualID", user.VirtualID, "Sleep", true)));
            //        //    }
            //        //}

            //        //if (user.TempEffectTimer > 0)
            //        //{
            //        //    if (--user.TempEffectTimer <= 0)
            //        //    {
            //        //        user.ApplyEffect(user.EffectID);
            //        //    }
            //        //}

            //        //if (user.UpdateUserTileTimer > 0)
            //        //{
            //        //    if (--user.UpdateUserTileTimer <= 0)
            //        //    {
            //        //        this.UpdateUserStateOnTile(user);
            //        //    }
            //        //}

            //        //if (user.Handitem > 0)
            //        //{
            //        //    if (--user.HanditemTimer <= 0)
            //        //    {
            //        //        user.SetHanditem(0);
            //        //    }
            //        //}

            //        //if (user.StatussesLifetime.Count > 0)
            //        //{
            //        //    foreach(KeyValuePair<string, double> timeouts in user.StatussesLifetime.ToList())
            //        //    {
            //        //        if (TimeUtilies.GetUnixTimestamp() >= timeouts.Value)
            //        //        {
            //        //            user.RemoveStatus(timeouts.Key);
            //        //            user.StatussesLifetime.Remove(timeouts.Key);
            //        //        }
            //        //    }
            //        //}

            //        //if (user.IsRealUser)
            //        //{
            //        //    if (user.IceSkateStatus >= IceSkateStatus.Playing)
            //        //    {
            //        //        user.GetClient().GetHabbo().GetUserStats().IceIceBaby += timeSinceLastRoomCycle;
            //        //        user.GetClient().GetHabbo().GetUserAchievements().CheckAchievement("IceIceBaby");
            //        //    }

            //        //    if (user.Rollerskate)
            //        //    {
            //        //        user.GetClient().GetHabbo().GetUserStats().RollerDerby += timeSinceLastRoomCycle;
            //        //        user.GetClient().GetHabbo().GetUserAchievements().CheckAchievement("RollerDerby");
            //        //    }

            //        //    if (this.Room.RoomData.OwnerID != user.GetClient().GetHabbo().ID)
            //        //    {
            //        //        achievementRoomHost += timeSinceLastRoomCycle;
            //        //    }
            //        //}

            //        ////last
            //        //(user as BotAI)?.OnRoomCycle();
            //    }
            //}

            //if (idleKickUsers.Count > 0)
            //{
            //    foreach (uint userId in idleKickUsers)
            //    {
            //        this.Room.ThrowIfRoomCycleCancalled("Cycle idle kick users", userId); //Have room cycle cancalled?

            //        this.LeaveRoom(Skylight.GetGame().GetGameClientManager().GetGameClientById(userId), true);
            //    }
            //}
        }

        //public void UpdateUserStateOnTile(RoomUser user)
        //{
        //    if (user != null)
        //    {
        //        RoomTile tile = user.CurrentTile;
        //        if (tile != null)
        //        {
        //            user.RemoveStatus("sit");
        //            user.RemoveStatus("lay");

        //            if (tile.IsSeat)
        //            {
        //                user.SetLocation(user.X, user.Y, tile.GetZ(false), true, false);

        //                if (tile.HigestRoomItem != null)
        //                {
        //                    user.AddStatus("sit", TextUtilies.DoubleWithDotDecimal(tile.HigestRoomItem.BaseItem.Height));
        //                    user.SetRotation(tile.HigestRoomItem.Rot, true);
        //                }
        //                else
        //                {
        //                    user.AddStatus("sit", TextUtilies.DoubleWithDotDecimal(1.0));
        //                    user.SetRotation(this.Room.RoomGamemapManager.Model.Rotation[user.X, user.Y], true);
        //                }
        //            }
        //            else if (tile.IsBed)
        //            {
        //                user.AddStatus("lay", TextUtilies.DoubleWithDotDecimal(tile.HigestRoomItem.BaseItem.Height));
        //                user.SetLocation(user.X, user.Y, tile.GetZ(false), true, false);
        //                user.SetRotation(tile.HigestRoomItem.Rot, true);
        //            }
        //            else
        //            {
        //                user.SetLocation(user.X, user.Y, tile.GetZ(true), true, false);
        //            }

        //            if (!(user is RoomPet))
        //            {
        //                RoomItem higestRoomItem = tile.HigestRoomItem;
        //                if (higestRoomItem != null)
        //                {
        //                    string gender = user.IsRealUser ? user.GetClient().GetHabbo().Gender : "M";
        //                    int effectId = gender == "M" ? higestRoomItem.GetBaseItem().EffectM : higestRoomItem.GetBaseItem().EffectF;
        //                    if (effectId > 0)
        //                    {
        //                        if (user.ActiveEffect != effectId)
        //                        {
        //                            user.ApplyEffect(effectId);
        //                            user.EffectGaveByItem = effectId;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (user.EffectGaveByItem > 0)
        //                        {
        //                            if (user.ActiveEffect != -1)
        //                            {
        //                                user.ApplyEffect(-1);
        //                                user.EffectGaveByItem = -1;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (user.EffectGaveByItem > 0)
        //                    {
        //                        if (user.ActiveEffect != -1)
        //                        {
        //                            user.ApplyEffect(-1);
        //                            user.EffectGaveByItem = -1;
        //                        }
        //                    }
        //                }

        //                if (higestRoomItem is RoomItemFootballGate)
        //                {
        //                    RoomItemFootballGate gate = (RoomItemFootballGate)higestRoomItem;

        //                    FigureParts gateFigureParts = new FigureParts(user.IsRealUser ? (user.GetClient().GetHabbo().Gender.ToLower() == "m" ? gate.Data[0] : gate.Data[1]) : gate.Data[0]);
        //                    FigureParts userCurrentFigureParts = new FigureParts(user.IsRealUser ? user.GetClient().GetHabbo().Look : (user as RoomBot).Data.Look);

        //                    bool changedSomething = false;
        //                    foreach (KeyValuePair<string, Dictionary<string, object>> data in gateFigureParts.GetParts())
        //                    {
        //                        bool changePart = false;

        //                        Dictionary<string, object> data_ = userCurrentFigureParts.GetPart(data.Key);
        //                        if (data_ != null)
        //                        {
        //                            if (data_["setid"] != data.Value["setid"])
        //                            {
        //                                changePart = true;
        //                            }
        //                            else
        //                            {
        //                                List<string> userColors = data_["colorids"] as List<string>;
        //                                List<string> gateColors = data.Value["colorids"] as List<string>;
        //                                if (userColors != null && gateColors != null && userColors.Count == gateColors.Count)
        //                                {
        //                                    for(int i = 0; i < userColors.Count; i++)
        //                                    {
        //                                        if (userColors[i] != gateColors[i])
        //                                        {
        //                                            changePart = true;
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    changePart = true;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            changePart = true;
        //                        }

        //                        if (changePart)
        //                        {
        //                            changedSomething = true;

        //                            userCurrentFigureParts.ReplacePart(data.Value);
        //                        }
        //                    }

        //                    if (!user.FootballGateFigureActive)
        //                    {
        //                        if (changedSomething)
        //                        {
        //                            user.FootballGateFigureActive = true;

        //                            string figure = userCurrentFigureParts.GetPartString();
        //                            if (user.IsRealUser)
        //                            {
        //                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
        //                                message_.Init(r63aOutgoing.UpdateUser);
        //                                message_.AppendInt32(-1);
        //                                message_.AppendString(figure);
        //                                message_.AppendString(user.GetClient().GetHabbo().Gender.ToLower());
        //                                message_.AppendString(user.GetClient().GetHabbo().Motto);
        //                                message_.AppendInt32(user.GetClient().GetHabbo().GetUserStats().AchievementPoints);
        //                                user.GetClient().SendMessage(message_);
        //                            }

        //                            ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
        //                            message_2.Init(r63aOutgoing.UpdateUser);
        //                            message_2.AppendInt32(user.VirtualID);
        //                            message_2.AppendString(figure);
        //                            message_2.AppendString(user.IsRealUser ? user.GetClient().GetHabbo().Gender.ToLower() : "m");
        //                            message_2.AppendString(user.IsRealUser ? user.GetClient().GetHabbo().Motto : (user as RoomBot).Data.Motto);
        //                            message_2.AppendInt32(0);
        //                            this.Room.SendToAll(message_2);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (user.FootballGateFigureActive)
        //                        {
        //                            user.FootballGateFigureActive = false;

        //                            if (user.IsRealUser)
        //                            {
        //                                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
        //                                message_.Init(r63aOutgoing.UpdateUser);
        //                                message_.AppendInt32(-1);
        //                                message_.AppendString(user.GetClient().GetHabbo().Look);
        //                                message_.AppendString(user.GetClient().GetHabbo().Gender.ToLower());
        //                                message_.AppendString(user.GetClient().GetHabbo().Motto);
        //                                message_.AppendInt32(user.GetClient().GetHabbo().GetUserStats().AchievementPoints);
        //                                user.GetClient().SendMessage(message_);
        //                            }

        //                            ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
        //                            message_2.Init(r63aOutgoing.UpdateUser);
        //                            message_2.AppendInt32(user.VirtualID);
        //                            message_2.AppendString(user.IsRealUser ? user.GetClient().GetHabbo().Look : (user as RoomBot).Data.Look);
        //                            message_2.AppendString(user.IsRealUser ? user.GetClient().GetHabbo().Gender.ToLower() : "m");
        //                            message_2.AppendString(user.IsRealUser ? user.GetClient().GetHabbo().Motto : (user as RoomBot).Data.Motto);
        //                            message_2.AppendInt32(0);
        //                            this.Room.SendToAll(message_2);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public void EnterRoom(GameClient gameClient)
        {
            RoomUnitUser roomUser = new RoomUnitUser(gameClient, this.Room, this.GetNextVirtualID());
            if (roomUser.Session.GetHabbo() != null)
            {
                this.RoomUsers.Add(roomUser.VirtualID, roomUser.GetType(), roomUser);

                if (gameClient.GetHabbo().GetRoomSession().TargetTeleportID == 0)
                {
                    roomUser.SetLocation(this.Room.RoomGamemapManager.Model.DoorX, this.Room.RoomGamemapManager.Model.DoorY, this.Room.RoomGamemapManager.Model.DoorZ);
                    roomUser.SetRotation(this.Room.RoomGamemapManager.Model.DoorDir, true);
                }
                else
                {
                    RoomItemTeleport item = (RoomItemTeleport)this.Room.RoomItemManager.TryGetRoomItem(gameClient.GetHabbo().GetRoomSession().TargetTeleportID);
                    if (item != null)
                    {
                        roomUser.SetLocation(item.X, item.Y, item.Z);
                        roomUser.SetRotation(item.Rot, true);

                        roomUser.Interacting = item;
                        item.Interactor = roomUser;
                        item.Way = 2;
                    }
                    else
                    {
                        roomUser.SetLocation(this.Room.RoomGamemapManager.Model.DoorX, this.Room.RoomGamemapManager.Model.DoorY, this.Room.RoomGamemapManager.Model.DoorZ);
                        roomUser.SetRotation(this.Room.RoomGamemapManager.Model.DoorDir, true);
                    }
                }
                gameClient.GetHabbo().GetRoomSession().TargetTeleportID = 0;

                if (this.Room.HaveOwnerRights(gameClient))
                {
                    roomUser.AddStatus("flatctrl", "useradmin");
                }
                else if (this.Room.GaveRoomRights(gameClient))
                {
                    roomUser.AddStatus("flatctrl", "");
                }

                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.SetRoomUser, new ValueHolder().AddValue("RoomUser", new List<RoomUnit>() { roomUser })));

                roomUser.Session.GetHabbo().GetRoomSession().EnteredRoom(this.Room.ID, roomUser);
                roomUser.Session.GetHabbo().GetMessenger().UpdateAllFriends(false);
                this.Room.UpdateUsersCount();

                gameClient.GetHabbo().GetMessenger().UpdateAllFriends(false);

                if (this.Room.RoomData.OwnerID == gameClient.GetHabbo().ID) //room owner enters
                {
                    if (gameClient.GetHabbo().NewbieStatus != 2)
                    {
                        if (gameClient.GetHabbo().NewbieRoom == this.Room.RoomData.ID)
                        {
                            foreach(RoomUnit user in this.RoomUsers.Values)
                            {
                                if (user is RoomBotNewbieGuide)
                                {
                                    return;
                                }
                            }

                            this.AddNewbieGuideTimer = 10;
                       }
                    }
                }

                gameClient.GetHabbo().GetUserStats().RoomRaider++;
                gameClient.GetHabbo().GetUserAchievements().CheckAchievement("RoomRaider");

                if (this.Room.RoomData.ExtraData.SellRoomPrice != null && this.Room.RoomData.ExtraData.SellRoomPrice.Count > 0)
                {
                    string message = "Looks like this room is on sale for the price of:\n";
                    foreach(KeyValuePair<string, int> price in this.Room.RoomData.ExtraData.SellRoomPrice)
                    {
                        message += price.Key.First().ToString().ToUpper() + price.Key.Substring(1) + " => " + price.Value + "\n";
                    }
                    gameClient.SendNotif(message);
                }

                if (this.Room.RoomData.ExtraData.RoleplayEnabled)
                {
                    gameClient.SendNotif("RP is enabled on this room!");
                }
            }
        }

        public void AddAIUser(RoomBotData data, int type, int x, int y, double z, int rot)
        {
            int virtualId = this.GetNextVirtualID();
            RoomBot roomUser = null;
            if (type == 0)
            {
                roomUser = new RoomBot(data, this.Room, virtualId);
            }
            else if (type == 1)
            {
                roomUser = new RoomBotGuide(data, this.Room, virtualId);
            }
            else if (type == 2)
            {
                roomUser = new RoomBotNewbieGuide(data, this.Room, virtualId);
            }

            if (roomUser != null)
            {
                roomUser.SetLocation(x, y, z);
                roomUser.SetRotation(rot, true);

                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.SetRoomUser, new ValueHolder().AddValue("RoomUser", new List<RoomUnit>() { roomUser })));

                this.RoomUsers.Add(roomUser.VirtualID, roomUser.GetType(), roomUser);

                roomUser.OnSelfEnterRoom();
            }
        }

        public void AddPet(Pet pet, int x, int y, double z, int rot)
        {
            int virtualId = this.GetNextVirtualID();
            RoomPet roomPet = new RoomPet(pet, this.Room, virtualId);
            if (roomPet != null)
            {
                roomPet.SetLocation(x, y, z);
                roomPet.SetRotation(rot, true);

                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.SetRoomUser, new ValueHolder().AddValue("RoomUser", new List<RoomUnit>() { roomPet })));

                this.RoomUsers.Add(roomPet.VirtualID, roomPet.GetType(), roomPet);

                roomPet.OnSelfEnterRoom();
            }
        }

        public MultiRevisionServerMessage GetUserStatus(bool everyone)
        {
            if (everyone)
            {
                return new MultiRevisionServerMessage(OutgoingPacketsEnum.UpdateUserState, new ValueHolder().AddValue("Everyone", everyone).AddValue("RoomUser", this.RoomUsers.Values.ToList()));
            }
            else
            {
                IEnumerable<RoomUnit> users = this.RoomUsers.Values.Where(u => u.NeedUpdate == true);

                if (users.Count() == 0)
                {
                    return null;
                }
                else
                {
                    return new MultiRevisionServerMessage(OutgoingPacketsEnum.UpdateUserState, new ValueHolder().AddValue("Everyone", everyone).AddValue("RoomUser", this.RoomUsers.Values.ToList()));
                }
            }
        }

        public void LeaveRoom(GameClient session, bool sendLeavePacket)
        {
            this.LeaveRoom(session.GetHabbo().GetRoomSession().GetRoomUser());

            session.GetHabbo().GetRoomSession().LeavedRoom();

            if (sendLeavePacket)
            {
                session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.LeaveRoom).Handle());
            }
        }

        public void LeaveRoom(RoomUnit roomUnit)
        {
            roomUnit.StopMoving();
            roomUnit.CurrentTile.UsersOnTile.Remove(roomUnit.VirtualID);

            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.UserLeaved, new ValueHolder("VirtualID", roomUnit.VirtualID)));

            this.RoomUsers.Remove(roomUnit.VirtualID, roomUnit.GetType());
            this.MovingUsers.TryRemove(roomUnit.VirtualID, out RoomUnit trash);
            this.Room.UpdateUsersCount();

            if (roomUnit.IsRealUser && roomUnit is RoomUnitUser realUser)
            {
                foreach (BotAI bot in this.GetBots())
                {
                    bot.OnUserLeaveRoom(realUser);
                }

                FreezePlayer player = this.Room.RoomFreezeManager.TryGetFreezePlayer(realUser.UserID);
                if (player != null)
                {
                    this.Room.RoomFreezeManager.LeaveGame(player, false);
                }
                this.Room.RoomGameManager.LeaveTeam(realUser);
                this.Room.RoomGameManager.LeaveTag(realUser);

                this.Room.GetTradeByUserId(realUser.UserID)?.Cancel(realUser.Session);

                if (this.Room.RoomData.IsPublicRoom)
                {
                    this.Room.RoomGameManager.RoomWobbleSquabbleManager.Leave(realUser);

                    foreach (RoomGameboard board in this.Room.RoomGameManager.Gameboards.Values)
                    {
                        board.Leave(realUser);
                    }
                }
            }

            if (roomUnit is BotAI bot_)
            {
                bot_.OnSelfLeaveRoom(false);
            }
        }

        public void KickUser(GameClient gameClient, bool showKickMessage)
        {
            this.LeaveRoom(gameClient, true);

            if (showKickMessage)
            {
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                Message.Init(r63aOutgoing.KickMessage);
                Message.AppendInt32(4008);
                gameClient.SendMessage(Message);
            }
        }

        public void BanUser(GameClient gameClient)
        {
            this.LeaveRoom(gameClient, true);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.KickMessage);
            Message.AppendInt32(4008);
            gameClient.SendMessage(Message);

            this.Bans.Add(gameClient.GetHabbo().ID, TimeUtilies.GetUnixTimestamp());
        }

        public bool UserHaveBan(uint id)
        {
            if (this.Bans.ContainsKey(id))
            {
                if (TimeUtilies.GetUnixTimestamp() - this.Bans[id] >= 900)
                {
                    this.Bans.Remove(id);

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void SaveAll()
        {
            StringBuilder query = new StringBuilder();
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                foreach (RoomPet pet in this.RoomUsers.Get(typeof(RoomPet)))
                {
                    if (pet.PetData.NeedUpdate)
                    {
                        dbClient.AddParamWithValue("petId" + pet.PetData.ID, pet.PetData.ID);
                        dbClient.AddParamWithValue("roomId" + pet.PetData.ID, this.Room.ID);
                        dbClient.AddParamWithValue("expirience" + pet.PetData.ID, pet.PetData.Expirience);
                        dbClient.AddParamWithValue("energy" + pet.PetData.ID, pet.PetData.Energy);
                        dbClient.AddParamWithValue("happiness" + pet.PetData.ID, pet.PetData.Happiness);
                        dbClient.AddParamWithValue("respect" + pet.PetData.ID, pet.PetData.Respect);
                        dbClient.AddParamWithValue("x" + pet.PetData.ID, pet.X);
                        dbClient.AddParamWithValue("y" + pet.PetData.ID, pet.Y);
                        dbClient.AddParamWithValue("z" + pet.PetData.ID, pet.Z);

                        query.Append("UPDATE user_pets SET room_id = @roomId" + pet.PetData.ID + ", expirience = @expirience" + pet.PetData.ID + ", energy = @energy" + pet.PetData.ID + ", happiness = @happiness" + pet.PetData.ID + ", respect = @respect" + pet.PetData.ID + ", x = @x" + pet.PetData.ID + ", y = @y" + pet.PetData.ID + ", z = @z" + pet.PetData.ID + " WHERE id = @petId" + pet.PetData.ID + " LIMIT 1; ");
                    }
                }

                if (query.Length > 0)
                {
                    dbClient.ExecuteQuery(query.ToString());
                }
            }
        }

        public List<RoomUnit> GetRealUsers()
        {
            return this.RoomUsers.Get(typeof(RoomUnitUser));
        }

        public IEnumerable<BotAI> GetBots()
        {
            return this.RoomUsers.Values.Where(u => u.IsRealUser == false && u is BotAI).Select(u => u as BotAI);
        }

        public ICollection<RoomUnit> GetRoomUsers()
        {
            return this.RoomUsers.Values;
        }

        public RoomUnitUser GetUserByID(uint id)
        {
            return (RoomUnitUser)this.RoomUsers.Get(typeof(RoomUnitUser)).FirstOrDefault(u => ((RoomUnitUser)u).UserID == id);
        }

        public RoomUnitUser GetUserByVirtualID(int virtualId)
        {
            return (RoomUnitUser)this.RoomUsers.Get(typeof(RoomUnitUser)).FirstOrDefault(u => u.VirtualID == virtualId);
        }

        public RoomPet GetPetByID(uint id)
        {
            return (RoomPet)this.RoomUsers.Get(typeof(RoomPet)).FirstOrDefault(p => ((RoomPet)p).PetData.ID == id);
        }

        public RoomUnitUser GetUserByName(string username)
        {
            return (RoomUnitUser)this.RoomUsers.Get(typeof(RoomUnitUser)).FirstOrDefault(u => ((RoomUnitUser)u).Session?.GetHabbo()?.Username == username);
        }

        public RoomPet GetPetByName(string name)
        {
            return (RoomPet)this.RoomUsers.Get(typeof(RoomPet)).FirstOrDefault(p => ((RoomPet)p).PetData.Name == name);
        }
    }
}
