using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms.Algorithm;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomBattleBanzaiManager
    {
        public Room Room;
        public bool GameStarted = false;
        public RoomBattleBallGameField RoomBattleBallGameField;
        public int GreenScore;
        public int BlueScore;
        public int YellowScore;
        public int RedScore;
        public int WiningBlink = -1;
        public Dictionary<int, List<RoomItemBattleBanzaiPatch>> WiningTeamTiles;

        public RoomBattleBanzaiManager(Room room)
        {
            this.Room = room;
            this.WiningTeamTiles = new Dictionary<int, List<RoomItemBattleBanzaiPatch>>();
        }

        public void OnCycle()
        {
            foreach (RoomItemBattleBanzaiPuck puck in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPuck)))
            {
                this.Room.ThrowIfRoomCycleCancalled("Cycle battle banzai pucks", puck); //Have room cycle cancalled?

                switch(puck.Power)
                {
                    case 6:
                    case 5:
                        {
                            puck.Power--;
                            this.DoPuckMath(puck, puck.PuckDirection, puck.Power);
                        }
                        break;
                    case 4:
                        {
                            puck.Power--;
                            this.DoPuckMath(puck, puck.PuckDirection, puck.Power);
                            puck.PuckWaitTime = 1;
                        }
                        break;
                    case 3:
                        {
                            puck.Power--;
                            this.DoPuckMath(puck, puck.PuckDirection, puck.Power);
                            puck.PuckWaitTime = 2;
                        }
                        break;
                    case 2:
                        {
                            puck.Power--;
                            this.DoPuckMath(puck, puck.PuckDirection, puck.Power);
                        }
                        break;
                    case 1:
                        {
                            puck.ExtraData = "0";
                            puck.UpdateState(true, true);
                            puck.Power--;
                        }
                        break;
                    case 0:
                        {
                            break;
                        }
                }
            }

            if (this.WiningBlink != -1)
            {
                this.HandleWiningBlink();
            }
        }

        public void HandleWiningBlink()
        {
            this.WiningBlink++;
            if (this.WiningBlink % 10 == 0)
            {
                foreach(KeyValuePair<int, List<RoomItemBattleBanzaiPatch>> data in this.WiningTeamTiles)
                {
                    foreach (RoomItemBattleBanzaiPatch tile in data.Value)
                    {
                        this.Room.ThrowIfRoomCycleCancalled("Cycle battle banzai blink", tile); //Have room cycle cancalled?

                        tile.ExtraData = (data.Key * 3 + 2).ToString();
                        tile.UpdateState(false, true);
                    }
                }
            }
            else if (this.WiningBlink % 5 == 0)
            {
                foreach (KeyValuePair<int, List<RoomItemBattleBanzaiPatch>> data in this.WiningTeamTiles)
                {
                    foreach (RoomItemBattleBanzaiPatch tile in data.Value)
                    {
                        this.Room.ThrowIfRoomCycleCancalled("Cycle battle banzai blink", tile); //Have room cycle cancalled?

                        tile.ExtraData = "1";
                        tile.UpdateState(false, true);
                    }
                }
            }

            if (this.WiningBlink > 40)
            {
                this.WiningBlink = -1;
            }
        }

        public void UserKickPuck(RoomUnit user, RoomItemBattleBanzaiPuck puck)
        {
            if (user is RoomUnitUser user_)
            {
                puck.LastUserHitPuck = user;

                puck.ExtraData = ((int)user_.GameTeam).ToString();
                puck.UpdateState(false, true);

                if (user_.GameType == GameType.BattleBanzai) //we have change user is on game
                {
                    (this.Room.RoomGamemapManager.GetTile(puck.X, puck.Y).HigestRoomItem as RoomItemBattleBanzaiPatch)?.TileWalkOnLogic(puck.LastUserHitPuck);
                }
            }
            else
            {
                puck.LastUserHitPuck = null;
            }

            if (user.TargetX == puck.X && user.TargetY == puck.Y)
            {
                this.DoPuckMath(puck, user.BodyRotation, 6);
            }
            else
            {
                this.DoPuckMath(puck, user.BodyRotation, 1);
            }
        }

        //return: was not blocked
        public bool DoPuckHitTest(RoomItemBattleBanzaiPuck puck, int x, int y, int direction, int blockedDirection)
        {
            if (this.Room.RoomItemManager.MoveFloorItemOnRoom(null, puck, x, y, puck.Rot, -1.0, false))
            {
                puck.PuckDirection = direction;
                return true;
            }
            else
            {
                RoomTile tile = this.Room.RoomGamemapManager.GetTile(x, y);
                if (tile != null && tile.IsInUse)
                {
                    puck.Power = 0;
                    return true;
                }
                else
                {
                    puck.PuckDirection = blockedDirection;
                    puck.PuckWaitTime = 3;
                    return false;
                }
            }
        }

        public void DoPuckMath(RoomItemBattleBanzaiPuck puck, int direction, int power)
        {
            int originalX = puck.X;
            int originalY = puck.Y;
            double originalZ = puck.Z;

            int footballX = puck.X;
            int footballY = puck.Y;

            puck.Power = power;

            if (direction == 4)
            {
                footballY++;
                this.DoPuckHitTest(puck, footballX, footballY, 4, 0);
            }
            else
            {
                if (direction == 0)
                {
                    footballY--;
                    this.DoPuckHitTest(puck, footballX, footballY, 0, 4);
                }
                else
                {
                    if (direction == 6)
                    {
                        footballX--;
                        this.DoPuckHitTest(puck, footballX, footballY, 6, 2);
                    }
                    else
                    {
                        if (direction == 2)
                        {
                            footballX++;
                            this.DoPuckHitTest(puck, footballX, footballY, 2, 6);
                        }
                        else
                        {
                            if (direction == 3)
                            {
                                footballX++;
                                footballY++;
                                if (!this.DoPuckHitTest(puck, footballX, footballY, 3, 3))
                                {
                                    bool rightAvaible = this.FootballCanMoveTo(puck, puck.X + 1, puck.Y);
                                    bool leftAvaible = this.FootballCanMoveTo(puck, puck.X, puck.Y + 1);
                                    if (!rightAvaible && !leftAvaible)
                                    {
                                        puck.PuckDirection = 7;
                                    }
                                    else if (!rightAvaible && leftAvaible)
                                    {
                                        puck.PuckDirection = 5;
                                    }
                                    else
                                    {
                                        puck.PuckDirection = 1;
                                    }

                                    puck.PuckWaitTime = 3;
                                }
                            }
                            else
                            {
                                if (direction == 1)
                                {
                                    footballX++;
                                    footballY--;
                                    if (!this.DoPuckHitTest(puck, footballX, footballY, 1, 3))
                                    {
                                        bool leftAvaible = this.FootballCanMoveTo(puck, puck.X + 1, puck.Y);
                                        bool rightAvaible = this.FootballCanMoveTo(puck, puck.X, puck.Y - 1);
                                        if (!leftAvaible && !rightAvaible)
                                        {
                                            puck.PuckDirection = 5;
                                        }
                                        else if (leftAvaible)
                                        {
                                            puck.PuckDirection = 3;
                                        }
                                        else
                                        {
                                            puck.PuckDirection = 7;
                                        }
                                        puck.PuckWaitTime = 3;
                                    }
                                }
                                else
                                {
                                    if (direction == 7)
                                    {
                                        footballX--;
                                        footballY--;
                                        if (!this.DoPuckHitTest(puck, footballX, footballY, 7, 3))
                                        {
                                            bool leftAvaible = this.FootballCanMoveTo(puck, puck.X - 1, puck.Y);
                                            bool rightAvaible = this.FootballCanMoveTo(puck, puck.X - 1, puck.Y);
                                            if (leftAvaible && !rightAvaible)
                                            {
                                                puck.PuckDirection = 3;
                                            }
                                            else if (leftAvaible && rightAvaible)
                                            {
                                                puck.PuckDirection = 5;
                                            }
                                            else
                                            {
                                                puck.PuckDirection = 1;
                                            }
                                            puck.PuckWaitTime = 3;
                                        }
                                    }
                                    else
                                    {
                                        if (direction == 5)
                                        {
                                            footballX--;
                                            footballY++;
                                            if (!this.DoPuckHitTest(puck, footballX, footballY, 5, 3))
                                            {
                                                bool leftAvaible = this.FootballCanMoveTo(puck, puck.X - 1, puck.Y);
                                                bool rightAvaible = this.FootballCanMoveTo(puck, puck.X, puck.Y + 1);
                                                if (!leftAvaible && rightAvaible)
                                                {
                                                    puck.PuckDirection = 3;
                                                }
                                                else
                                                {
                                                    puck.PuckDirection = 7;
                                                }

                                                puck.PuckWaitTime = 3;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (originalX == puck.X && originalY == puck.Y) //no movement
            {
                return;
            }
            
            this.Room.RoomItemManager.MoveAnimation[puck.ID] = new RoomItemRollerMovement(puck.ID, originalX, originalY, originalZ, 0, puck.X, puck.Y, puck.Z);

            (this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPatch)).FirstOrDefault(t => t.X == puck.X && t.Y == puck.Y) as RoomItemBattleBanzaiPatch)?.TileWalkOnLogic(puck.LastUserHitPuck);
        }

        public bool FootballCanMoveTo(RoomItemBattleBanzaiPuck puck, int x, int y)
        {
            return this.Room.RoomItemManager.CanPlaceItemAt(puck, x, y);
        }

        public void StartGame()
        {
            if (!this.GameStarted)
            {
                this.WiningBlink = -1;
                this.GameStarted = true;
                this.RoomBattleBallGameField = new RoomBattleBallGameField(new GameTeam[this.Room.RoomGamemapManager.Model.MaxY, this.Room.RoomGamemapManager.Model.MaxX]);

                this.GreenScore = 0;
                this.BlueScore = 0;
                this.YellowScore = 0;
                this.RedScore = 0;

                this.UpdateScore(GameTeam.Green);
                this.UpdateScore(GameTeam.Blue);
                this.UpdateScore(GameTeam.Yellow);
                this.UpdateScore(GameTeam.Red);

                foreach(RoomItem item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPatch)))
                {
                    item.ExtraData = "1";
                    item.UpdateState(false, true);
                }
            }
        }

        public void StopGame()
        {
            if (this.GameStarted)
            {
                this.GameStarted = false;
                this.RoomBattleBallGameField = null;

                foreach (RoomItem item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPatch)))
                {
                    if (item.ExtraData == "1")
                    {
                        item.ExtraData = "0";
                        item.UpdateState(true, true);
                    }
                }

                //team, team score
                Dictionary<int, int> scores = new Dictionary<int, int>();
                scores.Add((int)GameTeam.Blue, this.BlueScore);
                scores.Add((int)GameTeam.Green, this.GreenScore);
                scores.Add((int)GameTeam.Yellow, this.YellowScore);
                scores.Add((int)GameTeam.Red, this.RedScore);
                scores = scores.OrderByDescending(d => d.Value).ToDictionary(d => d.Key, d => d.Value);

                this.WiningTeamTiles.Clear();
                KeyValuePair<int, int> bestScore = scores.ElementAt(0);
                List<int> winingTeams = new List<int>();
                foreach (KeyValuePair<int, int> data in scores.OrderByDescending(d => d.Value))
                {
                    if (bestScore.Value == data.Value) //wining team or tie :o
                    {
                        this.WiningTeamTiles.Add((int)data.Key, new List<RoomItemBattleBanzaiPatch>());
                        winingTeams.Add((int)data.Key);
                    }
                }

                foreach(RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
                {
                    if (user.GameType == GameType.BattleBanzai)
                    {
                        if (winingTeams.Contains((int)user.GameTeam))
                        {
                            this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Wave, new ValueHolder("VirtualID", user.VirtualID)));
                        }
                    }
                }

                foreach(RoomItemBattleBanzaiPatch tile in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemBattleBanzaiPatch)))
                {
                    if (tile.ExtraData == "5")
                    {
                        if (winingTeams.Contains((int)GameTeam.Red))
                        {
                            this.WiningTeamTiles[(int)GameTeam.Red].Add(tile);
                        }
                    }
                    else if (tile.ExtraData == "8")
                    {
                        if (winingTeams.Contains((int)GameTeam.Green))
                        {
                            this.WiningTeamTiles[(int)GameTeam.Green].Add(tile);
                        }
                    }
                    else if (tile.ExtraData == "11")
                    {
                        if (winingTeams.Contains((int)GameTeam.Blue))
                        {
                            this.WiningTeamTiles[(int)GameTeam.Blue].Add(tile);
                        }
                    }
                    else if (tile.ExtraData == "14")
                    {
                        if (winingTeams.Contains((int)GameTeam.Yellow))
                        {
                            this.WiningTeamTiles[(int)GameTeam.Yellow].Add(tile);
                        }
                    }
                }
                this.WiningBlink = 0;
            }
        }

        public void AddScore(RoomUnitUser scorer, GameTeam team, int amount)
        {
            int newScore = 0;
            switch(team)
            {
                case GameTeam.Green:
                    {
                        this.GreenScore += amount;
                        newScore = this.GreenScore;
                    }
                    break;
                case GameTeam.Blue:
                    {
                        this.BlueScore += amount;
                        newScore = this.BlueScore;
                    }
                    break;
                case GameTeam.Yellow:
                    {
                        this.YellowScore += amount;
                        newScore = this.YellowScore;
                    }
                    break;
                case GameTeam.Red:
                    {
                        this.RedScore += amount;
                        newScore = this.RedScore;
                    }
                    break;
            }

            this.Room.RoomWiredManager.ScoreChanged(scorer, newScore);
        }

        public void UpdateScore(GameTeam team)
        {
            switch (team)
            {
                case GameTeam.Green:
                    {
                        foreach(RoomItem item in this.Room.RoomGameManager.GreenScoreboards.Values)
                        {
                            item.ExtraData = this.GreenScore.ToString();
                            item.UpdateState(true, true);
                        }
                    }
                    break;
                case GameTeam.Blue:
                    {
                        foreach (RoomItem item in this.Room.RoomGameManager.BlueScoreboards.Values)
                        {
                            item.ExtraData = this.BlueScore.ToString();
                            item.UpdateState(true, true);
                        }
                    }
                    break;
                case GameTeam.Yellow:
                    {
                        foreach (RoomItem item in this.Room.RoomGameManager.YellowScoreboards.Values)
                        {
                            item.ExtraData = this.YellowScore.ToString();
                            item.UpdateState(true, true);
                        }
                    }
                    break;
                case GameTeam.Red:
                    {
                        foreach (RoomItem item in this.Room.RoomGameManager.RedScoreboards.Values)
                        {
                            item.ExtraData = this.RedScore.ToString();
                            item.UpdateState(true, true);
                        }
                    }
                    break;
            }
        }
    }
}
