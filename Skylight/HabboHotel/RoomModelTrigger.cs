using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel
{
    public class RoomModelTrigger
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public string Type { get; private set; }
        public string[] Data { get; private set; }

        public RoomModelTrigger(int x, int y, string type, string data)
        {
            this.X = x;
            this.Y = y;
            this.Type = type;
            this.Data = data.Split(' ');
        }

        public void UserWalkOff(RoomUnit unit)
        {
            if (unit.IsRealUser && unit is RoomUnitUser user)
            {
                if (this.Type == "pool_exit" || this.Type == "ws_right" || this.Type == "ws_left")
                {
                    user.Override = false;
                }
            }
        }

        public void UserWalkOn(RoomUnit unit)
        {
            if (unit.IsRealUser && unit is RoomUnitUser user)
            {
                if (this.Type == "curtains")
                {
                    user.RestrictMovementType |= RestrictMovementType.Client;
                    user.ChangingSwimsuit = true;
                    user.StopMoving();

                    ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    message.Init(r26Outgoing.Swimsuit);
                    user.Session.SendMessage(message);

                    ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    message2.Init(r26Outgoing.SpecialCast);
                    message2.AppendString(this.Data[0]);
                    message2.AppendString("close");
                    user.Room.SendToAll(message2);
                }
                else if (this.Type == "pool_enter")
                {
                    if (!string.IsNullOrEmpty(user.Swimsuit) && !user.Metadata.Remove("single_pool_enter_ignore"))
                    {
                        user.AddStatus("swim", "");

                        ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                        message2.Init(r26Outgoing.SpecialCast);
                        message2.AppendString(this.Data[0]);
                        message2.AppendString("enter");
                        user.Room.SendToAll(message2);

                        user.MoveTo(int.Parse(this.Data[1]), int.Parse(this.Data[2]));
                        user.Metadata["single_pool_exit_ignore"] = null;
                    }
                }
                else if (this.Type == "pool_exit")
                {
                    if (!user.Metadata.Remove("single_pool_exit_ignore"))
                    {
                        user.RemoveStatus("swim");

                        ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                        message2.Init(r26Outgoing.SpecialCast);
                        message2.AppendString(this.Data[0]);
                        message2.AppendString("exit");
                        user.Room.SendToAll(message2);

                        user.Override = true;
                        user.MoveTo(int.Parse(this.Data[1]), int.Parse(this.Data[2]));
                        user.Metadata["single_pool_enter_ignore"] = null;
                    }
                }
                else if (this.Type == "ws_left")
                {
                    if (this.Data[0] == "join")
                    {
                        user.RemoveStatus("swim");

                        user.RestrictMovementType |= RestrictMovementType.Client;
                        user.Override = true;
                        user.MoveTo(this.X, this.Y - 1);
                    }
                    else if (this.Data[0] == "queue")
                    {
                        user.RestrictMovementType |= RestrictMovementType.Client;
                        user.Override = true;
                        user.MoveTo(this.X, this.Y - 1);
                    }
                    else if (this.Data[0] == "play")
                    {
                        user.RestrictMovementType |= RestrictMovementType.Client;
                        user.Room.RoomGameManager.RoomWobbleSquabbleManager.LeftUser = user;
                        user.Room.RoomGameManager.RoomWobbleSquabbleManager.TryStart();
                    }
                }
                else if (this.Type == "ws_right")
                {
                    if (this.Data[0] == "join")
                    {
                        user.RemoveStatus("swim");

                        user.RestrictMovementType |= RestrictMovementType.Client;
                        user.Override = true;
                        user.MoveTo(this.X, this.Y + 1);
                    }
                    else if (this.Data[0] == "queue")
                    {
                        user.RestrictMovementType |= RestrictMovementType.Client;
                        user.Override = true;
                        user.MoveTo(this.X, this.Y + 1);
                    }
                    else if (this.Data[0] == "play")
                    {
                        user.RestrictMovementType |= RestrictMovementType.Client;
                        user.Room.RoomGameManager.RoomWobbleSquabbleManager.RightUser = user;
                        user.Room.RoomGameManager.RoomWobbleSquabbleManager.TryStart();
                    }
                }
                else if (this.Type == "open_gameboard")
                {
                    user.Room.RoomGameManager.Gameboards[uint.Parse(this.Data[1])].Join(user);
                }
            }
        }
    }
}
