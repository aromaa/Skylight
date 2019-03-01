using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomEvent
    {
        public uint RoomID;
        public uint CreatorID;
        public string Name;
        public string Description;
        public int Category;
        public List<string> Tags;
        public DateTime StartTime;

        public RoomEvent(uint roomId, uint creatorId, string name, string description, int category, List<string> tags)
        {
            this.RoomID = roomId;
            this.CreatorID = creatorId;
            this.Name = name;
            this.Description = description;
            this.Category = category;
            this.Tags = tags;
            this.StartTime = DateTime.Now;
        }

        public ServerMessage Serialize()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.RoomEvent);
            message.AppendString(this.CreatorID.ToString()); //event creator id
            message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.CreatorID)); //event creator username
            message.AppendString(this.RoomID.ToString()); //room id
            message.AppendInt32(this.Category);
            message.AppendString(this.Name);
            message.AppendString(this.Description);
            message.AppendString(this.StartTime.ToShortTimeString()); //start time
            message.AppendInt32(this.Tags.Count);
            foreach(string tag in this.Tags)
            {
                message.AppendString(tag);
            }
            return message;
        }
    }
}
