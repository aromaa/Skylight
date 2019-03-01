using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomUnit
    {
        public static readonly Point EmptyPoint = new Point(-1, -1);
        public static readonly Point3D EmptyPoint3D = new Point3D(-1, -1, -1);

        public readonly Room Room;
        public readonly int VirtualID;

        private Point3D location;
        public Point3D Location => this.location;
        public Point XY => this.location.XY;
        public int X => this.Location.X;
        public int Y => this.Location.Y;
        public double Z => this.Location.Z;

        public int HeadRotation { get; set; }
        public int BodyRotation { get; set; }

        public Point3D NextStep { get; private set; } = RoomUnit.EmptyPoint3D;
        public int NextStepY => this.NextStep.Y;
        public int NextStepX => this.NextStep.X;
        public double NextStepZ => this.NextStep.Z;
        public bool HasNextStep
        {
            get => this.NextStep != RoomUnit.EmptyPoint3D;
            set => this.NextStep = !value ? RoomUnit.EmptyPoint3D : throw new NotSupportedException("Use MoveTo method");
        }

        public Point Target { get; private set; } = RoomUnit.EmptyPoint;
        public int TargetX => this.Target.X;
        public int TargetY => this.Target.Y;
        public bool HasTarget
        {
            get => this.Target != RoomUnit.EmptyPoint;
            set => this.Target = !value ? RoomUnit.EmptyPoint : throw new NotSupportedException("Use MoveTo method");
        }

        private RoomUnit rider;
        public RoomUnit Rider
        {
            get => this.rider;
            set
            {
                if (value != null)
                {
                    value.riding = this;

                    if (this.rider != null)
                    {
                        //Update location?
                        this.rider.riding = null;
                        this.rider = null;
                    }
                }
                else
                {
                    if (this.rider != null)
                    {
                        this.rider.riding = null;
                    }
                }

                this.rider = value;
            }
        }

        private RoomUnit riding;
        public RoomUnit Riding
        {
            get => this.riding;
            set => (this.riding = value).rider = this;
        }

        public bool NeedUpdate { get; set; }

        private RoomTile currentTile;
        public RoomTile CurrentTile => this.currentTile ?? (this.currentTile = this.Room.RoomGamemapManager.GetTile(this.X, this.Y));

        public bool Moving
        {
            get => this.HasTarget ? this.TargetX != this.X || this.TargetY != this.Y : false;
            set => this.HasTarget = value;
        }

        protected Dictionary<string, string> Statuses { get; }
        public Dictionary<string, double> StatusesLifetime { get; }

        public int UpdateUserStateTimer { get; set; }

        public RestrictMovementType RestrictMovementType { get; set; }

        /// <summary>
        /// If this unit can be casted to RoomUnitUser
        /// </summary>
        public virtual bool IsRealUser => false;
        public bool Moonwalk { get; set; }
        public string MovingStatusValue => this.NextStepX + "," + this.NextStepY + "," + TextUtilies.DoubleWithDotDecimal(this.NextStepZ);

        public Dictionary<string, string> Metadata { get; }

        public RoomUnit(Room room, int virtualId)
        {
            this.Statuses = new Dictionary<string, string>(5);
            this.StatusesLifetime = new Dictionary<string, double>(0);
            this.Metadata = new Dictionary<string, string>(0);

            this.Room = room;
            this.VirtualID = virtualId;
        }

        public void MoveCycle()
        {
            if (this.HasNextStep)
            {
                if (this.Riding == null) //The user that this user is riding will control this user movement example horse
                {
                    this.Room.UserWalkOff(this);
                    if (this.HasNextStep) //UserWalkOff might deny walk on due to example wired teleporting user away
                    {
                        this.Room.AboutToWalkOn(this); //Some items might need to know thte last and current tile example skateboard rails rely on this
                        if (this.HasNextStep) //AboutToWalkOn might deny walk for whatever reason, not used at the moment by emulator itself
                        {
                            if (this.IsRealUser && this is RoomUnitUser user_ && this.NextStepX == this.Room.RoomGamemapManager.Model.DoorX && this.NextStepY == this.Room.RoomGamemapManager.Model.DoorY)
                            {
                                this.Room.RoomUserManager.LeaveRoom(user_.Session, false);
                            }
                            else
                            {
                                this.SetLocation(this.NextStep, true, false, true);
                                this.RemoveStatus("mv");

                                this.Room.UserWalkOn(this);

                                this.HasNextStep = false;
                            }
                        }
                    }
                }
                else
                {
                    this.HasNextStep = false;
                }
            }

            if (this.Moving)
            {
                if ((this.RestrictMovementType & RestrictMovementType.Server) == 0 && this.Riding == null)
                {
                    RoomTile tile = DreamPathfinder.GetNearestRoomTile(this.Location, this.Target, this.Room, this, !this.Room.DisableDiagonal, this.Room.RoomData.AllowWalkthrough);
                    if (tile.X != this.X || tile.Y != this.Y)
                    {
                        this.ChangeTilesTo(this.CurrentTile, this.Room.RoomGamemapManager.GetTile(tile.X, tile.Y));

                        this.NextStep = new Point3D(tile.X, tile.Y, tile.GetZ(!tile.IsSeat && !tile.IsBed));

                        this.SetRotation(this.Moonwalk ? WalkRotation.Moonwalk(this.X, this.Y, tile.X, tile.Y) : WalkRotation.Walk(this.X, this.Y, tile.X, tile.Y), true);

                        this.AddStatus("mv", this.MovingStatusValue);
                        this.RemoveStatus("sit");
                        this.RemoveStatus("lay");
                    }
                }
            }
            else
            {
                this.Room.RoomUserManager.MovingUsers.TryRemove(this.VirtualID, out RoomUnit trash);
            }
        }

        public virtual void Cycle()
        {
            foreach (KeyValuePair<string, double> timeouts in this.StatusesLifetime.ToList())
            {
                if (TimeUtilies.GetUnixTimestamp() >= timeouts.Value)
                {
                    this.RemoveStatus(timeouts.Key);
                    this.StatusesLifetime.Remove(timeouts.Key);
                }
            }

            if (this.UpdateUserStateTimer > 0)
            {
                if (--this.UpdateUserStateTimer <= 0)
                {
                    this.UpdateState();
                }
            }
        }

        /// <summary>
        /// Sets the user location and updates the state to get the proper Z
        /// </summary>
        /// <param name="xy"></param>
        public void SetLocation(Point xy, bool needUpdate = true, bool updateTiles = true, bool updateState = false)
        {
            this.SetLocation(xy.X, xy.Y, needUpdate, updateTiles, updateState);
        }

        /// <summary>
        /// Sets the user location and updates the state to get the proper Z
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetLocation(int x, int y, bool needUpdate = true, bool updateTiles = true, bool updateState = false)
        {
            this.SetLocation(x, y, 0, needUpdate, updateTiles, updateState);
        }

        public void SetLocation(Point xy, double z, bool needUpdate = true, bool updateTiles = true, bool updateState = false)
        {
            this.SetLocation(xy.X, xy.Y, z, needUpdate, updateTiles, updateState);
        }

        public void SetLocation(Point3D point, bool needUpdate = true, bool updateTiles = true, bool updateState = false)
        {
            this.SetLocation(point.X, point.Y, point.Z, needUpdate, updateTiles, updateState);
        }

        public void SetLocation(int x, int y, double z, bool needUpdate = true, bool updateTiles = true, bool updateState = false)
        {
            if (needUpdate)
            {
                this.NeedUpdate = true;
            }

            if (updateTiles && (updateTiles = this.Location.X != x || this.Location.Y != y))
            {
                this.CurrentTile.UsersOnTile.Remove(this.VirtualID);
            }

            if (this.location.X != x ||this.location.Y != y)
            {
                this.currentTile = null;
            }

            this.location.X = x;
            this.location.Y = y;

            if (!updateState)
            {
                this.location.Z = z;
            }
            else
            {
                this.UpdateState();
            }

            if (updateTiles)
            {
                this.CurrentTile.UsersOnTile.Add(this.VirtualID, this);
            }

            this.Rider?.SetLocation(x, y, z + 1, true, true, false);
        }

        public void AddStatus(string key, string value, double lifetime = 0)
        {
            this.Statuses.Add(key, value);

            if (lifetime > 0)
            {
                this.StatusesLifetime[key] = TimeUtilies.GetUnixTimestamp() + lifetime;
            }

            this.NeedUpdate = true;
        }

        public void SetStatus(string key, string value)
        {
            this.Statuses[key] = value;
            this.NeedUpdate = true;
        }

        public bool HasStatus(string key)
        {
            return this.Statuses.ContainsKey(key);
        }

        public bool RemoveStatus(string key)
        {
            this.NeedUpdate = true;
            return this.Statuses.Remove(key);
        }

        public void SetRotation(int rot, bool force)
        {
            if (!force)
            {
                if (!this.HasStatus("lay"))
                {
                    int num = this.HeadRotation - rot;
                    this.BodyRotation = this.HeadRotation;
                    if (this.HasStatus("sit"))
                    {
                        if (this.HeadRotation == 2 || this.HeadRotation == 4)
                        {
                            if (num > 0)
                            {
                                this.BodyRotation = this.HeadRotation - 1;
                            }
                            else
                            {
                                if (num < 0)
                                {
                                    this.BodyRotation = this.HeadRotation + 1;
                                }
                            }
                        }
                        else
                        {
                            if (this.HeadRotation == 0 || this.HeadRotation == 6)
                            {
                                if (num > 0)
                                {
                                    this.BodyRotation = this.HeadRotation - 1;
                                }
                                else
                                {
                                    if (num < 0)
                                    {
                                        this.BodyRotation = this.HeadRotation + 1;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (num <= -2 || num >= 2)
                        {
                            this.BodyRotation = rot;
                            this.HeadRotation = rot;
                        }
                        else
                        {
                            this.BodyRotation = rot;
                        }
                    }

                    this.NeedUpdate = true;
                }
            }
            else
            {
                this.BodyRotation = rot;
                this.HeadRotation = rot;

                this.NeedUpdate = true;
            }
        }

        public void ChangeTilesTo(RoomTile from, RoomTile to)
        {
            from?.UsersOnTile.Remove(this.VirtualID);
            to?.UsersOnTile.Add(this.VirtualID, this);

            this.Rider?.ChangeTilesTo(from, to);
        }

        public void UpdateState()
        {
            RoomTile tile = this.CurrentTile;
            if (tile != null)
            {
                this.RemoveStatus("sit");
                this.RemoveStatus("lay");

                if (tile.IsSeat)
                {
                    this.SetLocation(this.X, this.Y, tile.GetZ(false), true, false);

                    if (tile.HigestRoomItem != null)
                    {
                        this.AddStatus("sit", TextUtilies.DoubleWithDotDecimal(tile.HigestRoomItem.BaseItem.Height));
                        this.SetRotation(tile.HigestRoomItem.Rot, true);
                    }
                    else
                    {
                        this.AddStatus("sit", TextUtilies.DoubleWithDotDecimal(1.0));
                        this.SetRotation(this.Room.RoomGamemapManager.Model.Rotation[this.X, this.Y], true);
                    }
                }
                else if (tile.IsBed)
                {
                    this.AddStatus("lay", TextUtilies.DoubleWithDotDecimal(tile.HigestRoomItem.BaseItem.Height));

                    this.SetLocation(this.X, this.Y, tile.GetZ(false), true, false);
                    this.SetRotation(tile.HigestRoomItem.Rot, true);
                }
                else
                {
                    this.SetLocation(this.X, this.Y, tile.GetZ(true), true, false);
                }
            }
        }

        public virtual void MoveTo(int x, int y)
        {
            if (this.Room.RoomGamemapManager.CoordsInsideRoom(x, y) && !this.Room.RoomGamemapManager.GetTile(x, y).IsInUse)
            {
                this.Target = new Point(x, y);

                this.Room.RoomUserManager.MovingUsers.TryAdd(this.VirtualID, this);
            }
        }

        public virtual void Serialize(ServerMessage message)
        {
            throw new NotImplementedException();
        }
        
        public virtual void Speak(string message, bool shout, int bubble = 0)
        {
            throw new NotImplementedException();
        }

        public void StopMoving()
        {
            this.RemoveStatus("mv");
            if (this.Moving)
            {
                this.Moving = false;
            }

            if (this.HasNextStep)
            {
                this.Room.RoomGamemapManager.GetTile(this.NextStepX, this.NextStepY).UsersOnTile.Remove(this.VirtualID);
                this.Room.RoomGamemapManager.GetTile(this.X, this.Y).UsersOnTile.Add(this.VirtualID, this);

                this.HasNextStep = false;
            }
        }

        public virtual void SerializeStatus(ServerMessage message)
        {
            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                message.AppendInt32(this.VirtualID);
                message.AppendInt32(this.X);
                message.AppendInt32(this.Y);
                message.AppendString(TextUtilies.DoubleWithDotDecimal(this.Z));
                message.AppendInt32(this.BodyRotation);
                message.AppendInt32(this.HeadRotation);
            }
            else
            {
                message.AppendString(this.VirtualID + " " + this.X + "," + this.Y + "," + TextUtilies.DoubleWithDotDecimal(this.Z) + "," + this.BodyRotation + "," + this.HeadRotation, null);
            }

            string status = "/";
            foreach (KeyValuePair<string, string> value in this.Statuses)
            {
                if (status.Length > 1)
                {
                    status += "/";
                }

                status += value.Key + " " + value.Value;
            }

            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                message.AppendString(status);
            }
            else
            {
                message.AppendString(status, (byte)13);
            }
        }

        public static int GetGesture(string message)
        {
            if (message.Contains(":)") || message.Contains("=)") || message.Contains("=]") || message.Contains(":d") || message.Contains("=d") || message.Contains(":>")) //happy
            {
                return 1;
            }
            else if (message.Contains(">:(") || message.Contains(":@")) //angry
            {
                return 2;
            }
            else if (message.Contains(":o") || message.Contains(";o")) //surprised
            {
                return 3;
            }
            else if (message.Contains(":(") || message.Contains("=(") || message.Contains("=[") || message.Contains(":<")) //sad
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }
    }
}
