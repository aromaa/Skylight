using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomFreezeManager
    {
        public Room Room;
        public bool GameStarted = false;
        public RoomItem FreezeExitTile;
        public Dictionary<uint, RoomItem> FreezeGates;
        public Dictionary<uint, RoomItem> FreezeScoreboards;
        public Dictionary<uint, FreezePlayer> Players;
        public List<FreezeBall> Balls;
        private List<FreezeBallHit> BallHits;

        //for some stuff
        public long NextFreezeBallID;

        public RoomFreezeManager(Room room)
        {
            this.Room = room;
            this.FreezeGates = new Dictionary<uint, RoomItem>();
            this.FreezeScoreboards = new Dictionary<uint, RoomItem>();
            this.Players = new Dictionary<uint, FreezePlayer>();
            this.Balls = new List<FreezeBall>();
            this.BallHits = new List<FreezeBallHit>();
        }

        public void AddItem(RoomItem item)
        {
            if (item is RoomItemFreezeExitTile)
            {
                this.FreezeExitTile = item;
            }
            else if (item is RoomItemFreezeGateBlue || item is RoomItemFreezeGateGreen || item is RoomItemFreezeGateRed || item is RoomItemFreezeGateYellow)
            {
                this.FreezeGates.Add(item.ID, item);
            }
            else if (item is RoomItemFreezeScoreBlue || item is RoomItemFreezeScoreGreen || item is RoomItemFreezeScoreRed || item is RoomItemFreezeScoreYellow)
            {
                this.FreezeScoreboards.Add(item.ID, item);
            }
        }

        public void RemoveItem(RoomItem item)
        {
            if (item is RoomItemFreezeExitTile)
            {
                this.FreezeExitTile = null;
            }
            else if (item is RoomItemFreezeGateBlue || item is RoomItemFreezeGateGreen || item is RoomItemFreezeGateRed || item is RoomItemFreezeGateYellow)
            {
                this.FreezeGates.Remove(item.ID);
            }
            else if (item is RoomItemFreezeScoreBlue || item is RoomItemFreezeScoreGreen || item is RoomItemFreezeScoreRed || item is RoomItemFreezeScoreYellow)
            {
                this.FreezeScoreboards.Remove(item.ID);
            }
        }

        public void StopGame()
        {
            if (this.GameStarted)
            {
                this.GameStarted = false;

                this.Balls.Clear();
                this.BallHits.Clear();

                if (this.FreezeExitTile != null)
                {
                    this.FreezeExitTile.ExtraData = "0";
                    this.FreezeExitTile.UpdateState(false, true);
                }

                foreach (RoomItem item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemFreezeTile)))
                {
                    item.ExtraData = "0";
                    item.UpdateState(false, true);
                }

                Dictionary<int, int> aliveTeams = new Dictionary<int, int>();
                foreach(FreezePlayer player in this.Players.Values)
                {
                    if (aliveTeams.ContainsKey((int)player.Team))
                    {
                        aliveTeams[(int)player.Team] += player.Lives;
                    }
                    else
                    {
                        aliveTeams.Add((int)player.Team, player.Lives);
                    }
                }

                if (aliveTeams.Count > 0)
                {
                    aliveTeams = aliveTeams.OrderByDescending(d => d.Value).ToDictionary(d => d.Key, d => d.Value);

                    KeyValuePair<int, int> bestScore = aliveTeams.ElementAt(0);
                    List<int> winingTeams = new List<int>();
                    foreach(KeyValuePair<int, int> data in aliveTeams.OrderByDescending(d => d.Value))
                    {
                        if (bestScore.Value == data.Value) //wining team or tie :o
                        {
                            winingTeams.Add((int)data.Key);
                        }
                    }

                    foreach (FreezePlayer player in this.Players.Values)
                    {
                        this.Players.Remove(player.User.Session.GetHabbo().ID);

                        player.User.RestrictMovementType &= ~RestrictMovementType.Server;
                        player.Reset();
                        player.GiveHelmet();

                        if (winingTeams.Contains((int)player.Team))
                        {
                            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Wave, new ValueHolder("VirtualID", player.User.VirtualID)));
                        }
                    }
                }

                foreach (RoomItem gate in this.FreezeGates.Values)
                {
                    this.Room.RoomGamemapManager.GetTile(gate.X, gate.Y).UpdateTile();
                }
            }
        }

        public void StartGame()
        {
            if (!this.GameStarted)
            {
                this.GameStarted = true;

                foreach (RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
                {
                    if (user.GameType == GameType.Freeze)
                    {
                        FreezePlayer player = new FreezePlayer(this.Room, user, user.GameTeam);
                        this.Players.Add(user.Session.GetHabbo().ID, player);

                        player.Reset();
                        player.GiveHelmet();
                    }
                }

                if (this.FreezeExitTile != null)
                {
                    this.FreezeExitTile.ExtraData = "1";
                    this.FreezeExitTile.UpdateState(false, true);
                }

                List<Point> checkedTiles = new List<Point>();
                foreach (RoomItem item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemFreezeTile)))
                {
                    item.ExtraData = "0";
                    item.UpdateState(false, true);

                    Point point = new Point(item.X, item.Y);
                    if (!checkedTiles.Contains(point))
                    {
                        RoomTile tile = this.Room.RoomGamemapManager.GetTile(point.X, point.Y);
                        if (tile != null)
                        {
                            foreach (RoomUnitUser user in tile.UsersOnTile.Values)
                            {
                                if (!this.Players.ContainsKey(user.Session.GetHabbo().ID))
                                {
                                    if (this.FreezeExitTile != null)
                                    {
                                        user.StopMoving();
                                        user.SetLocation(this.FreezeExitTile.X, this.FreezeExitTile.Y, this.FreezeExitTile.Z); //set new location
                                        user.UpdateState();
                                    }
                                }
                            }
                        }
                    }
                }

                foreach(RoomItem item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemFreezeIceBlock)))
                {
                    item.ExtraData = "0";
                    item.UpdateState(true, true);
                }

                this.UpdateScoreboards();

                foreach(RoomItem gate in this.FreezeGates.Values)
                {
                    this.Room.RoomGamemapManager.GetTile(gate.X, gate.Y).UpdateTile();
                }
            }
        }

        public void OnCycle()
        {
            foreach(FreezeBall ball in this.Balls)
            {
                this.Room.ThrowIfRoomCycleCancalled("Cycle room freeze balls", ball); //Have room cycle cancalled?

                if (ball.HitGround)
                {
                    this.Balls.Remove(ball);

                    ball.Player.Balls++;

                    ball.Source.ExtraData = "11200";
                    ball.Source.UpdateState(false, true);

                    int freezedPlayers = this.FreezeUsersOnTile(ball.Source.X, ball.Source.Y);
                    if (freezedPlayers > 0)
                    {
                        ball.Player.User.Session.GetHabbo().GetUserStats().FreezeFigter += freezedPlayers;
                        ball.Player.User.Session.GetHabbo().GetUserAchievements().CheckAchievement("FreezeFigter");
                    }

                    long ballId = Interlocked.Increment(ref this.NextFreezeBallID);
                    if (ball.BallType == FreezeBallType.Normal || ball.BallType == FreezeBallType.Mega)
                    {
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 0, ball.Range));
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 1, ball.Range));
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 2, ball.Range));
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 3, ball.Range));
                    }

                    if (ball.BallType == FreezeBallType.Diagonal || ball.BallType == FreezeBallType.Mega)
                    {
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 4, ball.Range));
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 5, ball.Range));
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 6, ball.Range));
                        this.BallHits.AddRange(this.CalcListOfPoints(ballId, new Point(ball.Source.X, ball.Source.Y), ball.Player, 7, ball.Range));
                    }
                }
            }

            foreach(FreezeBallHit hit in this.BallHits)
            {
                this.Room.ThrowIfRoomCycleCancalled("Cycle room freeze ball hits", hit); //Have room cycle cancalled?

                hit.Ticks--;
                if (hit.Ticks <= 0)
                {
                    this.BallHits.Remove(hit);

                    RoomItem tile = this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemFreezeTile)).FirstOrDefault(t => t.X == hit.Point.X && t.Y == hit.Point.Y);
                    if (tile != null)
                    {
                        if (this.BreakIceBlock(this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemFreezeIceBlock)).FirstOrDefault(b => b.X == hit.Point.X && b.Y == hit.Point.Y)))
                        {
                            this.BallHits.RemoveAll(b => b.ID == hit.ID && b.Direction == hit.Direction);
                        }

                        tile.ExtraData = "11200";
                        tile.UpdateState(false, true);

                        int freezedPlayers = this.FreezeUsersOnTile(tile.X, tile.Y);
                        if (freezedPlayers > 0)
                        {
                            hit.Player.User.Session.GetHabbo().GetUserStats().FreezeFigter += freezedPlayers;
                            hit.Player.User.Session.GetHabbo().GetUserAchievements().CheckAchievement("FreezeFigter");
                        }
                    }
                    else
                    {
                        this.BallHits.RemoveAll(b => b.ID == hit.ID && b.Direction == hit.Direction);
                    }
                }
            }

            foreach(FreezePlayer player in this.Players.Values)
            {
                this.Room.ThrowIfRoomCycleCancalled("Cycle freeze players", player); //Have room cycle cancalled?

                player.Cycle();
            }
        }

        //return amount of freezed players
        public int FreezeUsersOnTile(int x, int y)
        {
            int amount = 0;

            foreach (FreezePlayer player in this.Players.Values)
            {
                if (!player.Freezed && !player.Shield)
                {
                    if (!player.User.Moving && player.User.X == x && player.User.Y == y)
                    {
                        player.Freeze();
                        amount++;
                    }
                    else if (player.User.Moving && player.User.NextStepX == x && player.User.NextStepY == y)
                    {
                        player.Freeze();
                        amount++;
                    }
                }
            }

            return amount;
        }

        public bool BreakIceBlock(RoomItem iceBlock)
        {
            if (iceBlock != null)
            {
                if (string.IsNullOrEmpty(iceBlock.ExtraData) || iceBlock.ExtraData == "0")
                {
                    switch(RandomUtilies.GetRandom(1, 9))
                    {
                        case 1:
                            iceBlock.ExtraData = "2000";
                            break;
                        case 2:
                            iceBlock.ExtraData = "3000";
                            break;
                        case 3:
                            iceBlock.ExtraData = "4000";
                            break;
                        case 4:
                            iceBlock.ExtraData = "5000";
                            break;
                        case 5:
                            iceBlock.ExtraData = "6000";
                            break;
                        case 6:
                            iceBlock.ExtraData = "7000";
                            break;
                        default:
                            iceBlock.ExtraData = "1000";
                            break;
                    }

                    iceBlock.UpdateState(false, true);
                    this.Room.RoomGamemapManager.GetTile(iceBlock.X, iceBlock.Y).UpdateTile();
                    return true;
                }
            }
            return false;
        }

        public List<FreezeBallHit> CalcListOfPoints(long ballId, Point point, FreezePlayer player, int direction, int range)
        {
            List<FreezeBallHit> points = new List<FreezeBallHit>();
            if (direction == 0)
            {
                for(int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X + i, point.Y), player, direction, i + 1));
                }
            }
            else if (direction == 1)
            {
                for (int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X - i, point.Y), player, direction, i + 1));
                }
            }
            else if (direction == 2)
            {
                for (int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X, point.Y + i), player, direction, i + 1));
                }
            }
            else if (direction == 3)
            {
                for (int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X, point.Y - i), player, direction, i + 1));
                }
            }
            else if (direction == 4)
            {
                for (int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X - i, point.Y - i), player, direction, i + 1));
                }
            }
            else if (direction == 5)
            {
                for (int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X + i, point.Y + i), player, direction, i + 1));
                }
            }
            else if (direction == 6)
            {
                for (int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X - i, point.Y + i), player, direction, i + 1));
                }
            }
            else if (direction == 7)
            {
                for (int i = 1; i <= range; i++)
                {
                    points.Add(new FreezeBallHit(ballId, new Point(point.X + 1, point.Y - i), player, direction, i + 1));
                }
            }
            return points;
        }

        public void UpdateScoreboards()
        {
            int blue = 0;
            int green = 0;
            int red = 0;
            int yellow = 0;

            foreach (FreezePlayer player in this.Players.Values)
            {
                if (player.Team == GameTeam.Blue)
                {
                    blue += player.Lives * 10;
                }
                else if(player.Team == GameTeam.Green)
                {
                    green += player.Lives * 10;
                }
                else if (player.Team == GameTeam.Red)
                {
                    red += player.Lives * 10;
                }
                else if (player.Team == GameTeam.Yellow)
                {
                    yellow += player.Lives * 10;
                }
            }

            foreach(RoomItem item in this.FreezeScoreboards.Values)
            {
                if (item is RoomItemFreezeScoreBlue)
                {
                    item.ExtraData = blue.ToString();
                    item.UpdateState(true, true);
                }
                else if (item is RoomItemFreezeScoreGreen)
                {
                    item.ExtraData = green.ToString();
                    item.UpdateState(true, true);
                }
                else if (item is RoomItemFreezeScoreRed)
                {
                    item.ExtraData = red.ToString();
                    item.UpdateState(true, true);
                }
                else if (item is RoomItemFreezeScoreYellow)
                {
                    item.ExtraData = yellow.ToString();
                    item.UpdateState(true, true);
                }
            }
        }

        public FreezePlayer TryGetFreezePlayer(uint id)
        {
            this.Players.TryGetValue(id, out FreezePlayer freezePlayer);
            return freezePlayer;
        }

        public void LeaveGame(FreezePlayer player, bool died)
        {
            if (died)
            {
                if (this.FreezeExitTile != null)
                {
                    player.User.StopMoving();
                    player.User.SetLocation(this.FreezeExitTile.X, this.FreezeExitTile.Y, this.FreezeExitTile.Z); //set new location
                    player.User.UpdateState();
                }
            }

            this.Players.Remove(player.User.Session.GetHabbo().ID);
            player.User.ApplyEffect(-1);

            HashSet<int> activeTeams = new HashSet<int>();
            foreach(FreezePlayer player_ in this.Players.Values)
            {
                activeTeams.Add((int)player_.Team);
            }

            if (activeTeams.Count <= 1)
            {
                this.Room.RoomGameManager.StopGame();
            }
        }
    }
}
