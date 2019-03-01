using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Bots
{
    public class RoomBot : RoomUnitHuman, BotAI
    {
        public RoomBotData Data;
        public int RandomSpeechTimer;
        public int RandomMoveTimer;

        public RoomBot(RoomBotData data, Room room, int virtualId) : base(room, virtualId)
        {
            this.Data = data;
            this.RandomSpeechTimer = this.GetRandom().Next(10, 250);
            this.RandomMoveTimer = this.GetRandom().Next(10, 30);
        }

        public Random GetRandom()
        {
            return new Random((this.VirtualID ^ 2) + DateTime.Now.Millisecond);
        }

        public virtual void OnSelfEnterRoom()
        {

        }

        public virtual void OnSelfLeaveRoom(bool kicked)
        {

        }

        public virtual void OnUserSpeak(RoomUnitUser user, string message, bool shout)
        {
            if (!shout)
            {
                if (Math.Abs(this.X - user.X) + Math.Abs(this.Y - user.Y) <= 8)
                {
                    BotResponse response = this.Data.BotResponses.FirstOrDefault(r => r.Keywords.Any(s => message.ToLower().Contains(s.ToLower())));
                    if (response != null)
                    {
                        if (response.ResponseMode == 0)
                        {
                            this.Speak(response.ResponseText, false);
                        }
                        else if (response.ResponseMode == 1)
                        {
                            this.Speak(response.ResponseText, true);
                        }
                        else if (response.ResponseMode == 2)
                        {
                            ServerMessage whisper = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            whisper.Init(r63aOutgoing.Whisper);
                            whisper.AppendInt32(this.VirtualID);
                            whisper.AppendString(response.ResponseText);
                            whisper.AppendInt32(0); //gesture
                            whisper.AppendInt32(0); //links count
                            this.Room.SendToAll(whisper);
                        }

                        if (response.ServerID > 0)
                        {
                            user.SetHanditem(response.ServerID);
                        }
                    }
                }
            }
        }

        public virtual void OnRoomCycle()
        {
            this.RandomSpeechTimer--;
            if (this.RandomSpeechTimer <= 0)
            {
                if (this.Data.BotSpeechs.Count > 0)
                {
                    BotSpeech speech = this.Data.BotSpeechs[RandomUtilies.GetRandom(0, this.Data.BotSpeechs.Count - 1)];
                    this.Speak(speech.Text, speech.Shout);
                }
                this.RandomSpeechTimer = this.GetRandom().Next(10, 300);
            }

            this.RandomMoveTimer--;
            if (this.RandomMoveTimer <= 0)
            {
                if (this.Data.WalkMode == 0)
                {

                }
                else if (this.Data.WalkMode == 1)
                {
                    int x = RandomUtilies.GetRandom(0, this.Room.RoomGamemapManager.Model.MaxX);
                    int y = RandomUtilies.GetRandom(0, this.Room.RoomGamemapManager.Model.MaxY);
                    this.MoveTo(x, y);
                }
                else if (this.Data.WalkMode == 2)
                {
                    int x = RandomUtilies.GetRandom(this.Data.MinX, this.Data.MaxX);
                    int y = RandomUtilies.GetRandom(this.Data.MinY, this.Data.MinY);
                    this.MoveTo(x, y);
                }
                this.RandomMoveTimer = this.GetRandom().Next(10, 30);
            }
        }

        public override void Serialize(ServerMessage message)
        {
            message.AppendInt32(-1);
            message.AppendString(this.Data.Name);
            message.AppendString(this.Data.Motto);
            message.AppendString(this.Data.Look);
            message.AppendInt32(this.VirtualID);
            message.AppendInt32(this.X);
            message.AppendInt32(this.Y);
            message.AppendString(TextUtilies.DoubleWithDotDecimal(this.Z));
            message.AppendInt32(2); //dir
            message.AppendInt32(3);
        }

        public virtual void OnUserLeaveRoom(RoomUnitUser user)
        {

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
    }
}
