using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomData
    {
        public readonly uint ID;
        public string Name;
        public string Description;
        public readonly uint OwnerID;
        public readonly string Type;
        public string Model;
        public RoomStateType State;
        public int Category;
        public int UsersNow;
        public int UsersMax;
        public string PublicCCTs;
        public int Score;
        public List<string> Tags;
        public RoomIcon RoomIcon;
        public string Password;
        public string Wallpaper;
        public string Floor;
        public string Landscape;
        public bool AllowPets;
        public bool AllowPetsEat;
        public bool AllowWalkthrough;
        public bool Hidewalls;
        public int Wallthick;
        public int Floorthick;

        public RoomData(DataRow dataRow)
        {
            if (dataRow != null)
            {
                this.ID = (uint)dataRow["id"];
                this.Name = (string)dataRow["name"];
                this.Description = (string)dataRow["description"];
                this.OwnerID = (uint)dataRow["ownerid"];
                this.Type = (string)dataRow["type"];
                this.Model = (string)dataRow["model"];
                this.State = (string)dataRow["state"] == "password" ? RoomStateType.PASSWORD : (string)dataRow["state"] == "locked" ? RoomStateType.LOCKED : RoomStateType.OPEN;
                this.Category = (int)dataRow["category"];
                this.UsersNow = (int)dataRow["users_now"]; //maybe we need this sometimes :3
                this.UsersMax = (int)dataRow["users_max"];
                this.PublicCCTs = (string)dataRow["public_ccts"];
                this.Score = (int)dataRow["score"];
                this.Tags = ((string)dataRow["tags"]).Split(';').ToList();
                this.RoomIcon = new RoomIcon((int)dataRow["icon_bg"], (int)dataRow["icon_fg"], (string)dataRow["icon_items"]);
                this.Password = (string)dataRow["password"];
                this.Wallpaper = (string)dataRow["wallpaper"];
                this.Floor = (string)dataRow["floor"];
                this.Landscape = (string)dataRow["landscape"];
                this.AllowPets = TextUtilies.StringToBool((string)dataRow["allow_pets"]);
                this.AllowPetsEat = TextUtilies.StringToBool((string)dataRow["allow_pets_eat"]);
                this.AllowWalkthrough = TextUtilies.StringToBool((string)dataRow["allow_walkthrough"]);
                this.Hidewalls = TextUtilies.StringToBool((string)dataRow["hidewalls"]);
                this.Wallthick = (int)dataRow["wallthick"];
                this.Floorthick = (int)dataRow["floorthick"];
            }
            else
            {
                this.NullValues();
            }
        }

        public void NullValues()
        {

        }

        public void Serialize(ServerMessage message, bool showEvent)
        {
            message.AppendUInt(this.ID);
            message.AppendBoolean(showEvent); //event
            message.AppendStringWithBreak(this.Name); //room name / event name
            message.AppendStringWithBreak(Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.OwnerID));
            message.AppendInt32((int)this.State); //state
            message.AppendInt32(this.UsersNow);
            message.AppendInt32(this.UsersMax); //users max
            message.AppendStringWithBreak(this.Description); //event desc / room desc
            message.AppendInt32(0); //unknown
            message.AppendBoolean(true); //unknown
            message.AppendInt32(this.Score); //score
            message.AppendInt32(this.Category); //category / event category
            message.AppendStringWithBreak(""); //event start time
            message.AppendInt32(this.Tags.Count); //tags count
            foreach(string tag in this.Tags)
            {
                message.AppendStringWithBreak(tag);
            }
            this.RoomIcon.Serialize(message);
            message.AppendBoolean(this.AllowPets); //allow pets
            message.AppendBoolean(this.AllowPetsEat); //allow pets eating
        }
    }
}
