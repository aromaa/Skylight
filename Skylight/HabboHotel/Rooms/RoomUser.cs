using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomUser
    {
        private GameClient Session;
        public readonly int VirtualID;
        private Room Room;

        private int X;
        private int Y;
        private double Z;
        private int BodyRotation;
        private int HeadRotation;
        public bool NeedUpdate;
        private Dictionary<string, string> Statusses;
        public int TargetX;
        public int TargetY;
        public int NextStepX;
        public int NextStepY;
        public bool Moving;
        public bool NextStep;

        public RoomUser(GameClient gameClient, int virtualId, Room room)
        {
            this.Statusses = new Dictionary<string, string>();

            this.Session = gameClient;
            this.VirtualID = virtualId;
            this.Room = room;
        }

        public int GetX
        {
            get
            {
                return this.X;
            }
        }
        public int GetY
        {
            get
            {
                return this.Y;
            }
        }

        public double GetZ
        {
            get
            {
                return this.Z;
            }
        }

        public GameClient GetClient()
        {
            return this.Session;
        }

        public void SetLocation(int x, int y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.NeedUpdate = true;
        }

        public void SetRotation(int rot)
        {
            this.BodyRotation = rot;
            this.HeadRotation = rot;
            this.NeedUpdate = true;
        }

        public void Unidle()
        {

        }

        public void Speak(string message, bool shout)
        {
            if (!this.Session.GetHabbo().IsMuted())
            {
                this.Unidle();
                if (!message.StartsWith(":") || !this.HandleCommand(message.Substring(1)))
                {
                    ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    if (shout)
                    {
                        Message.Init(r63aOutgoing.Shout);
                    }
                    else
                    {
                        Message.Init(r63aOutgoing.Say);
                    }
                    Message.AppendInt32(this.VirtualID);
                    Message.AppendStringWithBreak(message);
                    Message.AppendInt32(this.SmileyFace(message));
                    Message.AppendInt32(0); //links count
                    Message.AppendInt32(0); //unknown
                    this.Room.SendToAll(Message, null);
                }
            }
            else
            {
                this.Session.SendNotif("You have are muted!");
            }
        }

        public void RemoveStatus(string key)
        {
            if (this.Statusses.ContainsKey(key))
            {
                this.Statusses.Remove(key);
                this.NeedUpdate = true;
            }
        }

        public void AddStatus(string key, string value)
        {
            if (this.Statusses.ContainsKey(key))
            {
                this.Statusses.Add(key, value);
            }
            else
            {
                this.Statusses[key] = value;
            }

            this.NeedUpdate = true;
        }

        public bool HasStatus(string key)
        {
            return this.Statusses.ContainsKey(key);
        }

        public int SmileyFace(string message)
        {
            return 0;
        }

        public bool HandleCommand(string command)
        {
            string[] Params = command.Split(' ');

            try
            {
                switch(Params[0])
                {
                    case "pickall":
                        {
                            if (this.Room != null && this.Room.IsOwner(this.Session))
                            {
                                this.Room.RoomItemManager.Pickall(this.Session);
                            }
                            return true;
                        }
                    case "unload":
                        if (this.Room != null && this.Room.IsOwner(this.Session))
                        {
                            Skylight.GetGame().GetRoomManager().UnloadRoom(this.Room);
                        }
                        return true;
                    case "emptyitems":
                        this.Session.GetHabbo().GetInventoryManager().DeleteAllItems();
                        return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error when trying run command! " + ex.ToString());
                return false;
            }

            return false;
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendUInt(this.Session.GetHabbo().ID);
            message.AppendStringWithBreak(this.Session.GetHabbo().Username);
            message.AppendStringWithBreak(this.Session.GetHabbo().Motto);
            message.AppendStringWithBreak(this.Session.GetHabbo().Look);
            message.AppendInt32(this.VirtualID);
            message.AppendInt32(this.X);
            message.AppendInt32(this.Y);
            message.AppendStringWithBreak(TextUtilies.DoubleWithDotDecimal(this.Z));
            message.AppendInt32(2);
            message.AppendInt32(1);
            message.AppendStringWithBreak(this.Session.GetHabbo().Gender.ToLower());
            message.AppendInt32(-1);
            message.AppendInt32(-1); //fav group
            message.AppendInt32(-1);
            message.AppendStringWithBreak("");
            message.AppendInt32(0); //achievement score
        }

        public void SerializeStatus(ServerMessage message)
        {
            message.AppendInt32(this.VirtualID);
            message.AppendInt32(this.X);
            message.AppendInt32(this.Y);
            message.AppendStringWithBreak(TextUtilies.DoubleWithDotDecimal(this.Z));
            message.AppendInt32(this.BodyRotation);
            message.AppendInt32(this.HeadRotation);
            message.AppendString("/");
            foreach (KeyValuePair<string, string> value in this.Statusses)
            {
                message.AppendString(value.Key);
                message.AppendString(" ");
                message.AppendString(value.Value);
                message.AppendString("/");
            }
            message.AppendStringWithBreak("/");
        }

        public void MoveTo(int x, int y)
        {
            if (this.Room.RoomGamemapManager.CoordsInsideRoom(x, y) && this.Room.RoomGamemapManager.GetTile(x, y).CanUserMoveToTile)
            {
                this.Unidle();
                this.Moving = true;
                this.TargetX = x;
                this.TargetY = y;
            }
            else
            {

            }
        }
    }
}
