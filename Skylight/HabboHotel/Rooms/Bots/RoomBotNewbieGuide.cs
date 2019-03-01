using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Bots
{
    class RoomBotNewbieGuide : RoomBot
    {
        public int Ticks;
        public RoomBotNewbieGuide(RoomBotData data, Room room, int virtualId)
            : base(data, room, virtualId)
        {
            foreach (BotAction action in Skylight.GetGame().GetBotManager().GetBotActionsByTick(-1))
            {
                this.HandleBotAction(action);
            }

            GameClient roomOwner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.Room.RoomData.OwnerID);
            if (roomOwner != null)
            {
                Skylight.GetGame().GetAchievementManager().AddAchievement(roomOwner, "Student", 1);
            }
        }

        public override void OnRoomCycle()
        {
            this.Ticks++;

            foreach (BotAction action in Skylight.GetGame().GetBotManager().GetBotActionsByTick(this.Ticks))
            {
                this.HandleBotAction(action);
            }
        }

        public override void Speak(string message, bool shout, int bubble = 0)
        {
            if (shout)
            {
                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Shout, new ValueHolder("VirtualID", this.VirtualID, "Message", message, "Bubble", bubble)));
            }
            else
            {
                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Chat, new ValueHolder("VirtualID", this.VirtualID, "Message", message, "Bubble", bubble)));
            }
        }

        public override void OnSelfEnterRoom()
        {
            foreach (BotAction action in Skylight.GetGame().GetBotManager().GetBotActionsByTick(0))
            {
                this.HandleBotAction(action);
            }
        }

        public void HandleBotAction(BotAction action)
        {
            this.Unidle();

            if (action.Action == "setname")
            {
                this.Data.Name = action.Value;

                if (action.Tick != -1)
                {
                    this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.SetRoomUser, new ValueHolder("RoomUser", new List<RoomUnit>() { this })));
                }
            }
            else if (action.Action == "setlook")
            {
                this.Data.Look = action.Value;

                if (action.Tick != -1)
                {
                    this.Update();
                }
            }
            else if (action.Action == "setmotto")
            {
                this.Data.Motto = action.Value;

                if (action.Tick != -1)
                {
                    this.Update();
                }
            }
            else if (action.Action == "setlocation")
            {
                string[] data = action.Value.Split(',');

                if (data.Length == 2)
                {
                    RoomTile tile = this.Room.RoomGamemapManager.GetTile(int.Parse(data[0]), int.Parse(data[1]));
                    if (tile != null)
                    {
                        this.SetLocation(int.Parse(data[0]), int.Parse(data[1]), tile.GetZ(true));
                    }
                    else
                    {
                        this.SetLocation(int.Parse(data[0]), int.Parse(data[1]), 0);
                    }
                }
                else
                {
                    this.SetLocation(int.Parse(data[0]), int.Parse(data[1]), double.Parse(data[2]));
                }
            }
            else if (action.Action == "setrotation")
            {
                this.SetRotation(int.Parse(action.Value), true);
            }
            else if (action.Action == "move")
            {
                string[] data = action.Value.Split(',');
                this.MoveTo(int.Parse(data[0]), int.Parse(data[1]));
            }
            else if (action.Action == "say")
            {
                this.Speak(TextUtilies.FormatString(action.Value, Skylight.GetGame().GetGameClientManager().GetGameClientById(this.Room.RoomData.OwnerID)), false);
            }
            else if (action.Action == "whisper")
            {
                ServerMessage whisper = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                whisper.Init(r63aOutgoing.Whisper);
                whisper.AppendInt32(this.VirtualID);
                whisper.AppendString(TextUtilies.FormatString(action.Value, Skylight.GetGame().GetGameClientManager().GetGameClientById(this.Room.RoomData.OwnerID)));
                whisper.AppendInt32(0); //gesture
                whisper.AppendInt32(0); //links count
                this.Room.SendToAll(whisper);
            }
            else if (action.Action == "shout")
            {
                this.Speak(TextUtilies.FormatString(action.Value, Skylight.GetGame().GetGameClientManager().GetGameClientById(this.Room.RoomData.OwnerID)), true);
            }
            else if (action.Action == "wave")
            {
                this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Wave, new ValueHolder("VirtualID", this.VirtualID)));
            }
            else if (action.Action == "effect")
            {
                this.ApplyEffect(int.Parse(action.Value));
            }
            else if (action.Action == "handitem")
            {
                this.SetHanditem(int.Parse(action.Value));
            }
            else if (action.Action == "leave")
            {
                GameClient roomOwner = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.Room.RoomData.OwnerID);
                if (roomOwner != null)
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", roomOwner.GetHabbo().ID);
                        dbClient.ExecuteQuery("UPDATE users SET newbie_status = '2' WHERE id = @userId");
                    }
                    roomOwner.GetHabbo().NewbieStatus = 2;

                    Skylight.GetGame().GetAchievementManager().AddAchievement(roomOwner, "Graduate", 1);

                    this.Room.RoomUserManager.LeaveRoom(this);
                }
            }
        }

        public override void OnUserSpeak(RoomUnitUser user, string message, bool shout)
        {

        }

        public void Update()
        {
            ServerMessage update = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            update.Init(r63aOutgoing.UpdateUser);
            update.AppendInt32(this.VirtualID);
            update.AppendString(this.Data.Look);
            update.AppendString("M");
            update.AppendString(this.Data.Motto);
            update.AppendInt32(0);
            this.Room.SendToAll(update);
        }

        public override void OnUserLeaveRoom(RoomUnitUser user)
        {
            if (this.Room.RoomData.OwnerID == user.Session.GetHabbo().ID)
            {
                this.Room.RoomUserManager.LeaveRoom(this);
            }
        }
    }
}
