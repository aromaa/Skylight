using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomFootballManager
    {
        public Room Room;

        public RoomFootballManager(Room room)
        {
            this.Room = room;
        }

        public void OnCycle()
        {
            foreach (RoomItemBall ball in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBall)))
            {
                this.Room.ThrowIfRoomCycleCancalled("Cycle room footballs", ball); //Have room cycle cancalled?

                switch (ball.ExtraData)
                {
                    case "16":
                        this.DoFootballMath(ball, ball.FootballDirection, 15);
                        break;
                    case "15":
                        this.DoFootballMath(ball, ball.FootballDirection, 14);
                        break;
                    case "14":
                        this.DoFootballMath(ball, ball.FootballDirection, 13);
                        ball.FootballWaitTime = 1;
                        break;
                    case "13":
                        this.DoFootballMath(ball, ball.FootballDirection, 12);
                        ball.FootballWaitTime = 2;
                        break;
                    case "12":
                        this.DoFootballMath(ball, ball.FootballDirection, 11);
                        break;
                    case "11":
                        ball.ExtraData = "10";
                        ball.UpdateState(false, true);
                        break;
                    case "10":
                        break;
                }
            }
        }

        public void UserKickFootball(RoomUnit user, RoomItemBall football)
        {
            football.LastUserHitFootball = (user as RoomUnitUser)?.Session?.GetHabbo() != null ? (RoomUnitUser)user: null;

            if (user.TargetX == football.X && user.TargetY == football.Y)
            {
                this.DoFootballMath(football, user.BodyRotation, 16);
            }
            else
            {
                this.DoFootballMath(football, user.BodyRotation, 11);
            }
        }

        //return: was not blocked
        public bool DoFootballHitTest(RoomItemBall football, int x, int y, int direction, int blockedDirection)
        {
            RoomTile tile = this.Room.RoomGamemapManager.GetTile(x, y);
            RoomItem higestGoal = tile?.ItemsOnTile.Get(typeof(RoomItemFootballGoal)).OrderBy(i => i.Z).FirstOrDefault();

            bool goalBlocks = false;
            if (higestGoal != null)
            {
                if (higestGoal.Rot == 0 && direction != 4)
                {
                    goalBlocks = true;
                }
                else if (higestGoal.Rot == 2 && direction != 6)
                {
                    goalBlocks = true;
                }
                else if (higestGoal.Rot == 4 && direction != 0)
                {
                    goalBlocks = true;
                }
                else if (higestGoal.Rot == 6 && direction != 2)
                {
                    goalBlocks = true;
                }
            }

            if (!goalBlocks && this.Room.RoomItemManager.MoveFloorItemOnRoom(null, football, x, y, football.Rot, -1.0, false))
            {
                football.FootballDirection = direction;
                return true;
            }
            else
            {
                if (tile != null && tile.IsInUse)
                {
                    football.ExtraData = "10";
                    football.UpdateState(false, true);
                    return true;
                }
                else
                {
                    football.FootballDirection = blockedDirection;
                    football.FootballWaitTime = 3;
                    return false;
                }
            }
        }

        public void DoFootballMath(RoomItemBall football, int direction, int power)
        {
            int originalX = football.X;
            int originalY = football.Y;
            int footballX = football.X;
            int footballY = football.Y;
            double originalZ = football.Z;
            football.ExtraData = power.ToString();
            football.UpdateState(false, true);

            if (direction == 4)
            {
                footballY++;
                this.DoFootballHitTest(football, footballX, footballY, 4, 0);
            }
            else
            {
                if (direction == 0)
                {
                    footballY--;
                    this.DoFootballHitTest(football, footballX, footballY, 0, 4);
                }
                else
                {
                    if (direction == 6)
                    {
                        footballX--;
                        this.DoFootballHitTest(football, footballX, footballY, 6, 2);
                    }
                    else
                    {
                        if (direction == 2)
                        {
                            footballX++;
                            this.DoFootballHitTest(football, footballX, footballY, 2, 6);
                        }
                        else
                        {
                            if (direction == 3)
                            {
                                footballX++;
                                footballY++;
                                if (!this.DoFootballHitTest(football, footballX, footballY, 3, 3))
                                {
                                    bool rightAvaible = this.FootballCanMoveTo(football, football.X + 1, football.Y);
                                    bool leftAvaible = this.FootballCanMoveTo(football, football.X, football.Y + 1);
                                    if (!rightAvaible && !leftAvaible)
                                    {
                                        football.FootballDirection = 7;
                                    }
                                    else if (!rightAvaible && leftAvaible)
                                    {
                                        football.FootballDirection = 5;
                                    }
                                    else
                                    {
                                        football.FootballDirection = 1;
                                    }

                                    football.FootballWaitTime = 3;
                                }
                            }
                            else
                            {
                                if (direction == 1)
                                {
                                    footballX++;
                                    footballY--;
                                    if (!this.DoFootballHitTest(football, footballX, footballY, 1, 3))
                                    {
                                        bool leftAvaible = this.FootballCanMoveTo(football, football.X + 1, football.Y);
                                        bool rightAvaible = this.FootballCanMoveTo(football, football.X, football.Y - 1);
                                        if (!leftAvaible && !rightAvaible)
                                        {
                                            football.FootballDirection = 5;
                                        }
                                        else if (leftAvaible)
                                        {
                                            football.FootballDirection = 3;
                                        }
                                        else
                                        {
                                            football.FootballDirection = 7;
                                        }
                                        football.FootballWaitTime = 3;
                                    }
                                }
                                else
                                {
                                    if (direction == 7)
                                    {
                                        footballX--;
                                        footballY--;
                                        if (!this.DoFootballHitTest(football, footballX, footballY, 7, 3))
                                        {
                                            bool leftAvaible = this.FootballCanMoveTo(football, football.X - 1, football.Y);
                                            bool rightAvaible = this.FootballCanMoveTo(football, football.X - 1, football.Y);
                                            if (leftAvaible && !rightAvaible)
                                            {
                                                football.FootballDirection = 3;
                                            }
                                            else if (leftAvaible && rightAvaible)
                                            {
                                                football.FootballDirection = 5;
                                            }
                                            else
                                            {
                                                football.FootballDirection = 1;
                                            }
                                            football.FootballWaitTime = 3;
                                        }
                                    }
                                    else
                                    {
                                        if (direction == 5)
                                        {
                                            footballX--;
                                            footballY++;
                                            if (!this.DoFootballHitTest(football, footballX, footballY, 5, 3))
                                            {
                                                bool leftAvaible = this.FootballCanMoveTo(football, football.X - 1, football.Y);
                                                bool rightAvaible = this.FootballCanMoveTo(football, football.X, football.Y + 1);
                                                if (!leftAvaible && rightAvaible)
                                                {
                                                    football.FootballDirection = 3;
                                                }
                                                else
                                                {
                                                    football.FootballDirection = 7;
                                                }

                                                football.FootballWaitTime = 3;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (originalX == football.X && originalY == football.Y) //no movement
            {
                return;
            }
            
            this.Room.RoomItemManager.MoveAnimation[football.ID] = new RoomItemRollerMovement(football.ID, originalX, originalY, originalZ, 0, football.X, football.Y, football.Z);

            GameTeam goal = GameTeam.None;
            foreach (RoomItemFootballGoal goal_ in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemFootballGoal)))
            {
                GameTeam currentGoal = GameTeam.None;
                if (goal_.GetBaseItem().InteractionType.ToLower() == "blue_goal")
                {
                    currentGoal = GameTeam.Blue;
                }
                else if (goal_.GetBaseItem().InteractionType.ToLower() == "green_goal")
                {
                    currentGoal = GameTeam.Green;
                }
                else if (goal_.GetBaseItem().InteractionType.ToLower() == "yellow_goal")
                {
                    currentGoal = GameTeam.Yellow;
                }
                else if (goal_.GetBaseItem().InteractionType.ToLower() == "red_goal")
                {
                    currentGoal = GameTeam.Red;
                }

                if (currentGoal != GameTeam.None)
                {
                    if (football.X == goal_.X && football.Y == goal_.Y)
                    {
                        goal = currentGoal;
                        break;
                    }
                    else
                    {
                        bool found = false;
                        foreach (AffectedTile tile in goal_.AffectedTiles)
                        {
                            if (tile.X == football.X && tile.Y == football.Y)
                            {
                                goal = currentGoal;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }
                }
            }

            if (goal != GameTeam.None) //ITS A FUCKING SCOREEEEEEEE
            {
                football.ExtraData = "10";
                football.UpdateState(false, true);

                if (football.LastUserHitFootball != null)
                {
                    this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Wave, new ValueHolder("VirtualID", football.LastUserHitFootball.VirtualID)));

                    football.LastUserHitFootball.Session.GetHabbo().GetUserStats().FootballGoalScorer++;
                    football.LastUserHitFootball.Session.GetHabbo().GetUserAchievements().CheckAchievement("FootballGoalScorer");

                    this.Room.FootballGoalHost(1);
                }

                this.GoalScore(goal);
            }
        }

        public void GoalScore(GameTeam team)
        {
            if (team != GameTeam.None)
            {
                switch(team)
                {
                    case GameTeam.Blue:
                        {
                            foreach (RoomItemFootballBlueScore scoreboard in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBall)))
                            {
                                scoreboard.EditScore(1);
                            }
                        }
                        break;
                    case GameTeam.Green:
                        {
                            foreach (RoomItemFootballGreenScore scoreboard in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBall)))
                            {
                                scoreboard.EditScore(1);
                            }
                        }
                        break;
                    case GameTeam.Yellow:
                        {
                            foreach (RoomItemFootballYellowScore scoreboard in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBall)))
                            {
                                scoreboard.EditScore(1);
                            }
                        }
                        break;
                    case GameTeam.Red:
                        {
                            foreach (RoomItemFootballRedScore scoreboard in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBall)))
                            {
                                scoreboard.EditScore(1);
                            }
                        }
                        break;
                }
            }
        }

        public bool FootballCanMoveTo(RoomItemBall football,int x, int y)
        {
            return this.Room.RoomItemManager.CanPlaceItemAt(football, x, y);
        }
    }
}
