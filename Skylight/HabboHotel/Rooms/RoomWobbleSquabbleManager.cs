using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomWobbleSquabbleManager
    {
        private Room Room;
        private Stopwatch GameTimer;
        public WSStatus Status { get; private set; }

        public RoomUnitUser LeftUser;
        public RoomUnitUser RightUser;

        public RoomWobbleSquabbleManager(Room room)
        {
            this.Room = room;
            this.GameTimer = Stopwatch.StartNew();
            this.Status = WSStatus.NotRunning;
        }

        public void TryStart()
        {
            if (this.LeftUser != null && this.RightUser != null)
            {
                this.GameTimer.Restart();
                this.Status = WSStatus.Countdown;

                this.LeftUser.WSPlayer = new WSPlayer(-3, true);
                this.RightUser.WSPlayer = new WSPlayer(4, false);

                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message.Init(r26Outgoing.StartWobbleCountdown);
                message.AppendString("0:" + this.LeftUser.VirtualID, 13);
                message.AppendString("1:" + this.RightUser.VirtualID, 13);
                this.Room.SendToAll(message);
            }
        }

        public void OnCycle()
        {
            if (this.Status == WSStatus.Countdown)
            {
                if (this.GameTimer.Elapsed.TotalSeconds >= 3)
                {
                    this.GameTimer.Restart();
                    this.Status = WSStatus.Running;

                    ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    message.Init(r26Outgoing.StartWobbleGame);
                    message.AppendString("0:" + this.LeftUser.VirtualID, 13);
                    message.AppendString("1:" + this.RightUser.VirtualID, 13);
                    this.Room.SendToAll(message);
                }
            }
            else if (this.Status == WSStatus.Running)
            {
                if (this.GameTimer.Elapsed.TotalSeconds <= 90)
                {
                    if (this.LeftUser.WSPlayer.NeedUpdate || this.RightUser.WSPlayer.NeedUpdate)
                    {
                        this.LeftUser.WSPlayer.NeedUpdate = false;
                        this.RightUser.WSPlayer.NeedUpdate = false;

                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                        message.Init(r26Outgoing.WobbleUpdate);
                        message.AppendString(this.LeftUser.WSPlayer.Location.ToString(), 9);
                        message.AppendString(this.LeftUser.WSPlayer.Lean.ToString(), 9);
                        message.AppendString(this.LeftUser.WSPlayer.Action, 9);
                        message.AppendString(this.LeftUser.WSPlayer.BeenHit ? "h" : "", 13);

                        message.AppendString(this.RightUser.WSPlayer.Location.ToString(), 9);
                        message.AppendString(this.RightUser.WSPlayer.Lean.ToString(), 9);
                        message.AppendString(this.RightUser.WSPlayer.Action, 9);
                        message.AppendString(this.RightUser.WSPlayer.BeenHit ? "h" : "", 9);
                        this.Room.SendToAll(message);

                        this.LeftUser.WSPlayer.Action = "-";
                        this.RightUser.WSPlayer.Action = "-";

                        this.LeftUser.WSPlayer.BeenHit = false;
                        this.RightUser.WSPlayer.BeenHit = false;
                    }

                    if (Math.Abs(this.LeftUser.WSPlayer.Lean) >= 100 && Math.Abs(this.RightUser.WSPlayer.Lean) >= 100)
                    {
                        this.Done(true, true);
                    }
                    else if (Math.Abs(this.LeftUser.WSPlayer.Lean) >= 100)
                    {
                        this.Done(true, false);
                    }
                    else if (Math.Abs(this.RightUser.WSPlayer.Lean) >= 100)
                    {
                        this.Done(false, true);
                    }
                }
                else
                {
                    ServerMessage messageTimeout = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    messageTimeout.Init(r26Outgoing.WobbleTimeout);
                    this.Room.SendToAll(messageTimeout);

                    if (this.RightUser.WSPlayer.HitsTakenTotal == this.LeftUser.WSPlayer.HitsTakenTotal)
                    {
                        this.Done(true, true); //Tie I guess
                    }
                    else
                    {
                        if (this.RightUser.WSPlayer.HitsTakenTotal > this.LeftUser.WSPlayer.HitsTakenTotal)
                        {
                            this.Done(false, true);
                        }
                        else
                        {
                            this.Done(true, false);
                        }
                    }
                }
            }
            else if (this.Status == WSStatus.Ending)
            {
                if (this.GameTimer.Elapsed.TotalSeconds >= 1.5)
                {
                    this.Status = WSStatus.NotRunning;

                    ServerMessage messageEnd = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    messageEnd.Init(r26Outgoing.WobbleEnd);
                    this.Room.SendToAll(messageEnd);
                }
            }
        }

        public void Done(bool left, bool right)
        {
            this.GameTimer.Restart();
            this.Status = WSStatus.Ending;

            bool tie = left && right;
            if (left)
            {
                this.LeftUser.RestrictMovementType &= ~RestrictMovementType.Server;
                this.LeftUser.MoveTo(this.LeftUser.X + (this.LeftUser.WSPlayer.Lean >= 0 ? 1 : -1), this.LeftUser.Y);
                this.LeftUser.AddStatus("swim", "");

                this.LeftUser.WSPlayer = null;
                this.LeftUser = null;
            }

            if (right)
            {
                this.RightUser.RestrictMovementType &= ~RestrictMovementType.Server;
                this.RightUser.MoveTo(this.RightUser.X + (this.RightUser.WSPlayer.Lean >= 0 ? 1 : -1), this.RightUser.Y);
                this.RightUser.AddStatus("swim", "");

                this.RightUser.WSPlayer = null;
                this.RightUser = null;
            }

            if (tie)
            {
                ServerMessage messageTie = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                messageTie.Init(r26Outgoing.WobbleTie);
                this.Room.SendToAll(messageTie);
            }
            else
            {
                ServerMessage messageWinner = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                messageWinner.Init(r26Outgoing.WonWobble);
                messageWinner.AppendString(right ? "0" : "1", null);
                this.Room.SendToAll(messageWinner);
            }
        }

        public bool IsOpen(int location)
        {
            return this.LeftUser.WSPlayer.Location != location && this.RightUser.WSPlayer.Location != location && location < 4 && location > -3;
        }

        public void Hit(RoomUnitUser user, bool left)
        {
            if (user.WSPlayer.LeftSide)
            {
                if (user.WSPlayer.Location + 1 == this.RightUser.WSPlayer.Location)
                {
                    this.RightUser.WSPlayer.Lean += left ? -10 - RandomUtilies.GetRandom().Next(0, 10) : 10 + RandomUtilies.GetRandom().Next(0, 10);
                    this.RightUser.WSPlayer.HitsTakenTotal++;
                    this.RightUser.WSPlayer.BeenHit = true;
                }
                else
                {
                    user.WSPlayer.Lean += left ? -10 - RandomUtilies.GetRandom().Next(0, 10) : 10 + RandomUtilies.GetRandom().Next(0, 10);
                }
            }
            else
            {
                if (user.WSPlayer.Location - 1 == this.LeftUser.WSPlayer.Location)
                {
                    this.LeftUser.WSPlayer.Lean -= left ? -10 - RandomUtilies.GetRandom().Next(0, 10) : 10 + RandomUtilies.GetRandom().Next(0, 10);
                    this.LeftUser.WSPlayer.HitsTakenTotal++;
                    this.LeftUser.WSPlayer.BeenHit = true;
                }
                else
                {
                    user.WSPlayer.Lean += left ? -10 - RandomUtilies.GetRandom().Next(0, 10) : 10 + RandomUtilies.GetRandom().Next(0, 10);
                }
            }
        }

        public void Leave(RoomUnitUser user)
        {
            if (this.Status == WSStatus.Running || this.Status == WSStatus.Countdown)
            {
                this.Status = WSStatus.NotRunning;

                ServerMessage messageEnd = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                messageEnd.Init(r26Outgoing.WobbleEnd);
                this.Room.SendToAll(messageEnd);

                if (this.LeftUser == user)
                {
                    this.LeftUser = null;
                }
                else if (this.RightUser == user)
                {
                    this.RightUser = null;
                }
            }
        }
    }
}
